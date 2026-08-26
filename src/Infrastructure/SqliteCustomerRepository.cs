using Microsoft.Data.Sqlite;
using WoobaEtl.Application;
using WoobaEtl.Domain;

namespace WoobaEtl.Infrastructure;

public class SqliteCustomerRepository : ICustomerRepository
{
    private readonly SqliteConnection _connection;

    public SqliteCustomerRepository()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        CreateTable();
    }

    private void CreateTable()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS customers (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE,
                birth_date TEXT NOT NULL,
                phone TEXT,
                city TEXT,
                state TEXT
            );";
        command.ExecuteNonQuery();
    }

    public void Insert(Customer customer)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO customers (name, email, birth_date, phone, city, state)
            VALUES (@name, @email, @birthDate, @phone, @city, @state);";

        command.Parameters.AddWithValue("@name", customer.Name);
        command.Parameters.AddWithValue("@email", customer.Email);
        command.Parameters.AddWithValue("@birthDate", customer.BirthDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("@phone", customer.PhoneNumber);
        command.Parameters.AddWithValue("@city", customer.City);
        command.Parameters.AddWithValue("@state", customer.StateAbbreviation);

        command.ExecuteNonQuery();
    }
    public IReadOnlyList<Customer> GetAll()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT name, email, birth_date, phone, city, state
            FROM customers
            ORDER BY name;";

        var customers = new List<Customer>();

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var created = Customer.TryCreate(
                reader.GetString(0),
                reader.GetString(1),
                DateTime.Parse(reader.GetString(2)),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                out var customer,
                out _);

            if (created)
            {
                customers.Add(customer!);
            }
        }

        return customers;
    }

    public bool Update(string email, string newCity)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            UPDATE customers
            SET city = @city
            WHERE email = @email;";

        command.Parameters.AddWithValue("@city", newCity);
        command.Parameters.AddWithValue("@email", email.Trim().ToLowerInvariant());

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(string email)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            DELETE FROM customers
            WHERE email = @email;";

        command.Parameters.AddWithValue("@email", email.Trim().ToLowerInvariant());

        return command.ExecuteNonQuery() > 0;
    }
}