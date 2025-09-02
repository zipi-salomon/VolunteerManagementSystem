namespace BO;
using Helpers;
public class ClosedCallInList
{/// <summary>
/// id:id call
/// CallType:call type
/// CallAddress:call address
/// OpeningTime: opening time of call
/// EntryTimeForTreatment:Entry Time For Treatment
/// EndOfTreatmentTime:End Of Treatment Time
/// TypeOfTreatmentTermination:Type Of Treatment Termination
/// </summary>
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public string CallAddress { get; set; } = null!;
    public DateTime OpeningTime { get; set; }
    public DateTime EntryTimeForTreatment { get; set;}
    public DateTime? EndOfTreatmentTime { get; set; }
    public TypeOfTreatmentTermination? TypeOfTreatmentTermination { get; set; }

    public override string ToString() => this.ToStringProperty();


}
