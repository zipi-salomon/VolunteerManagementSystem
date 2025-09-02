namespace BO;
using Helpers;
public class VolunteerInList
{
    /// <summary>
    /// ID: volunteer id
    /// Name: name of volunteer
    /// Active: if volunteer is active
    /// TotalCallsHandledByVolunteer: Total calls handled by volunteer
    /// TotalCallsCanceledByVolunteer: Total calls canceled by volunteer
    /// TotalExpiredCallingsByVolunteer: Total calls expired by volunteer
    /// IDCallInHisCare: id call by that volunteer care
    /// CallType: call type
    /// </summary>
    public int Id {  get; init; }
    public string Name { get; set; } = null!;
    public bool Active { get; set; }
    public int TotalCallsHandledByVolunteer { get; set; }
    public int TotalCallsCanceledByVolunteer { get; set; }
    public int TotalExpiredCallingsByVolunteer { get; set; }
    public int? IDCallInHisCare { get; set; }
    public CallType CallType { get; set; }

    public override string ToString() =>this.ToStringProperty();

}
