using BenchmarkDotNet.Running;
using FileSnap.Benchmarks;

var config = new Config();
BenchmarkRunner.Run<FileSnapBenchmarks>(config);