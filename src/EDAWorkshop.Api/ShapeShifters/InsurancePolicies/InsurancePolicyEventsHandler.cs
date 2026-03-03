namespace EDAWorkshop.Api.ShapeShifters.InsurancePolicies;

public class InsurancePolicyEventsHandler
{
    private readonly Dictionary<Guid, InsurancePolicy> _store = [];

    public Task Handle(PolicyInformationEvent message)
    {
        _store[message.PolicyId] = new InsurancePolicy
        {
            PolicyId = message.PolicyId,
            PolicyNumber = message.PolicyNumber,
            ProductCode = message.ProductCode,
            GrossPremium = message.GrossPremium
        };

        return Task.CompletedTask;
    }
}