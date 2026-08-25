using WoobaEtl.Application;
using WoobaEtl.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using WoobaEtl.Domain;

var services = new ServiceCollection();
services.AddTransient<ICustomerReader, CsvCustomerReader>();
var provider = services.BuildServiceProvider();
var customerReader = provider.GetRequiredService<ICustomerReader>();

var customers = customerReader.ReadAll("data/customer_lot_a.csv");
Console.WriteLine(customers.Count);