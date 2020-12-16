using FASTER.core;

using System;

using Xunit;

namespace Brimborium.Latrans.StoreageFASTER.Test {
    public class BasicFasterTests {
        [Fact]
        public void Test1() {
            /*
            CheckpointType checkpointType = CheckpointType.Snapshot;
            var test_path = System.IO.Path.GetTempPath();
            var logPath = System.IO.Path.Combine(test_path, "hlog.log");
            var objLogPath = System.IO.Path.Combine(test_path, "hlog.obj.log");
            var log = Devices.CreateLogDevice(logPath);
            var objlog = Devices.CreateLogDevice(objLogPath);
            */
            /*
            var fasterKV = new FasterKV<MyKey, MyLargeValue>
            (128,
            new LogSettings { LogDevice = log, ObjectLogDevice = objlog, MutableFraction = 0.1, PageSizeBits = 21, MemorySizeBits = 26 },
            new CheckpointSettings { CheckpointDir = test_path, CheckPointType = checkpointType },
            new SerializerSettings<MyKey, MyLargeValue> { keySerializer = () => new MyKeySerializer(), valueSerializer = () => new MyLargeValueSerializer() }
            );
            */
        }
    }
}
