```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2314)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.100-rc.2.24474.11
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  Job-YTOVAA : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2

InvocationCount=1  IterationCount=1  RunStrategy=ColdStart  
UnrollFactor=1  

```
| Method                                              | WordsLength | Mean    | Error | Gen0          | Gen1       | Gen2      | Allocated |
|---------------------------------------------------- |------------ |--------:|------:|--------------:|-----------:|----------:|----------:|
| LetterCountAlgorithmUnpartitioned                   | Int32[4]    | 5.128 m |    NA | 81679000.0000 | 34000.0000 | 3000.0000 | 477.18 GB |
| LetterCountAlgorithmSingleThreadedLengthPartitioned | Int32[4]    | 5.029 m |    NA | 81678000.0000 | 30000.0000 | 2000.0000 | 477.18 GB |
| LetterCountAlgorithmMultithreadedLengthPartitioned  | Int32[4]    | 1.935 m |    NA | 82600000.0000 | 12000.0000 | 1000.0000 | 477.18 GB |
