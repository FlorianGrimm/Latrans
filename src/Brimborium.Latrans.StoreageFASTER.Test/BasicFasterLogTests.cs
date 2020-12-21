using FASTER.core;

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;

using Xunit;

namespace Brimborium.Latrans.StoreageFASTER.Test {
    public class BasicFasterLogTests {
        [Fact]
        public async Task FasterLog_Test1Write() {
            const int entryLength = 1 << 10;
            byte[] staticEntry1 = new byte[entryLength];
            byte[] staticEntry1Brotli = new byte[entryLength];
            for (int i = 0; i < entryLength; i++) {
                staticEntry1[i] = (byte)i;
            }
            byte[] staticEntry2 = new byte[entryLength];
            byte[] staticEntry2Brotli = new byte[entryLength];
            for (int i = 0; i < entryLength; i++) {
                staticEntry2[i] = (byte)(entryLength-i);
            }
            var path = GetPath();
            using (var logCommitManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(path), true, false)
                ) {
                using (IDevice device = Devices.CreateLogDevice(path + "hlog.log")) {


                    //FasterLogScanIterator iter;
                    var logSettings = new FASTER.core.FasterLogSettings() {
                        LogDevice = device,
                        LogChecksum = LogChecksumType.PerEntry,
                        LogCommitManager = logCommitManager

                    };
                    using (var log = new FASTER.core.FasterLog(logSettings)) {
                        //log.TruncateUntilPageStart(0);
                        //await log.CommitAsync();
                        for (int i = 0; i < 1000; i++) {
                            if (BrotliEncoder.TryCompress(staticEntry1, staticEntry1Brotli, out var bytesWritten1, 4, 10)) {
                                await log.EnqueueAsync(new Memory<byte>(staticEntry1Brotli).Slice(0, bytesWritten1));
                            } else { 
                                await log.EnqueueAsync(staticEntry1);
                            }
                            if (BrotliEncoder.TryCompress(staticEntry2, staticEntry2Brotli, out var bytesWritten2, 4, 10)) {
                                await log.EnqueueAsync(new Memory<byte>(staticEntry2Brotli).Slice(0, bytesWritten2));
                            } else {
                                await log.EnqueueAsync(staticEntry2);
                            }
                            
                        }
                        //await log.CommitAsync();
                        log.Commit();
                        var x = $"{log.BeginAddress} - {log.FlushedUntilAddress}";
                        
                    }
                }
            }
        }

        private static string GetPath() {
            return System.IO.Path.GetTempPath() + "FasterLogSample5/";
        }

        [Fact]
        public void FasterLog_Test2Read() {
            const int entryLengthExpected = 1 << 10;
            byte[] staticEntryAct = new byte[entryLengthExpected];

            byte[] staticEntryEx = new byte[entryLengthExpected];
            for (int i = 0; i < entryLengthExpected; i++) {
                staticEntryEx[i] = (byte)i;
            }
            var path = GetPath();
            using (var logCommitManager = new DeviceLogCommitCheckpointManager(
                new LocalStorageNamedDeviceFactory(),
                new DefaultCheckpointNamingScheme(path), true, false)
                ) {
                using (IDevice device = Devices.CreateLogDevice(path + "hlog.log")) {

                    //FasterLogScanIterator iter;
                    var logSettings = new FASTER.core.FasterLogSettings() {
                        ReadOnlyMode = true,
                        LogDevice = device,
                        LogChecksum = LogChecksumType.PerEntry,
                        //LogCommitManager = new LocalLogCommitManager(LogCommitFile)

                    };
                    using (var log = new FASTER.core.FasterLog(logSettings)) {

                        using var iter = log.Scan(0, long.MaxValue, null, false, ScanBufferingMode.DoublePageBuffering, false);
                        while (iter.GetNext(out var entryActual, out var entryLengthActual, out _)) {
                            Assert.Equal(entryLengthExpected, entryActual.Length);
                            Assert.Equal(entryLengthExpected, entryLengthActual);
                            for (int i = 0; i < entryLengthExpected; i++) {
                                Assert.Equal(entryActual[i], (byte)i);
                            }
                        }

                    }
                }
            }
        }
    }
}
