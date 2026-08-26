namespace WoobaEtl.Application;

public record DiscardedRow(
    int LineNumber,
    string Reason
);