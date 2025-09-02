namespace DO;
/// <summary>
/// Volunteer Entity represents a volunteer with all its props
/// </summary>
/// <param name="Id">Personal unique ID of the volunteer (as in national id card).</param>
/// <param name="Name">Volunteer's full name (first name and last name).</param>
/// <param name="Phone">Volunteer's phone number.</param>
/// <param name="Email">Volunteer's unique email.</param>
/// <param name="Password"></param>
/// <param name="Address">Full and real address in correct format, of the volunteer. where it is 
/// currently located and available to receive calls that are at the distance defined for it.</param>
/// <param name="Latitude">A number indicating how far a point on Earth is south or north of the equator.</param>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator.</param>
/// <param name="Role">"Manager" or "Volunteer"</param>
/// <param name="Active">Is the volunteer active or inactive (retired from the organization). A retired 
/// volunteer also has his call history saved for him, but he cannot handle new calls.</param>
/// <param name="MaxDistanceForCall">Maximum distance for call</param>
/// <param name="DistanceType">Aerial distance, walking distance, driving distance
///The default is air distance.</param>
public record Volunteer
(
    int Id,
    string Name,
    string Phone,
    string Email,
    Role Role,
    bool Active,
    DistanceType DistanceType,
    double? Latitude=null,
    double? Longitude = null,
    string? Password=null,
    string? Address = null,
    double? MaxDistanceForCall = null
)
{
 public Volunteer() : this(0, "", "","", Role.Volunteer,false,DistanceType.Aerial) { }
}


