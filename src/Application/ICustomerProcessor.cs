using WoobaEtl.Domain;

namespace WoobaEtl.Application;

public interface ICustomerProcessor
{
    ProcessingResult Process(IReadOnlyList<RawCustomer> rawCustomers);
}