namespace BO;
using Helpers;

public class Call
{/// <summary>
    /// ID: call id
    /// CallType: Call type 
    /// CallDescription: call description
    /// CallAddress: Call address
    /// Latitude: Call address latitude
    /// Longitude: Call address longitude
    /// OpeningTime: call start time
    /// MaxTimeFinishCall: Maximum time to finish call
    /// StatusCall: Call status
    /// CallAssignInList: Assignment list to call
    /// </summary>
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public string? CallDescription { get; set; }
    public string CallAddress { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime OpeningTime { get; init; }
    public DateTime? MaxTimeFinishCall {  get; set; }
    public StatusCall StatusCall { get; set; }
    public List<BO.CallAssignInList>? CallAssignInList { get; set; }

    public override string ToString() => this.ToStringProperty();

}
