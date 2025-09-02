namespace BlApi
{
    public interface IVolunteer: IObservable
    {        
        BO.Volunteer GetVolunteerDetails (int idVolunteer);
        BO.Role Login(int id,string password);
        void UpdateVolunteerDetails(int idRequester, BO.Volunteer volunteer);
        void DeleteVolunteer(int idVolunteer);
        void AddVolunteer(BO.Volunteer newBoVolunteer);
        IEnumerable<BO.VolunteerInList> GetVolunteersList(BO.VolunteerInListAttributes? filterByAttribute, object? filterValue, BO.VolunteerInListAttributes? sortByAttribute);//null בכותרת המימוש

    }
}
