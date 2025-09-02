using Helpers;
using BlApi;
using Helpers;
using System.Text;
using System;
using System.Diagnostics;
namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// From the logical object details, creates a new object of the data entity type DO.Call
    ///Attempts to request the addition of the new call to the data layer
    /// </summary>
    /// <param name="newBoCall">Object of the logical entity type "Call" BO.Call</param>
    public async Task AddCall(BO.Call newBoCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        if (newBoCall.MaxTimeFinishCall < AdminManager.Now || newBoCall.MaxTimeFinishCall < newBoCall.OpeningTime)
            throw new BO.BlInvalidValueException("OpeningTime value of call is not valid");
        DO.Call doCall = await CallManager.CreateDoCall(newBoCall, true);
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Create(doCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={newBoCall.Id} already exists", ex);
        }
        //_ = CallManager.UpdateCoordinatesForCallAddressAsync(doCall);

    }
    /// <summary>
    /// update call details
    /// </summary>
    /// <param name="call">Object of the logical entity type "Call" BO.Call</param>
    /// <exception cref="BO.BlInvalidValueException">Invalid value exception</exception>
    /// <exception cref="BO.BlDoesNotExistException">Call does not exist exception</exception>
    public async Task UpdateCallDetails(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        DO.Call doCall = await CallManager.CreateDoCall(call);

        try
        {
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(doCall.Id); //stage 5
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={call.Id} is not exists", ex);
        }
    }
    /// <summary>
    ///Choosing a call for treatment
    /// </summary>
    /// <param name="idVolunteer">id Volunteer who  s call.</param>
    /// <param name="idCall">id call for selection</param>
    /// <exception cref="BO.BlDoesNotExistException">Does Not Exist</exception>
    /// <exception cref="BO.BlInvalidRequestException">Invalid Request</exception>
    public void ChooseTreatmentCall(int idVolunteer, int idCall)
    {

        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        DO.Call call;
        DO.Volunteer vol;
        IEnumerable<DO.Assignment> assignments;

        lock (AdminManager.BlMutex)
        {
            call = _dal.Call.Read(idCall) ?? throw new BO.BlDoesNotExistException($"the call with id{idCall} does not exist");
            vol = _dal.Volunteer.Read(idVolunteer) ?? throw new BO.BlDoesNotExistException($"The volunteer with {idVolunteer} does not exist");
            assignments = _dal.Assignment.ReadAll(a => a.CallId == idCall).ToList();
        }

        if (assignments
            .Where(a => (a.TypeOfTreatmentTermination == DO.TypeOfTreatmentTermination.Handled)
                     || (a.TypeOfTreatmentTermination == null)
                     || (CallManager.GetStatusCall(call) == BO.StatusCall.Expired)
                       ).Any())
            throw new BO.BlInvalidRequestException("You cannot choose the reading.");

        if (vol.Active == false)
            throw new BO.BlUnauthorizedException("you are not active, you can not take a call");

        DO.Assignment newAssignment = new()
        {
            CallId = idCall,
            VolunteerId = idVolunteer,
            EntryTimeForTreatment = AdminManager.Now
        };

        try
        {
            lock (AdminManager.BlMutex)
                _dal.Assignment.Create(newAssignment);

            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(vol.Id);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlInvalidRequestException($"The call with id{idCall} cannot be taken", ex);
        }
    }

    /// <summary>
    /// The collection will include - all readings with the status "open" or "open at risk"
    /// </summary>
    /// <param name="idVolunteer">Volunteer ID - for whom the list of open calls for selection and
    /// their distance from their current distance is returned</param>
    /// <param name="filterByAttribute">The ENUM value of the call type by which the list will be filtered.</param>
    /// <param name="sortByAttribute">A parameter that is an ENUM value of a field in the "Open Read in List"
    /// entity, by which the list is sorted.</param>
    /// <returns>A sorted collection of a logical data entity "Open Calls in List" that includes the distance
    /// of each call from the volunteer</returns>
    public IEnumerable<BO.OpenCallInList> OpenCallsListSelectedByVolunteer(int idVolunteer, BO.CallType? filterByAttribute, BO.OpenCallInListAttributes? sortByAttribute)
    {
        IEnumerable<DO.Call> calls;
        DO.Volunteer vol;

        lock (AdminManager.BlMutex)
        {
            calls = _dal.Call.ReadAll();
            vol = _dal.Volunteer.Read(idVolunteer) ??
                throw new BO.BlDoesNotExistException($"The volunteer with id:{idVolunteer} does not exist");
        }

        var openCalls = (from c in calls
                         where (CallManager.GetStatusCall(c) == BO.StatusCall.Open ||
                                CallManager.GetStatusCall(c) == BO.StatusCall.OpenAtRisk) &&
                               vol.MaxDistanceForCall >= Tools.CalcDistance(vol, c)
                         select c).ToList();

        var openCallsList = openCalls.Select(c => new BO.OpenCallInList
        {
            Id = c.Id,
            CallType = (BO.CallType)c.CallType,
            CallDescription = c.CallDescription,
            CallAddress = c.CallAddress,
            OpeningTime = c.OpeningTime,
            MaxTimeFinishCall = c.MaxTimeFinishCall,
            CallingDistanceFromTreatingVolunteer = Tools.CalcDistance(vol, c)
        });

        openCallsList = CallManager.FilterAndSortCalls(openCallsList, filterByAttribute, sortByAttribute);

        return openCallsList;
    }
    /// <summary>
    /// Sorts and filters the calls by ID and by the attributes received as parameters.
    /// </summary>
    /// <param name="idVolunteer">ID volunteer</param>
    /// <param name="filterByAttribute">call filtering attribute</param>
    /// <param name="sortByAttribute">call sorting attribute</param>
    /// <returns>A sorted and filtered list of the volunteer</returns>
    public IEnumerable<BO.ClosedCallInList> ClosedCallsListHandledByVolunteer(int idVolunteer,
        BO.CallType? filterByAttribute = null, BO.ClosedCallInListAttributes? sortByAttribute = null)
    {
        IEnumerable<DO.Call> calls;
        IEnumerable<DO.Assignment> assignments;

        lock (AdminManager.BlMutex)
        {
            calls = _dal.Call.ReadAll();
            assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == idVolunteer).ToList();
        }

        var closeCalls = (from c in calls
                          from a in assignments
                          where c.Id == a.CallId && a.TypeOfTreatmentTermination is not null
                          select c).Distinct().ToList();

        var closeCallsInlist = closeCalls.Select(c =>
        {
            DO.Assignment assign;
            lock (AdminManager.BlMutex)
            {
                assign = _dal.Assignment.Read(a =>a.VolunteerId==idVolunteer&& c.Id == a.CallId && a.TypeOfTreatmentTermination is not null)!;
            }

            return new BO.ClosedCallInList
            {
                Id = c.Id,
                CallType = (BO.CallType)c.CallType,
                CallAddress = c.CallAddress,
                OpeningTime = c.OpeningTime,
                EntryTimeForTreatment = assign!.EntryTimeForTreatment,
                EndOfTreatmentTime = assign.EndOfTreatmentTime,
                TypeOfTreatmentTermination = (BO.TypeOfTreatmentTermination)assign.TypeOfTreatmentTermination!
            };
        });

        closeCallsInlist = CallManager.FilterAndSortCalls<BO.ClosedCallInList>(closeCallsInlist, filterByAttribute, sortByAttribute);
        return closeCallsInlist;
    }

    /// <summary>
    /// delete call
    /// </summary>
    /// <param name="idCall">call id</param>
    /// <exception cref="BO.BlCantDeleteException">if it is not possible to delete</exception>
    public void DeleteCall(int idCall)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex) //stage 7
        {
            DO.Call call = _dal.Call.Read(idCall) ?? throw new BO.BlDoesNotExistException($"Call with id {idCall} does not exist");

            if (CallManager.GetStatusCall(call) != BO.StatusCall.Open)
                throw new BO.BlCantDeleteException("It is not possible to delete a call that is not open.");

            if (_dal.Assignment.ReadAll(a => a.CallId == idCall).Any())
                throw new BO.BlCantDeleteException("It is not possible to delete a call handling assignment.");

            try
            {
                _dal.Call.Delete(idCall);
                CallManager.Observers.NotifyListUpdated(); //stage 5
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlCantDeleteException("It is not possible to delete the call", ex);
            }
        }
    }
    /// <summary>
    /// Requests the data layer to get details about the call and its allocation list.
    /// </summary>
    /// <param name="idCall">id call</param>
    /// <returns>List of logical entities of type "List Read Assignment"</returns>
    /// <exception cref="BO.BlDoesNotExistException">call does not exist</exception>
    public BO.Call GetCallDetails(int idCall)
    {
        DO.Call call;
        lock (AdminManager.BlMutex) //stage 7
        {
            call = _dal.Call.Read(idCall) ?? throw new BO.BlDoesNotExistException("call does not exist");
        }
        var assignments = _dal.Assignment.ReadAll(a => a.CallId == idCall);
        BO.Call newBOCall = new()
        {
            Id = call.Id,
            CallAddress = call.CallAddress,
            CallDescription = call.CallDescription,
            CallType = (BO.CallType)call.CallType,
            MaxTimeFinishCall = call.MaxTimeFinishCall,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpeningTime,
            StatusCall = CallManager.GetStatusCall(call),
            CallAssignInList = assignments.Any() ?
                                                     assignments.Select(a => new BO.CallAssignInList
                                                     {
                                                         VolunteerId = a.VolunteerId,
                                                         Name = _dal.Volunteer.Read(v => v.Id == a.VolunteerId)?.Name,
                                                         EntryTimeForTreatment = a.EntryTimeForTreatment,
                                                         EndOfTreatmentTime = a.EndOfTreatmentTime,
                                                         TypeOfTreatmentTermination = (BO.TypeOfTreatmentTermination?)a.TypeOfTreatmentTermination
                                                     }).ToList()
                                                        : null
        };
        return newBOCall;
    }

    /// <summary>
    /// In each cell in the array at index i, the number of calls whose status value is equal to i will be counted.
    /// </summary>
    /// <returns>Returns an array of quantities according to the call status</returns>
    public int[] GetCallQuantitiesByStatus()
    {
        int[] callCounts = new int[Enum.GetValues(typeof(BO.StatusCall)).Length];
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex) //stage 7
            calls = _dal.Call.ReadAll();
        var callsByStatus = calls.GroupBy(CallManager.GetStatusCall)
            .ToDictionary(group => (int)group.Key, group => group.Count());

        callCounts = callCounts
            .Select((value, index) => callsByStatus.ContainsKey(index) ? callsByStatus[index] : value).ToArray();
        return callCounts;
    }
    /// <summary>
    /// Returns a sorted and filtered collection of entities "call  in a list"
    /// </summary>
    /// <param name="filterByAttribute">A field in the "callInList" entity by which the list will be filtered</param>
    /// <param name="filterValue">Value to filter</param>
    /// <param name="sortByAttribute">a field in the "List Read" entity, by which the list is sorted</param>
    /// <returns>call list</returns>
    public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListAttributes? filterByAttribute = null, object? filterValue = null, BO.CallInListAttributes? sortByAttribute = null)
    {
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
        {
            calls = _dal.Call.ReadAll().ToList();
        }

        var callsInList = calls.Select(c =>
        {
            IEnumerable<DO.Assignment> allAssign;
            DO.Assignment? lastAssign;
            string? volunteerName;
            BO.StatusCall statusCall;

            lock (AdminManager.BlMutex)
            {
                allAssign = _dal.Assignment.ReadAll(a => a.CallId == c.Id).ToList();

                lastAssign = allAssign.FirstOrDefault(a => a.EndOfTreatmentTime == null) ??
                             allAssign.OrderByDescending(a => a.EndOfTreatmentTime).FirstOrDefault();

                volunteerName = _dal.Volunteer.Read(lastAssign?.VolunteerId ?? 0)?.Name;
                statusCall = CallManager.GetStatusCall(c);
            }

            return new BO.CallInList
            {
                Id = lastAssign?.Id ?? null,
                CallId = c.Id,
                CallType = (BO.CallType)c.CallType,
                OpeningTime = c.OpeningTime,
                TotalTimeRemainingFinishCalling = c.MaxTimeFinishCall - AdminManager.Now,
                LastVolunteerName = volunteerName,
                TotalTimeCompleteTreatment = (lastAssign?.TypeOfTreatmentTermination == DO.TypeOfTreatmentTermination.Handled)
                                                ? lastAssign?.EndOfTreatmentTime - c.OpeningTime
                                                : null,
                StatusCall = statusCall,
                TotalAssignments = allAssign.Count()
            };
        });

        var propertyFilter = filterByAttribute != null
            ? typeof(BO.CallInList).GetProperty(filterByAttribute.ToString()!)
            : null;

        callsInList = propertyFilter != null
            ? (from c in callsInList
               where propertyFilter.GetValue(c, null)?.ToString() == filterValue?.ToString()
               select c).ToList()
            : (from item in callsInList select item).ToList();

        var propertySort = sortByAttribute != null
            ? typeof(BO.CallInList).GetProperty(sortByAttribute.ToString()!)
            : null;

        callsInList = sortByAttribute != null
            ? (from c in callsInList
               orderby propertySort!.GetValue(c, null)
               select c).ToList()
            : (from c in callsInList
               orderby c.Id
               select c).ToList();

        return callsInList;
    }


    /// <summary>
    /// "Cancel Handling" update method on read
    /// </summary>
    /// <param name="id">ID of the person requesting the cancellation request</param>
    /// <param name="idCallAssign">assignment id</param>
    /// <exception cref="BlUnauthorizedException">No permission to update cancellation</exception>
    /// <exception cref="BlCantUpdateEception">Error during update</exception>
    public void UpdateCancelTreatmentOnCall(int idRequest, int idAssign)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        DO.Assignment assignment;
        DO.Call call;
        DO.Volunteer requester;

        lock (AdminManager.BlMutex)
        {
            assignment = _dal.Assignment.Read(idAssign)
                ?? throw new BO.BlDoesNotExistException($"Assignment with id:{idAssign} does not exist");

            call = _dal.Call.Read(assignment.CallId)!;

            requester = _dal.Volunteer.Read(idRequest)!;
        }

        if (!(requester.Role == DO.Role.Manager || assignment.VolunteerId == idRequest))
            throw new BO.BlUnauthorizedException("You are not allowed to update the call.");

        if (CallManager.GetStatusCall(call) != BO.StatusCall.InTreatment &&
            CallManager.GetStatusCall(call) != BO.StatusCall.InTreatmentAtRisk)
            throw new BO.BlCantUpdateException("the call is not at treatment");

        DO.TypeOfTreatmentTermination type = assignment.VolunteerId == idRequest
            ? DO.TypeOfTreatmentTermination.SelfCancellation
            : DO.TypeOfTreatmentTermination.CancelAdministrator;

        DO.Assignment newAssignment = CallManager.CreateDoAssignment(assignment, type);

        try
        {
            lock (AdminManager.BlMutex)
            {
                _dal.Assignment.Update(newAssignment);
            }

            CallManager.Observers.NotifyItemUpdated(newAssignment.Id);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlCantUpdateException("Unable to update the assignment", ex);
        }
    }


    /// <summary>
    /// "EndTreatment" update method on read
    /// </summary>
    /// <param name="id">ID of the person requesting the cancellation request</param>
    /// <param name="idCallAssign">assignment id</param>
    /// <exception cref="BlUnauthorizedException">No permission to update cancellation</exception>
    /// <exception cref="BlCantUpdateEception">Error during update</exception>
    public void UpdateEndTreatmentOnCall(int idVolunteer, int idAssign)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        DO.Assignment assignment;
        lock (AdminManager.BlMutex)
        {
            assignment = _dal.Assignment.Read(idAssign)
                ?? throw new BO.BlDoesNotExistException($"Assignment with id:{idAssign} does not exist");
        }

        if (assignment.EndOfTreatmentTime != null)
            throw new BO.BlCantUpdateException("The call is not in treatment");

        if (assignment.VolunteerId != idVolunteer)
            throw new BO.BlUnauthorizedException("You are not the one handling the call.");

        DO.Assignment newAssignment = CallManager.CreateDoAssignment(assignment, DO.TypeOfTreatmentTermination.Handled);

        try
        {
            lock (AdminManager.BlMutex)
            {
                _dal.Assignment.Update(newAssignment);
            }

            CallManager.Observers.NotifyItemUpdated(newAssignment.Id);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlCantUpdateException("Unable to update the assignment", ex);
        }
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

}