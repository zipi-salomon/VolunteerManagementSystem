namespace Dal;
using DalApi;

sealed internal class DalList : IDal
{
    private DalList() { }

    public static IDal Instance => Nested.instance;

    private static class Nested
    {
        internal static readonly IDal instance = new DalList();
    }

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// reset all entities
    /// </summary>
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}