using BenchmarkDotNet.Running;

BenchmarkRunner.Run<TrainLookupBenchmarks>();


/// CACHING
//| Method | Mean | Error | StdDev | Median | Allocated |
//| ------------------------ | -------------:| ------------:| ------------:| -------------:| ----------:|
//| LookupUsingGetNodeAt | 1,343.234 us | 174.9199 us | 515.7554 us | 1,161.753 us | - |
//| LookupUsingIndexOfNode | 30.638 us | 0.8839 us | 2.4345 us | 30.431 us | - |
//| LookupUsingIndexOfValue | 19.598 us | 0.6379 us | 1.8609 us | 19.248 us | - |
//| RepeatedBranchLength | 6.562 us | 0.3412 us | 0.9513 us | 6.503 us | - |

/// NO CACHING
//| Method | Mean | Error | StdDev | Median | Allocated |
//| ------------------------ | -----------:| -----------:| ------------:| -----------:| ----------:|
//| LookupUsingGetNodeAt | 1.642 ms | 0.2266 ms | 0.6680 ms | 1.466 ms | - |
//| LookupUsingIndexOfNode | 572.726 ms | 77.3510 ms | 228.0712 ms | 642.443 ms | 400 B |
//| LookupUsingIndexOfValue | 427.752 ms | 30.1201 ms | 86.4201 ms | 391.259 ms | 176200 B |
//| RepeatedBranchLength | 24.602 ms | 0.4773 ms | 0.7571 ms | 24.617 ms | 12 B |<