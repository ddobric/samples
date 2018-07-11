``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host]  : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core    : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DryCore : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


```
|                   Method |     Job | Runtime | IsBaseline | LaunchCount | RunStrategy | TargetCount | UnrollFactor | WarmupCount |                Mean |           Error |          StdDev |              Median | Scaled | ScaledSD |
|------------------------- |-------- |-------- |----------- |------------ |------------ |------------ |------------- |------------ |--------------------:|----------------:|----------------:|--------------------:|-------:|---------:|
|                   Time50 | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time100 | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time150 | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
| EqualityComparerInt32New | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                AddByType | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|              AddByInType | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|             AddByRefType | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|         FindElementByRef | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|       FindElementByValue | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                   Time50 |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |  50,592,165.1042 ns |  99,642.1524 ns |  93,205.3221 ns |  50,573,964.6875 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time100 |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default | 100,591,889.0625 ns | 111,811.6242 ns | 104,588.6524 ns | 100,590,222.8125 ns |   1.00 |     0.00 |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time150 |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default | 150,969,060.6250 ns |  19,684.0527 ns |  18,412.4733 ns | 150,963,463.1250 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
| EqualityComparerInt32New |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |      79,217.7353 ns |   1,529.6169 ns |   3,545.1196 ns |      79,866.9438 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                AddByType |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |           0.9202 ns |       0.0602 ns |       0.1745 ns |           0.8626 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|              AddByInType |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |           1.0280 ns |       0.0403 ns |       0.1189 ns |           1.0142 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|             AddByRefType |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |           1.0095 ns |       0.0412 ns |       0.1181 ns |           0.9783 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|         FindElementByRef |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |      79,444.8988 ns |   3,869.1259 ns |  10,656.6965 ns |      75,818.7134 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|       FindElementByValue |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |      75,175.0962 ns |   8,511.1186 ns |  24,827.3324 ns |      68,223.7567 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                   Time50 | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |  52,467,100.0000 ns |              NA |       0.0000 ns |  52,467,100.0000 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time100 | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 | 102,586,000.0000 ns |              NA |       0.0000 ns | 102,586,000.0000 ns |   1.00 |     0.00 |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time150 | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 | 152,425,900.0000 ns |              NA |       0.0000 ns | 152,425,900.0000 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
| EqualityComparerInt32New | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |   1,084,700.0000 ns |              NA |       0.0000 ns |   1,084,700.0000 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                AddByType | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |          87.7900 ns |              NA |       0.0000 ns |          87.7900 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|              AddByInType | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |         100.8400 ns |              NA |       0.0000 ns |         100.8400 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|             AddByRefType | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |         136.7700 ns |              NA |       0.0000 ns |         136.7700 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|         FindElementByRef | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |   1,227,800.0000 ns |              NA |       0.0000 ns |   1,227,800.0000 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|       FindElementByValue | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |   1,428,300.0000 ns |              NA |       0.0000 ns |   1,428,300.0000 ns |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                   Time50 |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time100 |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                  Time150 |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
| EqualityComparerInt32New |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|                AddByType |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|              AddByInType |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|             AddByRefType |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|         FindElementByRef |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |
|                          |         |         |            |             |             |             |              |             |                     |                 |                 |                     |        |          |
|       FindElementByValue |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                  NA |              NA |              NA |                  NA |      ? |        ? |

Benchmarks with issues:
  BenchmarkTests.Time50: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.Time100: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.Time150: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.EqualityComparerInt32New: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.AddByType: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.AddByInType: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.AddByRefType: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.FindElementByRef: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.FindElementByValue: Job-OQLMCN(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.Time50: Mono(Runtime=Mono)
  BenchmarkTests.Time100: Mono(Runtime=Mono)
  BenchmarkTests.Time150: Mono(Runtime=Mono)
  BenchmarkTests.EqualityComparerInt32New: Mono(Runtime=Mono)
  BenchmarkTests.AddByType: Mono(Runtime=Mono)
  BenchmarkTests.AddByInType: Mono(Runtime=Mono)
  BenchmarkTests.AddByRefType: Mono(Runtime=Mono)
  BenchmarkTests.FindElementByRef: Mono(Runtime=Mono)
  BenchmarkTests.FindElementByValue: Mono(Runtime=Mono)
