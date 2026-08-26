using WoobaEtl.Domain;

namespace WoobaEtl.Application;

public record ProcessingResult(
    IReadOnlyList<Customer> ValidCustomers,
    IReadOnlyList<DiscardedRow> DiscardedRows
);