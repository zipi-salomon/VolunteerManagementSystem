namespace BO;
using Helpers;
public class CallInList
{ /// <summary>
/// Id:id automatic
/// CallId: call id
/// CallType:call type
/// OpeningTime:opening time call
/// TotalTimeRemainingFinishCalling:Total Time Remaining Finish Calling
/// LastVolunteerName:Last Volunteer Name
/// TotalTimeCompleteTreatment:Total Time Complete Treatment
/// StatusCall: status call
/// TotalAssignments:Total Assignments of call
/// </summary>
    public int? Id { get; init; } = null;
    public int CallId { get; init; }
    public CallType CallType { get; set; }
    public DateTime OpeningTime { get; set; }
    public TimeSpan? TotalTimeRemainingFinishCalling {  get; set; }
    public string? LastVolunteerName { get; set; } = null;
    public TimeSpan? TotalTimeCompleteTreatment {  get; set; }
    public StatusCall StatusCall { get; set; }
    public int TotalAssignments { get; set; }

   public override string ToString() => this.ToStringProperty();
}
