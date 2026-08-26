using System.Globalization;
using WoobaEtl.Domain;

namespace WoobaEtl.Application;

public class CustomerProcessor : ICustomerProcessor
{
    
    private static readonly string[] DateFormats =
    [
        "dd/MM/yyyy",
        "dd-MM-yyyy",
        "yyyy-MM-dd"
    ];

    public ProcessingResult Process(IReadOnlyList<RawCustomer> rawCustomers)
    {
        var validos = new List<Customer>();
        var descartados = new List<DiscardedRow>();

        var emailsVistos = new HashSet<string>();

        for (var i = 0; i < rawCustomers.Count; i++)
        {
            var rawCustomer = rawCustomers[i];

            var lineNumber = i + 2;

            var name = rawCustomer.Name.Trim();
            var email = rawCustomer.Email.Trim();
            var birthDateText = rawCustomer.BirthDate.Trim();
            var phoneNumber = rawCustomer.PhoneNumber.Trim();
            var city = rawCustomer.City.Trim();
            var stateAbbreviation = rawCustomer.StateAbreviattion.Trim();

            
            if (!DateTime.TryParseExact(
                    birthDateText,
                    DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var birthDate))
            {
                descartados.Add(new DiscardedRow(lineNumber, "data invalida"));
                continue;
            }

            var emailNormalizado = email
                .ToLowerInvariant()
                .Replace(" ", "");


            if (emailsVistos.Contains(emailNormalizado))
            {
                descartados.Add(new DiscardedRow(lineNumber, "e-mail duplicado"));
                continue;
            }

            if (!Customer.TryCreate(
                    name,
                    emailNormalizado,
                    birthDate,
                    phoneNumber,
                    city,
                    stateAbbreviation,
                    out var customer,
                    out var error))
            {
                descartados.Add(new DiscardedRow(lineNumber, error!));
                continue;
            }

            validos.Add(customer!);
            emailsVistos.Add(emailNormalizado);
        }

        return new ProcessingResult(validos, descartados);
    }
}