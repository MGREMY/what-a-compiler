```

BenchmarkDotNet v0.15.8, Linux Debian GNU/Linux 13 (trixie)
Intel Core i5-7300U CPU 2.60GHz (Max: 3.50GHz) (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method | Mean     | Error     | StdDev    | Min      | Max      | Gen0   | Allocated |
|------- |---------:|----------:|----------:|---------:|---------:|-------:|----------:|
| Lex    | 6.739 μs | 0.0743 μs | 0.0659 μs | 6.637 μs | 6.868 μs | 3.9902 |   6.12 KB |
| Parse  | 1.425 μs | 0.0229 μs | 0.0203 μs | 1.390 μs | 1.453 μs | 2.0695 |   3.17 KB |
