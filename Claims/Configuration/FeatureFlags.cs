namespace Claims.Configuration;

public sealed class FeatureFlags
{
    public const string SectionName = "Features";

    // default is false, so that we don't accidentally use Azure Service Bus in development or testing environments.
    // we will use this flag in future iterations
    // to switch between the in-memory implementation of IAuditMessageBus and an Azure Service Bus implementation.
    public bool UseAzureServiceBus { get; set; }
}