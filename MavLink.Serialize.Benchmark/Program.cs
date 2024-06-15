// See https://aka.ms/new-console-template for more information


using BenchmarkDotNet.Running;
using MavLink.Serialize.Benchmark;

var summary = BenchmarkRunner.Run<DeserializeBenchmark>();

