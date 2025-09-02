namespace BlApi
{
    public interface ICall: IObservable
    {
        /// <summary>
        ///Return an array of quantities by call status
        ///(In each cell in the array at index i, the quantity of calls whose status value is equal to i will be returned)
        ///Must use the GroupBy extension function or linq group by queries
        /// </summary>
        /// <returns>array of quantities by call status</returns>
        int[] GetCallQuantitiesByStatus();
        /// <summary>
        /// </summary>
        /// <param name="filterByAttribute">ENUM value of a field in the "List Read" entity, by which the list will be filtered</param>
        /// <param name="filterValue">Value to filter by any type</param>
        /// <param name="sortByAttribute">ENUM value of a field in the "List Read" entity, by which the list is sorted</param>
        /// <returns>Sorted and filtered collection of logical data entity</returns>
        IEnumerable<BO.CallInList> GetCallsList(BO.CallInListAttributes? filterByAttribute, object? filterValue, BO.CallInListAttributes? sortByAttribute);//null בכותרת המימוש
        /// <summary>
        /// Requests the data layer (Read) to obtain details about the call and its list of assignments (if any)
        ///From the details received, creates an object of the logical entity type "Volunteer" (BO.Call)
        ///which includes a list of logical entities of the type "Listed Call Assignment"
        /// </summary>
        /// <param name="idCall">Call ID</param>
        /// <returns>The object she built</returns>
        BO.Call GetCallDetails(int idCall);
        /// <summary>
        ///call update in data layer
        /// </summary>
        /// <param name="call">Object of the logical entity type "Call" BO.Call</param>
        Task UpdateCallDetails(BO.Call call);
        /// <summary>
        /// call deletion request
        /// </summary>
        /// <param name="idCall">Call ID</param>
        void DeleteCall(int idCall);
        /// <summary>
        /// From the logical object details, creates a new object of the data entity type DO.Call
        ///Attempts to request the addition of the new call to the data layer
        /// </summary>
        /// <param name="newBoCall">Object of the logical entity type "Call" BO.Call</param>
        Task AddCall(BO.Call newBoCall);
        /// <summary>
        /// Returns a list filtered by the ID of that volunteer - all closed calls for that volunteer
        ///Calls closed for any type of treatment termination
        /// </summary>
        /// <param name="idVolunteer">Volunteer's ID</param>
        /// <param name="filterByAttribute">The ENUM value of the call type by which the list will be filtered.</param>
        /// <param name="sortByAttribute">ENUM value of a field in the "Closed Read List" entity, by which the list is sorted</param>
        /// <returns>Sorted collection of logical data entity</returns>
        IEnumerable<BO.ClosedCallInList> ClosedCallsListHandledByVolunteer(int idVolunteer,BO.CallType? filterByAttribute, BO.ClosedCallInListAttributes? sortByAttribute);
        /// <summary>
        /// The collection will include - all readings with the status "open" or "open at risk"
        /// </summary>
        /// <param name="idVolunteer">Volunteer ID - for whom the list of open calls for selection and their distance from their current distance is returned</param>
        /// <param name="filterByAttribute">The ENUM value of the call type by which the list will be filtered.</param>
        /// <param name="sortByAttribute">A parameter that is an ENUM value of a field in the "Open Read in List" entity, by which the list is sorted.</param>
        /// <returns>A sorted collection of a logical data entity "Open Reads in List" that includes the distance of each read from the volunteer</returns>
        IEnumerable<BO.OpenCallInList> OpenCallsListSelectedByVolunteer(int idVolunteer, BO.CallType? filterByAttribute, BO.OpenCallInListAttributes? sortByAttribute);//null בכותרת המימוש
        /// <summary>
        /// Makes requests to the data layer to fetch the data the method needs
        ///Checks for permission to terminate
        ///Checks that the allocation is open
        ///Attempts to request an update of the corresponding allocation entity from the data layer
        /// </summary>
        /// <param name="idVolunteer">Volunteer ID</param>
        /// <param name="idCallAssign">The assignment number of the call on which he wants to report the end of treatment</param>
        void UpdateEndTreatmentOnCall(int idVolunteer, int idAssign);
        /// <summary>
        /// Makes requests to the data layer to fetch the data the method needs
        ///Checks for cancellation permission
        ///Checks that the allocation has not yet been processed
        ///Attempts to request an update of the corresponding allocation entity from the data layer
        ///"Actual processing completion time" will be updated according to the system clock
        /// </summary>
        /// <param name="id">ID of the person requesting the cancellation request</param>
        /// <param name="idCallAssign">The allocation number of the currently processing call that he wants to cancel.</param>
        void UpdateCancelTreatmentOnCall(int idRequest, int idAssign);
        /// <summary>
        /// Makes requests to the data layer to fetch the data the method needs
        ///Checks that the request is valid
        ///Attempts to create a new assignment entity in the data layer with the call ID and the volunteer ID
        /// </summary>
        /// <param name="idVolunteer">Volunteer ID</param>
        /// <param name="idCall">The number of the call he wants to handle</param>
        void ChooseTreatmentCall(int idVolunteer,int idCall);
    }
}
