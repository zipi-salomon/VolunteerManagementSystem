namespace BO;
using Helpers;

public class CallInProgress
{
    /// <summary>
    /// Id:  id call in progress, automatic
    /// CallId: call id
    /// CallType: call type
    /// CallDescription: call description
    /// CallAddress: call address
    /// OpeningTime: opening time of call
    /// MaxTimeFinishCall: Max time finish call
    /// EntryTimeForTreatment: entry time for treatment
    /// CallingDistanceFromTreatingVolunteer: Calling distance from treating volunteer
    /// StatusCalling: call status
    /// </summary>
    public int Id {  get; init; }
    public int CallId {  get; init; }
    public CallType CallType { get; set; }
    public string? CallDescription{ get; set; }
    public string CallAddress { get; set; } = null!;
    public DateTime OpeningTime { get; set; }
    public DateTime? MaxTimeFinishCall {  get; set; }
    public DateTime EntryTimeForTreatment { get; set; }//maybee init??
    public double CallingDistanceFromTreatingVolunteer { get; set; }
    public StatusCallInProgress StatusCalling { get; set; }

    public override string ToString() => this.ToStringProperty();
}
