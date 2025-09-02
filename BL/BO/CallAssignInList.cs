namespace BO;
using Helpers;

public class CallAssignInList
{/// <summary>
/// VolunteerId: volunteer id, automatic
/// Name: name volunteer
/// EntryTimeForTreatment: Entry time for treatment
///EndOfTreatmentTime:  End of treatment time
/// TypeOfTreatmentTermination: Type of treatment termination
/// </summary>
    public int? VolunteerId { get; init; }
    public string? Name { get; set; }
    public DateTime EntryTimeForTreatment { get; set; }
    public DateTime? EndOfTreatmentTime {  get; set; }
    public TypeOfTreatmentTermination? TypeOfTreatmentTermination {  get; set; }
    public override string ToString() => this.ToStringProperty();


}

