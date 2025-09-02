using BlApi;
using Helpers;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// From the logical object details, creates a new object of the data entity type DO.Voluteer
    ///Performs an attempt to request the addition of the new volunteer to the data layer(Create)
    /// </summary>
    /// <param name="newBoVolunteer">An object of the logical entity type "volunteer"</param>
    /// <exception cref="BO.BlAlreadyExistsException">There is a volunteer with this id}</exception>

    public void AddVolunteer(BO.Volunteer newBoVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(newBoVolunteer);
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Create(doVolunteer);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={newBoVolunteer.Id} already exists", ex);
        }
        // חישוב אסינכרוני של קואורדינטות ברקע    שלב 7
        _ = VolunteerManager.UpdateCoordinatesForVolunteerAddressAsync(doVolunteer);
    }
    /// <summary>
    /// Requests the record from the data layer and checks which fields have changed
    /// From the details of the logical object BO.Volunteer, creates an object of the data entity type DO.Volunteer
    ///Attempts to request an update of the volunteer in the data layer DO.Volunteer
    /// </summary>
    /// <param name="idVolunteer">ID of the person requesting the update</param>
    /// <param name="volunteer">An object of the logical entity type "volunteer" for update</param>
    /// <exception cref="BO.BlDoesNotExistException">The volunteer is not exist</exception>
    /// <exception cref="BO.BlUnauthorizedException"> There is no authorization</exception>
    /// <exception cref="BO.BlCantUpdateException">cant update volunteer details</exception>
    public void UpdateVolunteerDetails(int idRequester, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        DO.Volunteer updatedDoVolunteer;
        lock (AdminManager.BlMutex) //stage 7
        {
            DO.Volunteer doVolunteer = _dal.Volunteer.Read(volunteer.Id) ??
                throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteer.Id} does Not exist");

            DO.Volunteer requester = _dal.Volunteer.Read(idRequester)!;

            if (requester.Role != DO.Role.Manager && idRequester != volunteer.Id)
                throw new BO.BlUnauthorizedException("Only a manager can update the volunteer's role");
            updatedDoVolunteer = VolunteerManager.CreateDoVolunteer(volunteer,
             requester.Role == DO.Role.Manager ? (DO.Role)volunteer.Role : null);

            try
            {
                _dal.Volunteer.Update(updatedDoVolunteer);
                

            }

            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlCantUpdateException($"volunteer with ID={volunteer.Id} does not exists", ex);
            }

        }

        VolunteerManager.Observers.NotifyItemUpdated(updatedDoVolunteer.Id); //stage 5
        VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        CallManager.Observers.NotifyListUpdated(); //stage 5

        _ = VolunteerManager.UpdateCoordinatesForVolunteerAddressAsync(updatedDoVolunteer);
    }
    /// <summary>
    /// Requesting a request to the data layer to check if the volunteer can be deleted
    ///Attempting to request a deletion of the volunteer from the data layer
    /// </summary>
    /// <param name="idVolunteer">Volunteer ID</param>
    /// <exception cref="BO.BlCantDeleteException"></exception>
    public void DeleteVolunteer(int idVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); //stage 7
        lock (AdminManager.BlMutex) //stage 7
        {
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == idVolunteer);
            if (assignments.Any(a => a.TypeOfTreatmentTermination == DO.TypeOfTreatmentTermination.Handled || a.TypeOfTreatmentTermination == null))
                throw new BO.BlCantDeleteException("It is not possible to delete a volunteer handling calls or handled in past.");
            try
            {
                _dal.Volunteer.Delete(idVolunteer);


            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlCantDeleteException("It is not possible to delete the volunteer", ex);
            }
        }
        VolunteerManager.Observers.NotifyListUpdated(); //stage 5
    }

    /// <summary>
    /// Please refer to the data layer (Read) to obtain details about the volunteer and the read he/she is handling (if any).
    /// </summary>
    /// <param name="idVolunteer">volunteer id</param>
    /// <returns>An object of the logical entity type "Volunteer" (BO.Volunteer)
    ///which includes an object of the logical entity type "Volunteer Care Call"</returns>
    /// <exception cref="BO.BlDoesNotExistException"> the volunteer not exist</exception>
    public BO.Volunteer GetVolunteerDetails(int idVolunteer)
    {
        lock (AdminManager.BlMutex) //stage 7
        {

            DO.Volunteer vol = _dal.Volunteer.Read(idVolunteer) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID {idVolunteer} is not found in database.");
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == idVolunteer);
            DO.Assignment? assignInTreatment = VolunteerManager.GetCallInTreatment(idVolunteer);
            var call = AssignmentManager.GetCallByAssignment(assignInTreatment);
            var volunteerBO = new BO.Volunteer
            {
                Id = vol.Id,
                Name = vol.Name,
                Phone = vol.Phone,
                Email = vol.Email,
                Password = vol.Password,
                Address = vol.Address,
                Latitude = vol.Latitude,
                Longitude = vol.Longitude,
                Role = (BO.Role)vol.Role,
                Active = vol.Active,
                MaxDistanceForCall = vol.MaxDistanceForCall,
                DistanceType = (BO.DistanceType)vol.DistanceType,
                TotalCallsHandled = VolunteerManager.CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination.Handled, assignments),
                TotalCallsCanceled = VolunteerManager.CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination.SelfCancellation, assignments),
                TotalCallsChoseHandleHaveExpired = VolunteerManager.CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination.CancellationExpired, assignments),
                CallingVolunteerTherapy = assignInTreatment != null ? new BO.CallInProgress
                {
                    Id = assignInTreatment.Id,
                    CallId = assignInTreatment.CallId,
                    CallType = (BO.CallType)call!.CallType,
                    CallDescription = call.CallDescription,
                    CallAddress = call.CallAddress,
                    OpeningTime = call.OpeningTime,
                    MaxTimeFinishCall = call.MaxTimeFinishCall,
                    EntryTimeForTreatment = assignInTreatment.EntryTimeForTreatment,
                    CallingDistanceFromTreatingVolunteer = Tools.CalcDistance(vol, call),
                    StatusCalling = VolunteerManager.GetCallInProgress(call),
                } : null,

            };

            return volunteerBO;
        }
    }
    /// <summary>
    /// login and return the role
    /// </summary>
    /// <param name="username">user name</param>
    /// <returns>the role of volunteer</returns>
    /// <exception cref="BO.BlDoesNotExistException">the volunteer does not exist</exception>
    public BO.Role Login(int id, string password)
    {
        lock (AdminManager.BlMutex)
        { //stage 7
            DO.Volunteer vol = _dal.Volunteer.Read(vol => vol.Id == id && vol.Password == password) ??
        throw new BO.BlDoesNotExistException($"Volunteer with Id {id} and password {password} does Not exist");

            return (BO.Role)vol.Role;
        }
    }
    /// <summary>
    /// Returns a sorted and filtered collection of entities "volunteerInList"
    /// </summary>
    /// <param name="filterByAttribute">A field in the "volunteerInList" entity by which the list will be filtered</param>
    /// <param name="filterValue">Value to filter</param>
    /// <param name="sortByAttribute">a field in the "List Read" entity, by which the list is sorted</param>
    /// <returns>volunteerInList list</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(BO.VolunteerInListAttributes? filterByAttribute = null, object? filterValue = null, BO.VolunteerInListAttributes? sortByAttribute = null)
    {
        IEnumerable<DO.Volunteer> volunteers;
        lock (AdminManager.BlMutex) //stage 7
            volunteers = _dal.Volunteer.ReadAll();

        var volsInList = volunteers.Select(v =>
        {
            IEnumerable<DO.Assignment> assignVol;
            DO.Assignment? assignInTreatment;
            DO.Call? call;
            lock (AdminManager.BlMutex) //stage 7
            {
                assignVol = _dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);
                assignInTreatment = VolunteerManager.GetCallInTreatment(v.Id);
                call = AssignmentManager.GetCallByAssignment(assignInTreatment);
            }
            return new BO.VolunteerInList
            {
                Id = v.Id,
                Name = v.Name,
                Active = v.Active,
                TotalCallsHandledByVolunteer = VolunteerManager.CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination.Handled, assignVol),
                TotalCallsCanceledByVolunteer = VolunteerManager.CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination.SelfCancellation, assignVol)
                + VolunteerManager.CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination.CancelAdministrator, assignVol),
                TotalExpiredCallingsByVolunteer = VolunteerManager.CountTypeOfTreatmentTermination(DO.TypeOfTreatmentTermination.CancellationExpired, assignVol),
                IDCallInHisCare = call?.Id,
                CallType = (BO.CallType?)call?.CallType ?? BO.CallType.None
            };
        });



        var propertyFilter = filterByAttribute != null ? typeof(BO.VolunteerInList).GetProperty(filterByAttribute.ToString()!) : null;

        volsInList = propertyFilter != null ?
                (from v in volsInList
                 where propertyFilter.GetValue(v, null)?.ToString() == filterValue?.ToString()
                 select v).ToList()
                :
                volsInList.ToList();


        var propertySort = sortByAttribute != null ? typeof(BO.VolunteerInList).GetProperty(sortByAttribute.ToString()!) : null;

        volsInList = sortByAttribute != null ?
                (from v in volsInList
                 orderby propertySort!.GetValue(v, null)
                 select v).ToList()
                :
                (from v in volsInList
                 orderby v.Id
                 select v).ToList();

        return volsInList;
    }


    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


}