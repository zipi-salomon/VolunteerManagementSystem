namespace BO;
using Helpers;
public class OpenCallInList
{
    /// <summary>
    /// CallID: call id 
    /// CallType:call type
    /// CallDescription:  Call description
    /// CallAddress :  Call address
    /// CallOpeningTime: Call opening time
    /// MaxTimeFinishCall: Max Time Finish Call
    /// CallingDistanceFromTreatingVolunteer:Calling Distance From Treating Volunteer
    /// </summary>
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public string? CallDescription { get; set; }
    public string CallAddress { get; set; } = null!;
    public DateTime OpeningTime { get; set; }
    public DateTime? MaxTimeFinishCall { get; set; }
    public double CallingDistanceFromTreatingVolunteer { get; set; }
    public override string ToString() => this.ToStringProperty();

}
