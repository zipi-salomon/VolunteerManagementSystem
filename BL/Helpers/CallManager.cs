using DalApi;
using System;
namespace Helpers {

    internal static class CallManager
    {
        internal static ObserverManager Observers = new(); //stage 5
        private static IDal s_dal = Factory.Get; //stage 4
        /// <summary>
        /// A function that updates assignments that expired of their calls so that the termination type is expired
        /// </summary>
        //internal static void PeriodicCallsUpdates(DateTime oldClock,DateTime newClock)
        //{
        //    var calls = s_dal.Call.ReadAll().Where(c => c.MaxTimeFinishCall < newClock
        //           && GetStatusCall(c) != BO.StatusCall.Closed && GetStatusCall(c) != BO.StatusCall.Expired)
        //          .Select(
        //        call =>
        //        {
        //            var assin = (from a in s_dal.Assignment.ReadAll(a => a.CallId == call.Id)
        //                         where a.TypeOfTreatmentTermination is null
        //                         select a).FirstOrDefault();
        //            DO.Assignment newDoAssign;
        //            if (assin is not null)
        //            {
        //                s_dal.Assignment.Update(CreateDoAssignment(assin, DO.TypeOfTreatmentTermination.CancellationExpired));
        //                Observers.NotifyItemUpdated(assin.Id);
        //                Observers.NotifyListUpdated();
        //            }
        //            else
        //            {
        //                newDoAssign = new(
        //                 0,
        //                 call.Id,
        //                 0,
        //                 newClock,
        //                 DO.TypeOfTreatmentTermination.CancellationExpired,
        //                 newClock);
        //                s_dal.Assignment.Create(newDoAssign);
        //            }
        //            return 1;
        //        }
        //        );
        //}
        internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
        {
            var calls = new List<DO.Call>();
            lock (AdminManager.BlMutex)
            {
                calls = s_dal.Call.ReadAll()
                    .Where(c => c.MaxTimeFinishCall < newClock &&
                                GetStatusCall(c) != BO.StatusCall.Closed &&
                                GetStatusCall(c) != BO.StatusCall.Expired)
                    .ToList();
            }

            calls.Select(call =>
            {
                DO.Assignment? assin = null;
                bool updated = false;
                int? updatedId = null;

                lock (AdminManager.BlMutex)
                {
                    assin = s_dal.Assignment
                        .ReadAll(a => a.CallId == call.Id)
                        .FirstOrDefault(a => a.TypeOfTreatmentTermination is null);

                    if (assin is not null)
                    {
                        s_dal.Assignment.Update(CreateDoAssignment(assin, DO.TypeOfTreatmentTermination.CancellationExpired));
                        updated = true;
                        updatedId = assin.Id;
                    }
                    else
                    {
                        DO.Assignment newDoAssign = new(
                            0,
                            call.Id,
                            0,
                            newClock,
                            DO.TypeOfTreatmentTermination.CancellationExpired,
                            newClock);

                        s_dal.Assignment.Create(newDoAssign);
                        updated = false;
                    }
                }

                if (updated && updatedId.HasValue)
                {
                    Observers.NotifyItemUpdated(updatedId.Value);
                    Observers.NotifyListUpdated();
                }

                return 1;
            }).ToList();
        }

