using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.ComponentModel.Design;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace Helpers
{
    internal static class VolunteerManager
    {
        internal static ObserverManager Observers = new(); //stage 5

        private static IDal s_dal = DalApi.Factory.Get; //stage 4
        private static IBl s_bl = BlApi.Factory.Get();
        private class OSMGeocodeResponse
        {
            public string display_name { get; set; }
        }
        /// <summary>
        /// Function to check the correctness of volunteer details
        /// </summary>
        /// <param name="volunteer">Volunteer for testing</param>
        /// <returns>true if correct</returns>
        /// <exception cref="BO.BlInvalidValueException">Invalid volunteer details</exception>

        public static bool IntegrityCheck(BO.Volunteer volunteer)
        {
            if (volunteer.MaxDistanceForCall < 0)
                throw new BO.BlInvalidValueException("max distance for call must be positive");
            //Email health check
            if (!Regex.IsMatch(volunteer.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {

            }
            //Phone check
            if (!Regex.IsMatch(volunteer.Phone, @"^\d{10}$"))
            {
                throw new BO.BlInvalidValueException("Invalid phone number format");
            }
            // ID check
            if (!IsValidIsraeliID(volunteer.Id))
            {
                throw new BO.BlInvalidValueException("Invalid Israeli ID number");
            }

            return true;
        }
        /// <summary>
        /// Checking if the id is valid
        /// </summary>
        /// <param name="id">id to check</param>
        /// <returns> true if the ID is valid, otherwise false</returns>
        static public bool IsValidIsraeliID(int id)
        {

            string idStr = id.ToString().PadLeft(9, '0');
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                int num = (idStr[i] - '0') * ((i % 2) + 1);
                sum += num > 9 ? num - 9 : num;
            }
            return sum % 10 == 0;
        }

        /// <summary>
        /// The assignment that volunteer handles
        /// </summary>
        /// <param name="idVol">id volunteer</param>
        /// <returns>DO.Assignment</returns>
        internal static DO.Assignment? GetCallInTreatment(int idVol)
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                var assignVol = s_dal.Assignment.ReadAll(a => a.VolunteerId == idVol);
                DO.Assignment? assignInTreatment = (from a in assignVol
                                                    where a.TypeOfTreatmentTermination == null
                                                    let call = s_dal.Call.Read(a.CallId)
                                                    where call != null && 
                                                          (CallManager.GetStatusCall(call) == BO.StatusCall.InTreatment ||
                                                           CallManager.GetStatusCall(call) == BO.StatusCall.InTreatmentAtRisk)
                                                    select a).FirstOrDefault();
                
                return assignInTreatment;
            }
        }
        /// <summary>
        /// Call status in treatment
        /// </summary>
        /// <param name="call">call to get status</param>
        /// <returns>StatusCallInProgress</returns>
        internal static BO.StatusCallInProgress GetCallInProgress(DO.Call call)
        {
            lock (AdminManager.BlMutex) //stage 7
                return call.MaxTimeFinishCall - AdminManager.Now > s_dal.Config.RiskRange ?
                BO.StatusCallInProgress.InTreatment : BO.StatusCallInProgress.InRiskTreatment;
        }
        /// <summary>
        /// Count of calls of a specific treatment termination type
        /// </summary>
        /// <param name="type">Type of treatment termination</param>
        /// <param name="assignVol">Volunteer's collection of assignments</param>
        /// <returns></returns>
        internal static int CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination type, IEnumerable<DO.Assignment> assignVol)
        {

            return (from a in assignVol
                    where a.TypeOfTreatmentTermination == type
                    select a).Count();
        }
        /// <summary>
        /// Function that converts BO.Volunteer to DO.Volunteer
        /// </summary>
        /// <param name="volunteer">BO.Volunteer</param>
        /// <param name="role">Volunteer role</param>
        /// <returns>DO.Volunteer</returns>
        internal static DO.Volunteer CreateDoVolunteer(BO.Volunteer volunteer, DO.Role? role = null)
        {
            //double[]? latlon = Tools.CalcCoordinates(volunteer.Address ?? null);
            IntegrityCheck(volunteer);
            DO.Volunteer doVolunteer = new(
                volunteer.Id,
                volunteer.Name,
                volunteer.Phone,
                volunteer.Email,
                role == null ? (DO.Role)volunteer.Role : (DO.Role)role,
                volunteer.Active,
                (DO.DistanceType)volunteer.DistanceType,
                null,
                null,
                volunteer.Password,
                volunteer.Address,
                //latlon?[0],
                //latlon?[1],
                //latlon == null ? null : volunteer.Address,

                volunteer.MaxDistanceForCall
                 );
            return doVolunteer;
        }

        private static readonly Random s_rand = new();
        private static int s_simulatorCounter = 0;

        internal static void VolunteerActivitySimulation()
        {
            Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";
            List<BO.VolunteerInList> activeVolunteers;
            lock (AdminManager.BlMutex) // שלב 7
                activeVolunteers = s_bl.Volunteer.GetVolunteersList(BO.VolunteerInListAttributes.Active, true, null).ToList();

            foreach (var volunteer in activeVolunteers)
            {
                if (volunteer.IDCallInHisCare == null)
                {
                    // הסתברות של 20% לבחור קריאה
                    if (s_rand.NextDouble() < 0.2)
                    {
                        List<BO.OpenCallInList> openCalls;
                        lock (AdminManager.BlMutex)
                            openCalls = s_bl.Call.OpenCallsListSelectedByVolunteer(volunteer.Id, null, null).ToList();

                        if (openCalls.Count != 0)
                        {
                            int callId = openCalls[s_rand.Next(openCalls.Count)].Id;

                            lock (AdminManager.BlMutex)
                                s_bl.Call.ChooseTreatmentCall(volunteer.Id, callId);
                        }
                    }
                }
                else
                {
                    DO.Call call;
                    DO.Volunteer vol;
                    DO.Assignment assignment;

                    lock (AdminManager.BlMutex)
                    {
                        call = s_dal.Call.Read(c => c.Id == volunteer.IDCallInHisCare)!;
                        vol = s_dal.Volunteer.Read(v => v.Id == volunteer.Id)!;
                        assignment = s_dal.Assignment.Read(a => a.VolunteerId == volunteer.Id && a.EndOfTreatmentTime == null)!;
                    }

                    // חישוב מרחק
                    double distance = Tools.CalcDistance(vol, call);

                    // זמן טיפול מינימלי נדרש לפי המרחק + 10 דקות אקראיות
                    double requiredMinutes = distance * 3 + s_rand.Next(7, 14);

                    if (assignment.EntryTimeForTreatment.AddMinutes(requiredMinutes) < AdminManager.Now)
                    {
                        // עבר מספיק זמן - לסיים טיפול
                        lock (AdminManager.BlMutex)
                            s_bl.Call.UpdateEndTreatmentOnCall(volunteer.Id, assignment.Id);
                    }
                    else
                    {
                        // הסתברות של 10% לבטל טיפול
                        if (s_rand.NextDouble() < 0.1)
                        {
                            lock (AdminManager.BlMutex)
                                s_bl.Call.UpdateCancelTreatmentOnCall(volunteer.Id, assignment.Id);
                        }
                    }
                }
            }
        }

        public static async Task UpdateCoordinatesForVolunteerAddressAsync(DO.Volunteer dovol)
        {
            if (dovol.Address is not null)
            {
                double[]? loc = await Tools.CalcCoordinates(dovol.Address);
                if (loc is not null)
                {
                    dovol = dovol with { Latitude = loc[0], Longitude = loc[1] };
                    lock (AdminManager.BlMutex)
                        s_dal.Volunteer.Update(dovol);
                    Observers.NotifyListUpdated();
                    Observers.NotifyItemUpdated(dovol.Id);
                }

            }
        }
    }
}
