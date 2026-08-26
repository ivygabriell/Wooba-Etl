using WoobaEtl.Domain; 
using WoobaEtl.Application;

namespace WoobaEtl.Infrastructure; 

public class CsvCustomerReader : ICustomerReader
{
    public IReadOnlyList<RawCustomer> ReadAll(string filePath)
    {
        var csvLines = File.ReadAllLines(filePath);

        var rawCustomers = new List<RawCustomer>();    

        for (int i = 1; i < csvLines.Length; i++)
        {
            var fields = csvLines[i].Split(',');

            var rawCustomer = new RawCustomer(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5]);
            rawCustomers.Add(rawCustomer);
        }
        return rawCustomers;
    }
}