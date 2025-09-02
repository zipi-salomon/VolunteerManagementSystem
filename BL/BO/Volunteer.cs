namespace BO;
using Helpers;
public class Volunteer
{
    /// <summary>
    /// Id:id volunteer
    /// Name: volunteer name
    /// Phone:volunteer phone
    /// Email:voluteer email
    /// Address:volunteer address
    /// latitude: Volunteer address latitude
    /// Longitude: Volunteer address longitude
    /// Role: role of volunteer
    /// Active: if the volunteer is active
    /// MaxDistanceForCall: max distance for call
    /// DistanceType: Distance type
    /// TotalCallsHandled: Total calls handled
    /// TotalCallsCanceled: Total calls canceled
    /// TotalCallsChoseHandleHaveExpired: Total calls chose handle have expired
    /// CallingVolunteerTherapy: Calling volunteer therapy
    /// </summary>
    public int Id { get; init; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Role Role { get; set; }
    public bool Active { get; set; }
    public double? MaxDistanceForCall { get; set; }
    public DistanceType DistanceType { get; set; }  =DistanceType.Aerial;
    public int TotalCallsHandled { get; set; }
    public int TotalCallsCanceled { get; set; }
    public int TotalCallsChoseHandleHaveExpired { get; set; }
    public BO.CallInProgress? CallingVolunteerTherapy { get; set; }

    public override string ToString() => this.ToStringProperty();


}
