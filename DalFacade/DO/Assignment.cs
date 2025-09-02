using System.Reflection.Metadata;
namespace DO;
/// <summary>
/// Assignment Entity represents a assignment with all its props
/// </summary>
/// <param name="Id">Represents a number that uniquely identifies the allocation entity</param>
/// <param name="CallId">Represents a number that identifies the call that the volunteer chose to handle</param>
/// <param name="VolunteerId">represents the ID of the volunteer who chose to take care of the reading</param>
/// <param name="EntryTimeForTreatment">Time when the current call entered processing.</param>
/// <param name="EndOfTreatmentTime">Time when the current volunteer finished handling the current call.</param>
/// <param name="TypeOfTreatmentTermination">The manner in which the treatment of the 
/// current reading was completed by the current volunteer. Handled, self cancel, admin cancel, cancel expired</param>

public record Assignment
(
int Id,
int CallId,
int VolunteerId,
DateTime EntryTimeForTreatment,
TypeOfTreatmentTermination ? TypeOfTreatmentTermination= null,
DateTime? EndOfTreatmentTime = null
)
{
    public Assignment() : this(0, 0, 0, DateTime.Now) { }
}

