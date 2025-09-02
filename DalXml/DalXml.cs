using DalApi;
namespace Dal;

sealed internal class DalXml : IDal
{
    private DalXml() { }

    public static IDal Instance => Nested.instance;

    private static class Nested
    {
        internal static readonly IDal instance = new DalXml();
    }

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Assignment.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}