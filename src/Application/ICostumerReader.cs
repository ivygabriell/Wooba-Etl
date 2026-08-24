using WoobaEtl.Domain;

namespace WoobaEtl.Application; 

public interface ICustomerReader
{
    IReadOnlyList<RawCustomer> ReadAll(string filePath);
}