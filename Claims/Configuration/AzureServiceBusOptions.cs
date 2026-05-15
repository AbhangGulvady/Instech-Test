namespace Claims.Configuration;

public sealed class AzureServiceBusOptions
{
    public const string SectionName = "AzureServiceBus";
    /// example : [xyz-namespace].servicebus.windows.net — used with managed identity.
    public string? FullyQualifiedNamespace { get; set; }

    public string? ConnectionString { get; set; }

    public string QueueName { get; set; } = "audit-messages";
}