        /// <summary>
        /// A function that finds and returns the status of a specific call.
        /// </summary>
        /// <param name="call">The call whose status is of interest</param>
        /// <returns>status call</returns>
        internal static BO.StatusCall GetStatusCall(DO.Call call)
        {
            IEnumerable<DO.Assignment> assignmentsCall;
            DateTime now = AdminManager.Now;
            lock (AdminManager.BlMutex) //stage 7
            {
                assignmentsCall = s_dal.Assignment.ReadAll(assignment => assignment.CallId == call.Id);
            }
            if (now > call.MaxTimeFinishCall &&(!assignmentsCall.Any()|| assignmentsCall.Any(a => a.TypeOfTreatmentTermination != DO.TypeOfTreatmentTermination.Handled)))
                return BO.StatusCall.Expired;//Expired
            if (assignmentsCall.Any(a => a.TypeOfTreatmentTermination == DO.TypeOfTreatmentTermination.Handled))
                return BO.StatusCall.Closed;//Closed
            if (assignmentsCall.Where(a => a.TypeOfTreatmentTermination == null).Any())
                if (now.Add(s_dal.Config.RiskRange) > call.MaxTimeFinishCall)
                    return BO.StatusCall.InTreatmentAtRisk;//InTreatmentAtRisk
                else return BO.StatusCall.InTreatment;//InTreatment
            if (now.Add(s_dal.Config.RiskRange) > call.MaxTimeFinishCall)
                return BO.StatusCall.OpenAtRisk;//OpenAtRisk
            else return BO.StatusCall.Open;//Open
        }
        /// <summary>
        /// A method that get a call and checks its MaxTimeFinishCall.
        /// </summary>
        /// <param name="call">Call for valid check</param>
        /// <exception cref="BO.BlInvalidValueException">the max time to finish call is invalid</exception>
        internal static void ValidCall(BO.Call call)
        {

            if (call.MaxTimeFinishCall < call.OpeningTime)
            {
                throw new BO.BlInvalidValueException("the finish-time cant be earlier than the opening time");
            }


        }
        /// <summary>
        /// A function that converts a BO call to a DO call.
        /// </summary>
        /// <param name="call">BO.Call type call</param>
        /// <param name="add">Boolean parameter if the object to be added is true or updated is false</param>
        /// <returns>DO.Call type call</returns>
        /// <exception cref="BO.BlInvalidValueException">address is invalid</exception>
        internal static async Task<DO.Call> CreateDoCall(BO.Call call, bool add = false)
        {
            double[]? latlon = await Tools.CalcCoordinates(call.CallAddress)
                ?? throw new BO.BlInvalidValueException("invalid address");

            ValidCall(call);

            DateTime openingTime = add ? AdminManager.Now : call.OpeningTime;

            DO.Call doCall = new(
                call.Id,
                (DO.CallType)call.CallType,
                call.CallAddress,
                latlon[0],
                latlon[1],
                openingTime,
                call.CallDescription,
                call.MaxTimeFinishCall
            );

            return doCall;
        }

        /// <summary>
        /// A function that sorts and filters a call list
        /// </summary>
        /// <param name="calls">List of BO type calls for sorting and filtering</param>
        /// <param name="filterByAttribute">CallType attribute</param>
        /// <param name="sortByAttributeObj">Attribute</param>
        /// <returns>filtered and sort list call</returns>
        internal static IEnumerable<T> FilterAndSortCalls<T>(IEnumerable<T> calls, BO.CallType? filterValue,
                                                                object? sortByAttributeObj)
        {
            if (filterValue != null)
            {

                var callTypeProperty = typeof(T).GetProperty("CallType");

                calls = calls
                    .Where(item => callTypeProperty!.GetValue(item)?.Equals((BO.CallType)filterValue!) == true)
                    .ToList();
            }

            if (sortByAttributeObj != null)
            {
                var propertySort = sortByAttributeObj != null ? typeof(T).GetProperty(sortByAttributeObj.ToString()!) : null;

                //var propertySort = sortByAttributeObj?.GetType().GetProperty(sortByAttributeObj.ToString()!);
                calls = propertySort != null ?
                    (from c in calls
                     orderby propertySort.GetValue(c, null)
                     select c).ToList()
                    :
                    (from c in calls
                     orderby typeof(T).GetProperty("Id")!.GetValue(c, null)
                     select c).ToList();

            }
            return calls;
        }
        /// <summary>
        /// A function that converts a BO call to a DO call.
        /// </summary>
        /// <param name="assignment">BO Assignment</param>
        /// <param name="type">DO Assignment</param>
        /// <returns>DO assignment</returns>
        internal static DO.Assignment CreateDoAssignment(DO.Assignment assignment, DO.TypeOfTreatmentTermination type)
        {
            DateTime? endTime = type == DO.TypeOfTreatmentTermination.CancellationExpired ? null : AdminManager.Now;
            return new(
                assignment.Id,
                assignment.CallId,
                assignment.VolunteerId,
                assignment.EntryTimeForTreatment,
                type,
                endTime
                );
        }

        public static async Task UpdateCoordinatesForCallAddressAsync(DO.Call doCall)
        {
            if (doCall.CallAddress is not null)
            {
                double[]? loc = await Tools.CalcCoordinates(doCall.CallAddress);
                if (loc is not null)
                {
                    doCall = doCall with { Latitude = loc[0], Longitude = loc[1] };
                    lock (AdminManager.BlMutex)
                        s_dal.Call.Update(doCall);
                    Observers.NotifyListUpdated();
                    Observers.NotifyItemUpdated(doCall.Id);
                }

            }
        }
    }
}