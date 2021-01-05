using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.IO;
using Brimborium.Latrans.Utility;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
// EventLogRecordReadable
namespace Brimborium.Latrans.Storeage.Readable {
    public sealed class EventLogStorage
        : IEventLogStorage
        , IDisposable {
        public const string FileNamePattern = "log-{dtPart}{version}.log";

        public static Task<IEventLogStorage?> CreateAsync(
            EventLogStorageOptions options,
            ILocalFileSystem? localFileSystem = default,
            ISystemClock? systemClock = default) {
            var result = new EventLogStorage(options, localFileSystem, systemClock);
            result.Initialize();
            //await result.InitializeAsync();
            return Task.FromResult<IEventLogStorage?>(result);
        }

        private readonly string _BaseFolder;
        private readonly ILocalFileSystem _LocalFileSystem;
        private readonly ISystemClock _SystemClock;
        private AsyncQueue _LastWrite;
        private EventLogStorageFileBase? _StorageFile;

        //private string? _DtPart;
        private int _IsDisposed;

        public EventLogStorage(
            EventLogStorageOptions options,
            ILocalFileSystem? localFileSystem = default,
            ISystemClock? systemClock = default
            ) {
            this._BaseFolder = options.BaseFolder;
            this._LocalFileSystem = localFileSystem ?? new LocalFileSystem();
            this._SystemClock = systemClock ?? new SystemClock();
            this._LastWrite = AsyncQueue.Create();
        }

        public void Initialize() {
            if (!System.IO.Directory.Exists(this._BaseFolder)) {
                System.IO.Directory.CreateDirectory(this._BaseFolder);
            }

            var utcNow = this._SystemClock.UtcNow;
            //var dtPart = utcNow.ToString("dd-HH");
            var storageFile = this.EnsureStorageFile(utcNow, null);
            if (storageFile is object) {
                storageFile.Initialize();
                this._StorageFile = storageFile;
            }
        }

        public Task ReadAsync(Action<EventLogRecord> callback) {
            throw new NotImplementedException();
        }
        public void Write(EventLogRecord eventLogRecord) {
            if (eventLogRecord.DT == System.DateTime.MinValue) {
                eventLogRecord.DT = this._SystemClock.UtcNow;
            }
            var utcNow = eventLogRecord.DT;
            if ((eventLogRecord.DataObject is object)
                && (eventLogRecord.DataByte == null)
                && string.IsNullOrEmpty(eventLogRecord.DataText)) {
                eventLogRecord.DataByte = Utf8Json.JsonSerializer.Serialize(eventLogRecord.DataObject);
            }
            var storageFile = this._StorageFile;
            var nextStorageFile = this.EnsureStorageFile(utcNow, storageFile);
            if (nextStorageFile is object) {
                nextStorageFile.Initialize();
                
                var oldStorageFile = System.Threading.Interlocked.CompareExchange(
                    ref this._StorageFile,
                        nextStorageFile,
                        storageFile);
                nextStorageFile.Write(eventLogRecord);
                if (ReferenceEquals(oldStorageFile, storageFile)) {
                    this._LastWrite.Next((innerState) => {
                        innerState?.Dispose();
                        return Task.CompletedTask;
                    }, storageFile);
                }
                    
            } else if (storageFile is object) {
                storageFile.Write(eventLogRecord);
            }
        }

#if false
        public async Task WriteAsync(EventLogRecord eventLogRecord) {
            if (eventLogRecord.DT == System.DateTime.MinValue) {
                eventLogRecord.DT = this._SystemClock.UtcNow;
            }
            var utcNow = eventLogRecord.DT;
            if ((eventLogRecord.DataObject is object)
                && (eventLogRecord.DataByte == null)
                && string.IsNullOrEmpty(eventLogRecord.DataText)) {
                eventLogRecord.DataByte = Brimborium.Latrans.JSON.JsonSerializer.Serialize(eventLogRecord.DataObject);
            }
            var storageFile = this._StorageFile;
            var nextStorageFile = this.EnsureStorageFile(utcNow, storageFile);
            if (nextStorageFile is object) {
                this._LastWrite.Next(
                    async (state) => {
                        await nextStorageFile.InitializeAsync();
                        var oldStorageFile = System.Threading.Interlocked.CompareExchange(
                            ref this._StorageFile,
                                state.nextStorageFile,
                                state.storageFile);
                        await nextStorageFile.WriteAsync(eventLogRecord);
                        if (ReferenceEquals(oldStorageFile, state.storageFile)) {
                            this._LastWrite.Next((innerState) => {
                                innerState?.Dispose();
                                return Task.CompletedTask;
                            }, storageFile);
                        }
                    },
                    (storageFile, nextStorageFile, eventLogRecord)
                    );
            } else if (storageFile is object) {
                this._LastWrite.Next(
                    async (state) => {
                        await state.storageFile.WriteAsync(state.eventLogRecord);
                    },
                    (storageFile, eventLogRecord)
                    );
            }
        }
#endif

        public EventLogStorageFileBase? EnsureStorageFile(
                DateTime utcNow,
                EventLogStorageFileBase? storageFile
            ) {
            var dtPart = utcNow.ToString("dd-HH");
            if ((storageFile is object)
                && (string.Equals(dtPart, storageFile.DtPart, StringComparison.Ordinal))
                ) {
                return null;
            } else {
                string filePath;
                FileMode fileMode;
                int nbrVersion = 0;
                while (true) {
                    string txtVersion = (nbrVersion == 0) ? string.Empty : $"-{nbrVersion}";
                    var fileName = FileNamePattern
                        .Replace("{dtPart}", dtPart)
                        .Replace("{version}", txtVersion)
                        ;

                    filePath = System.IO.Path.Combine(this._BaseFolder, fileName);

                    if (this._LocalFileSystem.FileExists(filePath)) {
                        var lastWriteTimeUtc = this._LocalFileSystem.FileGetLastWriteTimeUtc(filePath);
                        if (utcNow.Subtract(lastWriteTimeUtc).TotalMinutes > 60) {
                            fileMode = FileMode.OpenOrCreate;
                            break;
                        } else {
                            nbrVersion++;
                            continue;
                        }
                    } else {
                        fileMode = FileMode.Create;
                        break;
                    }
                }
                List<string>? filesToDelete = null;
                if (nbrVersion == 0) {
                    var pattern = FileNamePattern
                        .Replace("{dtPart}", dtPart)
                        .Replace("{version}", "-")
                        ;
                    filesToDelete = new List<string>(
                            this._LocalFileSystem.EnumerateFiles(this._BaseFolder, pattern, SearchOption.TopDirectoryOnly)
                        );
                }
                return new EventLogStorageFile(
                        dtPart,
                        nbrVersion,
                        filePath,
                        fileMode,
                        filesToDelete,
                        this._LocalFileSystem,
                        this._SystemClock
                    );
            }
        }

#if false
        public (
            string filePath,
            int nbrVersion,
            FileMode fileMode,
            List<string>? filesToDelete
        ) GetNextFileName(
            DateTime utcNow,
            string dtPart
        ) {
            string filePath;
            this._DtPart = dtPart;
            FileMode fileMode = FileMode.Create;
            int nbrVersion = 0;
            while (true) {
                string txtVersion = (nbrVersion == 0) ? string.Empty : $"-{nbrVersion}";
                var fileName = "log-{dtPart}{version}.log"
                    .Replace("{dtPart}", dtPart)
                    .Replace("{version}", txtVersion)
                    ;

                filePath = System.IO.Path.Combine(this._BaseFolder, fileName);

                if (this._LocalFileSystem.FileExists(filePath)) {
                    var lastWriteTimeUtc = this._LocalFileSystem.FileGetLastWriteTimeUtc(filePath);
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
                    fileMode = FileMode.Create;
                    break;
                }
            }
            List<string>? filesToDelete = null;
            if (nbrVersion == 0) {
                var pattern = "log-{dtPart}{version}.log"
                    .Replace("{dtPart}", dtPart)
                    .Replace("{version}", "-")
                    ;
                filesToDelete = new List<string>(
                        this._LocalFileSystem.EnumerateFiles(this._BaseFolder, pattern, new EnumerationOptions() { })
                    );
            }
            return (filePath, nbrVersion, fileMode, filesToDelete);
        }
#endif

        private void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (disposing) {
                    using (var storageFile = this._StorageFile) {
                        this._StorageFile = null;
                    }
                } else {
                    try {
                        using (var storageFile = this._StorageFile) {
                            this._StorageFile = null;
                        }
                    } catch { 
                    }
                }
            }
        }

        ~EventLogStorage() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class EventLogStorageFileBase
        : IDisposable {
        protected int _IsDisposed;
        protected readonly string _DtPart;
        protected readonly int _NbrVersion;
        protected readonly string _FilePath;
        protected readonly FileMode _FileMode;
        protected List<string>? _FilesToDelete;
        protected FileStream? _Stream;
        protected readonly ILocalFileSystem _LocalFileSystem;
        protected readonly ISystemClock _SystemClock;

        public EventLogStorageFileBase(
                string dtPart,
                int nbrVersion,
                string filePath,
                FileMode fileMode,
                List<string>? filesToDelete,
                ILocalFileSystem localFileSystem,
                ISystemClock systemClock
            ) {
            this._DtPart = dtPart;
            this._NbrVersion = nbrVersion;
            this._FilePath = filePath;
            this._FileMode = fileMode;
            this._FilesToDelete = filesToDelete;
            this._LocalFileSystem = localFileSystem;
            this._SystemClock = systemClock;
        }

        public string DtPart => this._DtPart;
        public int NbrVersion => this._NbrVersion;
        public string FilePath => this._FilePath;
        public FileMode FileMode => this._FileMode;
        public List<string>? FilesToDelete => this._FilesToDelete;


        public virtual void Initialize() { }
        public virtual Task ReadAsync(Action<EventLogRecord> callback) { return Task.CompletedTask; }
        public virtual void Write(EventLogRecord eventLogRecord) { }

        private void Dispose(bool disposing) {
            if (0 == System.Threading.Interlocked.Exchange(ref this._IsDisposed, 1)) {
                if (disposing) {
                    using (var stream = this._Stream) {
                        stream?.Flush();
                        this._Stream = null;
                    }
                } else {
                    try {
                        using (var stream = this._Stream) {
                            stream?.Flush();
                            this._Stream = null;
                        }
                    } catch {
                    }
                }
            }
        }

        ~EventLogStorageFileBase() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
    public sealed class EventLogStorageFile
    : EventLogStorageFileBase {

        public EventLogStorageFile(
                string dtPart,
                int nbrVersion,
                string filePath,
                FileMode fileMode,
                List<string>? filesToDelete,
                ILocalFileSystem localFileSystem,
                ISystemClock systemClock
            ) : base(
                dtPart,
                nbrVersion,
                filePath,
                fileMode,
                filesToDelete,
                localFileSystem,
                systemClock
            ) {
        }

        public override void Initialize() {
            if (this._FileMode == FileMode.Create) {
                this._Stream = System.IO.File.Open(this._FilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
            } else {
                var stream = System.IO.File.Open(this._FilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
                stream.SetLength(0);
                this._Stream = stream;
            }
            var filesToDelete = System.Threading.Interlocked.Exchange(ref this._FilesToDelete, null);
            if (filesToDelete is object) {
                foreach (var fileToDelete in filesToDelete) {
                    try {
                        this._LocalFileSystem.FileDelete(fileToDelete);
                    } catch {
                    }
                }
            }
        }

        public override Task ReadAsync(Action<EventLogRecord> callback) {
            throw new NotImplementedException();
        }

        public override void Write(EventLogRecord eventLogRecord) {
            if ((eventLogRecord.DataObject is object)
                && (eventLogRecord.DataByte == null)
                && string.IsNullOrEmpty(eventLogRecord.DataText)) {
                eventLogRecord.DataByte = Utf8Json.JsonSerializer.Serialize(eventLogRecord.DataObject);
            }

            var stream = this._Stream;
            if (stream is object) {
                ReadableLogUtil.WriteUtf8(eventLogRecord, stream);
                //stream.Flush();
            }
        }

#if false
        public override Task WriteAsync(EventLogRecord eventLogRecord) {
            if ((eventLogRecord.DataObject is object)
                && (eventLogRecord.DataByte == null)
                && string.IsNullOrEmpty(eventLogRecord.DataText)) {
                eventLogRecord.DataByte = Brimborium.Latrans.JSON.JsonSerializer.Serialize(eventLogRecord.DataObject);
            }

            var stream = this._Stream;
            if (stream is object) {
                ReadableLogUtil.WriteUtf8(eventLogRecord, stream);
            }

            return Task.CompletedTask;
        }
#endif
    }
}
