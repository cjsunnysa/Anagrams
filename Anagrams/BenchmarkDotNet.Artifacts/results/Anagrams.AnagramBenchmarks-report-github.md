```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.100-rc.2.24474.11
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-YMOMVH : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

IterationCount=1  RunStrategy=ColdStart  

```
| Method                                              | Mean    | Error | Min     | Max     | Median  |
|---------------------------------------------------- |--------:|------:|--------:|--------:|--------:|
| LetterCountAlgorithmUnpartitioned                   | 5.288 m |    NA | 5.288 m | 5.288 m | 5.288 m |
| LetterCountAlgorithmSingleThreadedLengthPartitioned | 4.661 m |    NA | 4.661 m | 4.661 m | 4.661 m |
| LetterCountAlgorithmMultithreadedLengthPartitioned  | 2.070 m |    NA | 2.070 m | 2.070 m | 2.070 m |
