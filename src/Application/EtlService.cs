using WoobaEtl.Domain;

namespace WoobaEtl.Application;

public class EtlService
{
    private readonly ICustomerReader _reader;
    private readonly ICustomerProcessor _processor;
    private readonly ICustomerRepository _repository;

    public EtlService(
        ICustomerReader reader,
        ICustomerProcessor processor,
        ICustomerRepository repository)
    {
        _reader = reader;
        _processor = processor;
        _repository = repository;
    }

    public void Run(string filePath)
    {
        var rawCustomers = _reader.ReadAll(filePath);

        var result = _processor.Process(rawCustomers);
        var inserted = 0;
        foreach (var customer in result.ValidCustomers)
        {
            _repository.Insert(customer);
            inserted++;
        }

        if (result.DiscardedRows.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Linhas descartadas:");
            foreach (var row in result.DiscardedRows)
            {
                Console.WriteLine($"  Linha {row.LineNumber}: {row.Reason}");
            }
        }

        var saved = _repository.GetAll();

        Console.WriteLine();
        Console.WriteLine("Clientes gravados:");
        foreach (var customer in saved)
        {
            Console.WriteLine($"  {customer.Name} | {customer.Email} | {customer.City}/{customer.StateAbbreviation}");
        }

        Console.WriteLine();
        Console.WriteLine("Resumo:");
        Console.WriteLine($"  Total lido:       {rawCustomers.Count}");
        Console.WriteLine($"  Total inserido:   {inserted}");
        Console.WriteLine($"  Total descartado: {result.DiscardedRows.Count}");
    }
}