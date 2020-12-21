using Brimborium.Latrans.Activity;
using Brimborium.Latrans.Mediator;

using FASTER.core;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.Latrans.StoreageFASTER.Test {
    public class BasicFasterKVTests {
        [Fact]
        public async Task Faster_Test1() {

            CheckpointType checkpointType = CheckpointType.FoldOver;
            //var test_path = System.IO.Path.GetTempPath();
            var test_path = @"C:\temp\Faster";
            //
            var logPath = System.IO.Path.Combine(test_path, "hlog.log");
            var objLogPath = System.IO.Path.Combine(test_path, "hlog.obj.log");

            var logDevice = Devices.CreateLogDevice(logPath);
            var objlogDevice = Devices.CreateLogDevice(objLogPath);

            var store = new FasterKV<ActivityEventKey, ActivityEventRecord>
            (
                size: 128,
                logSettings: new LogSettings {
                    LogDevice = logDevice,
                    ObjectLogDevice = objlogDevice,
                    MutableFraction = 0.1,
                    PageSizeBits = 21,
                    MemorySizeBits = 26
                },
                checkpointSettings: new CheckpointSettings {
                    CheckpointDir = test_path,
                    CheckPointType = checkpointType
                },
                serializerSettings: new SerializerSettings<ActivityEventKey, ActivityEventRecord> {
                    keySerializer = () => new ActivityEventKeySerializer(),
                    valueSerializer = () => new ActivityEventRecordSerializer()
                },
                comparer: new ActivityEventKeyFasterEqualityComparer()
            );
            // try { await store.RecoverAsync(); } catch { }
            Assert.True(store.EntryCount >= 0);
            for (int i = 0; i < 1000; i++) {
                using (var session = store.For(new CacheFunctions()).NewSession<CacheFunctions>()) {
                    //var operationId = new Guid("acb9d7c5-366e-42e3-a46d-8dfda57e78e4");
                    var operationId = Guid.NewGuid();
                    var executionId = new Guid("1176b978-d311-4df0-a39e-3caae08ef00f");
                    var key = new ActivityEventKey() {
                        OperationId = operationId,
                        ExecutionId = executionId,
                        SequenceNo = 0
                    };
                    var value = new ActivityEventRecord() {
                        RecordKind = ActivityEventRecordKind.Log,
                        OperationId = operationId,
                        ExecutionId = executionId,
                        SequenceNo = 0,
                        Occurrence = System.DateTime.UtcNow,
                        Status = ActivityStatus.Initialize,
                        User = "test",
                        Data = "data"
                    };
                    //await session.RMWAsync(ref key, ref value)
                    session.Upsert(ref key, ref value);

                    //var status = (await (
                    //            await session.RMWAsync(ref key, ref value)
                    //        ).CompleteAsync()
                    //    ).Complete();
                    await session.CompletePendingAsync();
                    //await session.WaitForCommitAsync();
                    //store.TakeFullCheckpoint(out _);
                }
            }

            /*
            await store.TakeFullCheckpointAsync(checkpointType);
            await store.CompleteCheckpointAsync();
            */
            

            store.Log.Flush(true);

            store.Dispose();
            logDevice.Dispose();
            objlogDevice.Dispose();
            //Assert.Equal(FASTER.core.Status.OK, status);
        }

        [Fact]
        public async Task Faster_Test2() {

            CheckpointType checkpointType = CheckpointType.FoldOver;
            //var test_path = System.IO.Path.GetTempPath();
            var test_path = @"C:\temp\Faster";
            //
            var logPath = System.IO.Path.Combine(test_path, "hlog.log");
            var objLogPath = System.IO.Path.Combine(test_path, "hlog.obj.log");

            var logDevice = Devices.CreateLogDevice(logPath);
            var objlogDevice = Devices.CreateLogDevice(objLogPath);

            var store = new FasterKV<ActivityEventKey, ActivityEventRecord>
            (
                size: 128,
                logSettings: new LogSettings {
                    LogDevice = logDevice,
                    ObjectLogDevice = objlogDevice,
                    MutableFraction = 0.1,
                    PageSizeBits = 21,
                    MemorySizeBits = 26
                },
                checkpointSettings: new CheckpointSettings {
                    CheckpointDir = test_path,
                    CheckPointType = checkpointType
                },
                serializerSettings: new SerializerSettings<ActivityEventKey, ActivityEventRecord> {
                    keySerializer = () => new ActivityEventKeySerializer(),
                    valueSerializer = () => new ActivityEventRecordSerializer()
                },
                comparer: new ActivityEventKeyFasterEqualityComparer()
            );
            
            //try { store.Recover(); } catch { }
            //try { await store.RecoverAsync(); } catch { }
            Assert.True(store.EntryCount >= 0);

            var lst = new List<ActivityEventKey>();
            using (var iter = store.Iterate()) {
                int i = 100;
                while (iter.GetNext(out var recordInfo) && (--i > 0)) {
                    lst.Add(iter.GetKey());
                    //var v = iter.GetValue();
                    //System.Console.Out.WriteLine(v.OperationId);
                }
            }


            using (var session = store.For(new CacheFunctions()).NewSession<CacheFunctions>()) {
                for (int i = 0; i < lst.Count; i++) {
                    var key = lst[i];
                    session.Delete(ref key);
                }
                session.Compact(store.Log.BeginAddress, shiftBeginAddress: true);
            }

            //for (int i = 0; i < 1000; i++) {
            //    using (var session = store.For(new CacheFunctions()).NewSession<CacheFunctions>()) {
            //        //var operationId = new Guid("acb9d7c5-366e-42e3-a46d-8dfda57e78e4");
            //        var operationId = Guid.NewGuid();
            //        var executionId = new Guid("1176b978-d311-4df0-a39e-3caae08ef00f");
            //        var key = new ActivityEventKey() {
            //            OperationId = operationId,
            //            ExecutionId = executionId,
            //            SequenceNo = 0
            //        };
            //        var value = new ActivityEventRecord() {
            //            RecordKind = ActivityEventRecordKind.Log,
            //            OperationId = operationId,
            //            ExecutionId = executionId,
            //            SequenceNo = 0,
            //            Occurrence = System.DateTime.UtcNow,
            //            Status = ActivityStatus.Initialize,
            //            User = "test",
            //            Data = "data"
            //        };
            //        //await session.RMWAsync(ref key, ref value)
            //        session.Upsert(ref key, ref value);

            //        //var status = (await (
            //        //            await session.RMWAsync(ref key, ref value)
            //        //        ).CompleteAsync()
            //        //    ).Complete();
            //        // await session.CompletePendingAsync();
            //        // await session.WaitForCommitAsync();
            //        //store.TakeFullCheckpoint(out _);
            //    }
            //}

            /*
            await store.TakeFullCheckpointAsync(checkpointType);
            await store.CompleteCheckpointAsync();
            */

            store.Log.Flush(true);

            store.Dispose();
            logDevice.Dispose();
            objlogDevice.Dispose();
            //Assert.Equal(FASTER.core.Status.OK, status);
        }
    }

    public class ActivityEventKeyFasterEqualityComparer : IFasterEqualityComparer<ActivityEventKey> {
        public bool Equals(ref ActivityEventKey k1, ref ActivityEventKey k2) {
            return (k1.OperationId == k2.OperationId)
                && (k1.ExecutionId == k2.ExecutionId)
                && (k1.SequenceNo == k2.SequenceNo)
                ;
        }

        public long GetHashCode64(ref ActivityEventKey k) {
            unchecked {
                return k.OperationId.GetHashCode() << 32
                    + k.ExecutionId.GetHashCode() << 8
                    + k.SequenceNo.GetHashCode();
            }
        }
    }

    public class ActivityEventKeySerializer : BinaryObjectSerializer<ActivityEventKey> {
        public override void Deserialize(out ActivityEventKey obj) {
            obj = new ActivityEventKey();
            obj.OperationId = new Guid(reader.ReadBytes(16));
            obj.ExecutionId = new Guid(reader.ReadBytes(16));
            obj.SequenceNo = reader.ReadInt32();
        }
        public override void Serialize(ref ActivityEventKey obj) {
            writer.Write(obj.OperationId.ToByteArray());
            writer.Write(obj.ExecutionId.ToByteArray());
            writer.Write(obj.SequenceNo);
        }
    }

    public class ActivityEventRecordSerializer : BinaryObjectSerializer<ActivityEventRecord> {
        public override void Deserialize(out ActivityEventRecord obj) {
            obj = new ActivityEventRecord();
            obj.RecordKind = (ActivityEventRecordKind)reader.ReadInt32();
            obj.OperationId = new Guid(reader.ReadBytes(16));
            obj.ExecutionId = new Guid(reader.ReadBytes(16));
            obj.SequenceNo = reader.ReadInt32();
            obj.Occurrence = new DateTime(reader.ReadInt64());
            obj.Status = (ActivityStatus)reader.ReadInt32();
            obj.User = reader.ReadString();
            obj.Data = reader.ReadString();
        }
        public override void Serialize(ref ActivityEventRecord obj) {
            writer.Write((int)obj.RecordKind);
            writer.Write(obj.OperationId.ToByteArray());
            writer.Write(obj.ExecutionId.ToByteArray());
            writer.Write(obj.SequenceNo);
            writer.Write(obj.Occurrence.Ticks);
            writer.Write((int)obj.Status);
            writer.Write(obj.User);
            writer.Write(obj.Data);
        }
    }
    public class CacheFunctions : SimpleFunctions<ActivityEventKey, ActivityEventRecord> {
    }
}
