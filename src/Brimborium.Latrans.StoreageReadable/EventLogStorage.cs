using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Utility;

using System;
using System.IO;
using System.Threading.Tasks;

namespace Brimborium.Latrans.Storeage.Readable {
    public class EventLogStorage
        : IEventLogStorage {
        public static async Task<IEventLogStorage?> CreateAsync(
            EventLogStorageOptions options,
            ISystemClock? systemClock = default) {
            var result = new EventLogStorage(options, systemClock);
            await result.InitializeAsync();
            return result;
        }

        private string _BaseFolder;
        private string _FileName;
        private string _FilePath;
        private FileStream _File;
        private readonly ISystemClock _SystemClock;

        public EventLogStorage(EventLogStorageOptions options, ISystemClock? systemClock = default) {
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
                && string.IsNullOrEmpty( eventLogRecord.DataText )) {
                eventLogRecord.DataByte = Utf8Json.JsonSerializer.Serialize(eventLogRecord.DataObject);
            }
            here
            var utcNow = this._SystemClock.UtcNow;
            var fileName = utcNow.ToString("dd-HH");
            if (string.Equals(this._FileName, fileName, StringComparison.Ordinal)) {
            } else {
                this._FileName = fileName;

                string filePath = System.IO.Path.Combine(this._BaseFolder, "log##-##.log".Replace("##-##", fileName));
                this._FilePath = filePath;

                FileMode fileMode;
                if (System.IO.File.Exists(filePath)) {
                    var lastWriteTimeUtc = System.IO.File.GetLastWriteTimeUtc(filePath);
                    if (utcNow.Subtract(lastWriteTimeUtc).TotalMinutes > 60) {
                        fileMode = FileMode.OpenOrCreate;
                    } else {
                        fileMode = FileMode.Append;
                    }                    
                } else {
                    fileMode = FileMode.OpenOrCreate;
                }
                this._File = System.IO.File.Open(filePath, fileMode, FileAccess.Read);
            }
            //this._File.w

            throw new NotImplementedException();
        }
    }
}
