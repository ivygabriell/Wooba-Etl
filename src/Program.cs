using Microsoft.Extensions.DependencyInjection;
using WoobaEtl.Application;
using WoobaEtl.Infrastructure;

var services = new ServiceCollection();

services.AddTransient<ICustomerReader, CsvCustomerReader>();
services.AddTransient<ICustomerProcessor, CustomerProcessor>();
services.AddSingleton<ICustomerRepository, SqliteCustomerRepository>();
services.AddTransient<EtlService>();

var provider = services.BuildServiceProvider();

var etl = provider.GetRequiredService<EtlService>();
var repository = provider.GetRequiredService<ICustomerRepository>();

const string DefaultCsvPath = "data/customer_lot_a.csv";

var csvPath = args.Length > 0 ? args[0] : DefaultCsvPath;

if (!File.Exists(csvPath))
{
    Console.WriteLine($"Arquivo nao encontrado: {csvPath}");
    return;
}

etl.Run(csvPath);

Console.WriteLine();
Console.WriteLine("======================================================");
Console.WriteLine("Modo interativo. Comandos disponiveis:");
Console.WriteLine("  list                            lista os clientes gravados");
Console.WriteLine("  update <email> <nova-cidade>    atualiza a cidade");
Console.WriteLine("  delete <email>                  exclui o cliente");
Console.WriteLine("  exit                            encerra o programa");
Console.WriteLine("======================================================");
Console.WriteLine();

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var command = parts[0].ToLowerInvariant();

    if (command == "exit")
    {
        Console.WriteLine("Encerrando.");
        break;
    }

    if (command == "list")
    {
        var customers = repository.GetAll();

        if (customers.Count == 0)
        {
            Console.WriteLine("Nenhum cliente gravado.");
            continue;
        }

        Console.WriteLine($"{customers.Count} cliente(s):");
        foreach (var customer in customers)
        {
            Console.WriteLine($"  {customer.Name} | {customer.Email} | {customer.City}/{customer.StateAbbreviation}");
        }

        continue;
    }

    if (command == "update")
    {
        if (parts.Length < 3)
        {
            Console.WriteLine("Uso: update <email> <nova-cidade>");
            continue;
        }

        var email = parts[1];
        var newCity = string.Join(' ', parts.Skip(2));

        var updated = repository.Update(email, newCity);

        Console.WriteLine(updated
            ? $"Cliente {email} atualizado para a cidade {newCity}."
            : $"Cliente {email} nao encontrado.");

        continue;
    }

    if (command == "delete")
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Uso: delete <email>");
            continue;
        }

        var email = parts[1];
        var deleted = repository.Delete(email);

        Console.WriteLine(deleted
            ? $"Cliente {email} excluido."
            : $"Cliente {email} nao encontrado.");

        continue;
    }

    Console.WriteLine($"Comando invalido: {command}");
}