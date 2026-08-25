namespace WoobaEtl.Domain;

public record RawCustomer(
    string Name,
    string Email,
    string BirthDate,
    string PhoneNumber,
    string City,
    string StateAbreviattion
);