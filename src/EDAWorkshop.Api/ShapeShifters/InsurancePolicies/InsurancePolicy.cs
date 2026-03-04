namespace EDAWorkshop.Api.ShapeShifters.InsurancePolicies;

public class InsurancePolicy
{
    public Guid PolicyId { get; set; }
    public string? PolicyNumber { get; set; } = string.Empty;
    public string? ProductCode { get; set; } = string.Empty;
    public decimal? GrossPremium { get; set; }
}