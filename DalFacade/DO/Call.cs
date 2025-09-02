namespace DO;
/// <summary>
///  Call Entity represents a call with all its props
/// </summary>
/// <param name="Id"> Qnique ID number of the call</param>
/// <param name="CallType">The type of call according to the type of specific system.</param>
/// <param name="CallDescription">Description of the Call. Detailed details on the Call.</param>
/// <param name="CallAddress">Full and real address in correct format, of the Call location.</param>
/// <param name="Latitude">A number that indicates how far a point on Earth is south or north of the equator.</param>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator.</param>
/// <param name="OpeningTime">Time (date and time) when the call was opened by the manager.</param>
/// <param name="MaxTimeFinishCall">Time (date and time) by which the reading should be closed.</param>
public record Call
(
    int Id,
    CallType CallType,
    string CallAddress,
    double Latitude,
    double Longitude,
    DateTime OpeningTime,
    string? CallDescription = null,
    DateTime? MaxTimeFinishCall=null
)
{
    public Call() : this(0,CallType.HouseCleaning,"", 0,0, DateTime.Now) { }
}


