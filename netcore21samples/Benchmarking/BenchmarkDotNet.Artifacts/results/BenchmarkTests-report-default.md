
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.300
  [Host]  : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  Core    : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT
  DryCore : .NET Core 2.1.0 (CoreCLR 4.6.26515.07, CoreFX 4.6.26515.06), 64bit RyuJIT


                   Method |     Job | Runtime | IsBaseline | LaunchCount | RunStrategy | TargetCount | UnrollFactor | WarmupCount |               Mean |          Error |         StdDev | Scaled | ScaledSD |
------------------------- |-------- |-------- |----------- |------------ |------------ |------------ |------------- |------------ |-------------------:|---------------:|---------------:|-------:|---------:|
 EqualityComparerInt32Old | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
 EqualityComparerInt32New | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                   Time50 | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time100 | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time150 | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                AddByType | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
              AddByInType | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
             AddByRefType | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
         FindElementByRef | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
       FindElementByValue | Default |     Clr |       True |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
 EqualityComparerInt32Old |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |     126,384.207 ns |  6,291.5653 ns | 18,352.7913 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
 EqualityComparerInt32New |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |     123,156.891 ns |  4,478.8898 ns | 12,778.5245 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                   Time50 |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |  50,982,195.277 ns | 11,964.1648 ns |  9,990.6185 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time100 |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default | 100,978,975.729 ns | 24,374.4050 ns | 22,799.8313 ns |   1.00 |     0.00 |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time150 |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default | 150,939,496.667 ns | 62,765.1251 ns | 58,710.5313 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                AddByType |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |           1.514 ns |      0.0907 ns |      0.2676 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
              AddByInType |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |           1.088 ns |      0.0376 ns |      0.1097 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
             AddByRefType |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |           1.123 ns |      0.0548 ns |      0.1572 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
         FindElementByRef |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |      62,800.575 ns |  3,236.1226 ns |  9,490.9873 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
       FindElementByValue |    Core |    Core |    Default |     Default |     Default |     Default |           16 |     Default |      83,746.641 ns |  4,746.1206 ns | 13,994.0435 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
 EqualityComparerInt32Old | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |   4,156,900.000 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
 EqualityComparerInt32New | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |   2,318,600.000 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                   Time50 | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |  53,545,000.000 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time100 | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 | 103,412,200.000 ns |             NA |      0.0000 ns |   1.00 |     0.00 |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time150 | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 | 152,419,300.000 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                AddByType | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |         109.300 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
              AddByInType | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |         103.040 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
             AddByRefType | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |         164.430 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
         FindElementByRef | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |   2,297,500.000 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
       FindElementByValue | DryCore |    Core |    Default |           1 |   ColdStart |           1 |            1 |           1 |   2,590,000.000 ns |             NA |      0.0000 ns |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
 EqualityComparerInt32Old |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
 EqualityComparerInt32New |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                   Time50 |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time100 |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                  Time150 |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
                AddByType |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
              AddByInType |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
             AddByRefType |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
         FindElementByRef |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |
                          |         |         |            |             |             |             |              |             |                    |                |                |        |          |
       FindElementByValue |    Mono |    Mono |    Default |     Default |     Default |     Default |           16 |     Default |                 NA |             NA |             NA |      ? |        ? |

Benchmarks with issues:
  BenchmarkTests.EqualityComparerInt32Old: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.EqualityComparerInt32New: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.Time50: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.Time100: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.Time150: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.AddByType: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.AddByInType: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.AddByRefType: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.FindElementByRef: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.FindElementByValue: Job-FIVGUK(Runtime=Clr, IsBaseline=True)
  BenchmarkTests.EqualityComparerInt32Old: Mono(Runtime=Mono)
  BenchmarkTests.EqualityComparerInt32New: Mono(Runtime=Mono)
  BenchmarkTests.Time50: Mono(Runtime=Mono)
  BenchmarkTests.Time100: Mono(Runtime=Mono)
  BenchmarkTests.Time150: Mono(Runtime=Mono)
  BenchmarkTests.AddByType: Mono(Runtime=Mono)
  BenchmarkTests.AddByInType: Mono(Runtime=Mono)
  BenchmarkTests.AddByRefType: Mono(Runtime=Mono)
  BenchmarkTests.FindElementByRef: Mono(Runtime=Mono)
  BenchmarkTests.FindElementByValue: Mono(Runtime=Mono)
