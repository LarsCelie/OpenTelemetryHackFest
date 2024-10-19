## Exercise 1: Tracing in ASP.NET Core (25 mins)

Objective: Understand how traces help visualize API request flow.

Tasks:

- Add OpenTelemetry NuGet packages for tracing.
- Set up basic TracerProvider to trace HTTP requests using AspNetCoreInstrumentation.
- Run the API and console app, making a few requests.
- Use the console exporter.

Outcome: Participants can see trace data that follows a request through the API.

## Exercise 2: Distributed Tracing with the Console Application (20 mins)

Objective: Demonstrate distributed tracing between the console app and the API.

Tasks:

- Add OpenTelemetry to the console app to trace HTTP client requests.
- Ensure trace context is propagated between the console app and the API.
- Run the apps and observe how traces are linked together.

Outcome: Participants understand how traces are linked between client and server, seeing the end-to-end flow.

## Exercise 3: Adding Metrics to ASP.NET Core (15-20 mins)

Objective: Learn to gather and visualize key performance metrics.

Tasks:

- Add the OpenTelemetry.Metrics package.
- Set up a simple custom metric, like counting incoming API requests.
- Export metrics to the console or a local Prometheus/Grafana setup.

Outcome: Participants see how metrics provide performance insights.

