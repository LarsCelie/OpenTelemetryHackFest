namespace Application.Api;

using System.Diagnostics.Metrics;

public static class Counters
{
    public static Meter Source = new Meter("Application.Api.Counters");
    public static Counter<int> ApiCallsCounter = Source.CreateCounter<int>("api_calls_count");
}