``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host]  : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core    : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DryCore : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


```
|                Method |     Job | Runtime | IsBaseline | LaunchCount | RunStrategy | TargetCount | UnrollFactor | WarmupCount |              Mean |       Error |      StdDev |            Median | Scaled | ScaledSD |
|---------------------- |-------- |-------- |----------- |------------ |------------ |------------ |------------- |------------ |------------------:|------------:|------------:|------------------:|-------:|---------:|
| EqualityComparerInt32 | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                NA |          NA |          NA |                NA |      ? |        ? |
| EqualityComparerInt32 |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |    60,869.6704 ns | 381.8457 ns | 338.4963 ns |    60,975.8411 ns |      ? |        ? |
| EqualityComparerInt32 | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 | 5,440,400.0000 ns |          NA |   0.0000 ns | 5,440,400.0000 ns |      ? |        ? |
| EqualityComparerInt32 |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                NA |          NA |          NA |                NA |      ? |        ? |
|                       |         |         |            |             |             |             |              |             |                   |             |             |                   |        |          |
|             AddByType | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                NA |          NA |          NA |                NA |      ? |        ? |
|             AddByType |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |         0.0000 ns |   0.0000 ns |   0.0000 ns |         0.0000 ns |      ? |        ? |
|             AddByType | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |         0.1047 ns |          NA |   0.0000 ns |         0.1047 ns |      ? |        ? |
|             AddByType |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                NA |          NA |          NA |                NA |      ? |        ? |

Benchmarks with issues:
  BenchmarkTests.EqualityComparerInt32: Job-MGHISP(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.EqualityComparerInt32: Mono(Runtime=Mono)
  BenchmarkTests.AddByType: Job-MGHISP(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.AddByType: Mono(Runtime=Mono)
