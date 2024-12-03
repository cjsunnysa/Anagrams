```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.100-rc.2.24474.11
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-LJTVQW : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

IterationCount=2  RunStrategy=ColdStart  

```
| Method                                               | Mean    | Error   | StdDev   | Min     | Max     | Median  |
|----------------------------------------------------- |--------:|--------:|---------:|--------:|--------:|--------:|
| GetAnagramsUsingDictionaryCountAlgorithmSingleThread | 2.600 m | 5.166 m | 0.0115 m | 2.591 m | 2.608 m | 2.600 m |
| GetAnagramsUsingDictionaryCountAlgorithmInParallel   | 1.712 m | 6.703 m | 0.0149 m | 1.701 m | 1.722 m | 1.712 m |
