## Exercise 1: Tracing in ASP.NET Core (20 mins)

Objective: Understand how traces help visualize API request flow.

Tasks:

- Add OpenTelemetry NuGet packages for tracing.
- Set up basic TracerProvider to trace HTTP requests using AspNetCoreInstrumentation.
- Choose an OLTP exporter:
  - (Docker option) Add the Aspire Dashboard exporter to visualize traces: https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=powershell
  - Add the console exporter to see traces in the console.
- Run the API and console app together to observe the traces in console.

## Exercise 2: Custom tracing in ASP.NET Core (10 mins)

Objective: Learn how to add custom traces to the API.

Tasks:

- Use the `ActivitySource` API to create custom traces.
- Create multiple Activities inside one API request
- Observe the custom traces in the exporter.

## Exercise 3: Distributed Tracing with the Console Application (15 mins)

Objective: Demonstrate distributed tracing between the console app and the API.

Tasks:

- Add OpenTelemetry to the console app to trace HTTP client requests.
- Ensure trace context is propagated between the console app and the API.
- Use the `ActivitySource` API to create additional custom traces.
- Run the apps and observe how traces are linked together.

# Exercise 4: Exploring Trace Attributes and Tags (10-15 mins)

Objective: Learn how to enrich trace data with meaningful information using attributes.

Tasks:

- Change the name of the application in the trace.
- Add attributes to existing traces or spans (e.g., add the user ID or order ID to a span).
- Rerun the console app and API, and observe how these attributes appear in the exported traces.

## Exercise 5: Adding Metrics to ASP.NET Core (10-15 mins)

Objective: Learn to gather and visualize key performance metrics.

Tasks:

- Add the OpenTelemetry.Metrics package.
- Set up a simple custom metric, like counting incoming API requests.
- Visualize the metric using the console exporter or the Aspire Dashboard.

## Exercise 6: Add structured logging to the console application (10-15 mins)

Objective: Learn to add structured logging which helps with debugging and monitoring.

Tasks:

- Replace `Console.WriteLine` with structured logging using ILogger
- Write logs with additional context (e.g., log levels, timestamps, and custom properties)
- Run the console app and observe the structured logs.
