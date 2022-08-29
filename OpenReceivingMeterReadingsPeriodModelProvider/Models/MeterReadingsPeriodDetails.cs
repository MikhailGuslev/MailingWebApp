namespace OpenReceivingMeterReadingsPeriodModelProvider.Models;

public sealed record class MeterReadingsPeriodDetails
{
    public string ServiceProviderName { get; init; } = string.Empty;
    public string ProvidedServiceName { get; init; } = string.Empty;
    public string MeteringDevice { get; init; } = string.Empty;
    public DateTime StartTakingReadings { get; init; }
    public DateTime EndTakingReadings { get; init; }
}
