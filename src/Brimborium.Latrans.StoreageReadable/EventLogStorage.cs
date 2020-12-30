using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Utility;
using Brimborium.Latrans.IO;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Storeage.Readable {
    public class EventLogStorage
        : IEventLogStorage {
        public static async Task<IEventLogStorage?> CreateAsync(
            EventLogStorageOptions options,
            ILocalFileSystem? localFileSystem = default,
            ISystemClock? systemClock = default) {
            var result = new EventLogStorage(options, localFileSystem, systemClock);
            await result.InitializeAsync();
            return result;
        }

        private readonly string _BaseFolder;
        private string? _DtName;
        private string? _FilePath;
        private FileStream? _File;
        private readonly ISystemClock _SystemClock;

        public EventLogStorage(
            EventLogStorageOptions options,
            ILocalFileSystem? localFileSystem = default,
            ISystemClock? systemClock = default) {
            this._BaseFolder = options.BaseFolder;
            this._SystemClock = systemClock ?? new SystemClock();
        }

        public Task InitializeAsync() {
            if (System.IO.Directory.Exists(this._BaseFolder)) {
                return Task.CompletedTask;
            } else {
                System.IO.Directory.CreateDirectory(this._BaseFolder);
                return Task.CompletedTask;
            }
        }

        public Task ReadAsync(Action<EventLogRecord> callback) {
            throw new NotImplementedException();
        }

        public Task WriteAsync(EventLogRecord eventLogRecord) {
            if ((eventLogRecord.DataObject is object)
                && string.IsNullOrEmpty(eventLogRecord.DataText)) {
                eventLogRecord.DataByte = Utf8Json.JsonSerializer.Serialize(eventLogRecord.DataObject);
            }
            var utcNow = this._SystemClock.UtcNow;
            this.GetFileName(utcNow, (filePath, fileMode) => {
                this._File = System.IO.File.Open(filePath, fileMode, FileAccess.Read);
            });

            throw new NotImplementedException();
        }

        public void GetFileName(DateTime utcNow, Action<string, FileMode> sideEffect) {
            var dtName = utcNow.ToString("dd-HH");

            if (!string.IsNullOrEmpty(this._FilePath)
                && string.Equals(this._DtName, dtName, StringComparison.Ordinal)) {
            } else {
                lock (this) {
                    if (!string.IsNullOrEmpty(this._FilePath)
                        && string.Equals(this._DtName, dtName, StringComparison.Ordinal)) {
                    } else {
                        string filePath;
                        this._DtName = dtName;
                        FileMode fileMode = FileMode.Create;
                        int nbrVersion = 0;
                        while (true) {
                            string txtVersion = (nbrVersion == 0) ? string.Empty : $"-{nbrVersion}";
                            var fileName = "log-{dtName}{version}.log"
                                .Replace("{dtName}", dtName)
                                .Replace("{version}", txtVersion)
                                ;

                            filePath = System.IO.Path.Combine(this._BaseFolder, fileName);

                            if (System.IO.File.Exists(filePath)) {
                                var lastWriteTimeUtc = System.IO.File.GetLastWriteTimeUtc(filePath);
                                if (utcNow.Subtract(lastWriteTimeUtc).TotalMinutes > 60) {
                                    fileMode = FileMode.OpenOrCreate;
                                    break;
                                } else {
                                    //fileMode = FileMode.Append;
                                    //break;
                                    nbrVersion++;
                                    continue;
                                }
                            } else {
                                fileMode = FileMode.OpenOrCreate;
                                break;
                            }
                        }
                        this._FilePath = filePath;
                        sideEffect(filePath, fileMode);
                    }
                }
            }
        }
    }
}
