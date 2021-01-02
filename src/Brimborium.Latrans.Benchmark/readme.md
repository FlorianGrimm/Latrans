# Benchmarks


```
dotnet run -c Release --project src\Brimborium.Latrans.Benchmark --runtimes netcoreapp31 --memory --filter *.SingleThreaded.*

dotnet run -c Release --project src\Brimborium.Latrans.Benchmark --runtimes netcoreapp31 --memory --filter *.MultiThreaded.*

dotnet run -c Release --project src\Brimborium.Latrans.Benchmark --runtimes netcoreapp31 --memory --filter * --warmupCount 1 --iterationCount 1 --invocationCount 1

dotnet run -c Release --project src\Brimborium.Latrans.Benchmark --runtimes netcoreapp31 --memory --filter *.MultiThreaded.* --warmupCount 1 --iterationCount 1 --invocationCount 16

dotnet run -c Release --project src\Brimborium.Latrans.Benchmark --runtimes netcoreapp31 --memory --filter *JsonTest* --warmupCount 1 --iterationCount 1 --invocationCount 16

dotnet run -c Release --project src\Brimborium.Latrans.Benchmark --runtimes netcoreapp31 --memory --filter *EventLogReadable* --warmupCount 1 --iterationCount 1 --invocationCount 16


```


|                 Method |        Mean |      Error |     StdDev | Ratio | RatioSD |    Gen 0 |  Gen 1 | Gen 2 |  Allocated |
|----------------------- |------------:|-----------:|-----------:|------:|--------:|---------:|-------:|------:|-----------:|
|          Scenario1List |    78.20 us |   1.541 us |   1.893 us |  1.00 |    0.00 | 103.3936 | 0.1221 |     - |  422.53 KB |
|    Scenario2ListCopied |   647.22 us |  12.777 us |  18.728 us |  8.32 |    0.25 | 861.3281 |      - |     - | 3520.38 KB |
| Scenario3ImmutableList | 6,626.47 us | 128.235 us | 171.190 us | 84.94 |    2.96 | 156.2500 | 7.8125 |     - |  642.39 KB |
|        Scenario4ImList |   164.79 us |   3.437 us |   3.047 us |  2.12 |    0.07 | 134.7656 |      - |     - |  551.15 KB |