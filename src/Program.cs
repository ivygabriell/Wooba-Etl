using WoobaEtl.Application;
using WoobaEtl.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using WoobaEtl.Domain;

var services = new ServiceCollection();
services.AddTransient<ICustomerReader, CsvCustomerReader>();
services.AddTransient<ICustomerProcessor, CustomerProcessor>(); 

var provider = services.BuildServiceProvider();

var customerReader = provider.GetRequiredService<ICustomerReader>();
var customers = customerReader.ReadAll("data/customer_lot_a.csv");

var processor = provider.GetRequiredService<ICustomerProcessor>();
var result = processor.Process(customers);

Console.WriteLine($"Total lido: {customers.Count}");
Console.WriteLine($"Validos: {result.ValidCustomers.Count}");
Console.WriteLine($"Descartados: {result.DiscardedRows.Count}");

foreach (var row in result.DiscardedRows)
    Console.WriteLine($"Linha {row.LineNumber}: {row.Reason}");
