namespace EDAWorkshop.Api.ShapeShifters.InsurancePolicies;

public class InsurancePolicyEventsHandler
{
    private readonly Dictionary<Guid, InsurancePolicy> _store = [];

    public Task Handle(PolicyInformationEvent message)
    {
        if (_store.TryGetValue(message.PolicyId, out var policy))
        {
            throw new NotImplementedException("Updating existing policies is not implemented yet. ");
        }
        _store[message.PolicyId] = new InsurancePolicy
        {
            PolicyId = message.PolicyId,
            PolicyNumber = message.PolicyNumber,
            ProductCode = message.ProductCode,
            GrossPremium = message.GrossPremium
        };

        return Task.CompletedTask;
    }

    public Task Handle(PolicyCreated message)
    {
        if (_store.TryGetValue(message.PolicyId, out var policy))
        {
            policy.PolicyNumber = message.PolicyNumber;
            policy.ProductCode = message.ProductCode;
            return Task.CompletedTask;
        }
        
        _store[message.PolicyId] = new InsurancePolicy
        {
            PolicyId = message.PolicyId,
            PolicyNumber = message.PolicyNumber,
            ProductCode = message.ProductCode
        };

        return Task.CompletedTask;
    }
    public Task Handle(PremiumCalculated message)
    {
        if (_store.TryGetValue(message.PolicyId, out var policy))
        {
            policy.GrossPremium = message.GrossPremium;
            return Task.CompletedTask;
        } 

        _store[message.PolicyId] = new InsurancePolicy
        {
            PolicyId = message.PolicyId,
            GrossPremium = message.GrossPremium
        };
        return Task.CompletedTask;
    }
}