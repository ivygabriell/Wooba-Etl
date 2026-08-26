namespace WoobaEtl.Domain;


// Entidade de dominio. Valida apenas regras de um cliente isolado.
public class Customer
{
    public string Name { get; }
    public string Email { get; }
    public DateTime BirthDate { get; }
    public string PhoneNumber { get; }
    public string City { get; }
    public string StateAbbreviation { get; }

    
    // Garantia que o objeto só nasça válido e não posso ser corrompido depois 

    public static bool TryCreate(
        string name,
        string email,
        DateTime birthDate,
        string phoneNumber,
        string city,
        string stateAbbreviation,
        out Customer? customer,
        out string? error)
    {
        customer = null;
        error = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "nome vazio";
            return false;    
        }
        
        if ( email.Count(c => c == '@') != 1)
        {
            error = "e-mail inválido";
            return false;
        }
        
        if (birthDate > DateTime.Today)
        {
            error = "data de nascimento no futuro";
            return false;
        }

        customer = new Customer (name, email, birthDate, phoneNumber, city, stateAbbreviation);
        return true;

    }

    private Customer(
        string name,
        string email,
        DateTime birthDate,
        string phone,
        string city,
        string state)
    {
        Name = name;
        Email = email;
        BirthDate = birthDate;
        PhoneNumber = phone;
        City = city;
        StateAbbreviation = state;
    }
}
