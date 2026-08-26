using WoobaEtl.Domain;

// teste

namespace WoobaEtl.Application;

public interface ICustomerRepository
{
    void Insert(Customer customer);
    IReadOnlyList<Customer> GetAll();
    bool Update(string email, string newCity);
    bool Delete(string email);
}