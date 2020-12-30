using Brimborium.Latrans.Contracts;
using Brimborium.Latrans.EventLog;
using Brimborium.Latrans.Utility;
using Brimborium.Latrans.IO;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;

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
        private readonly ILocalFileSystem _LocalFileSystem;
        private readonly ISystemClock _SystemClock;
        private string? _DtName;
        private string? _FilePath;
        private Stream? _File;
        private ReaderWriterLock _Lock;

        public EventLogStorage(
            EventLogStorageOptions options,
            ILocalFileSystem? localFileSystem = default,
            ISystemClock? systemClock = default) {
            this._BaseFolder = options.BaseFolder;
            this._LocalFileSystem = localFileSystem ?? new LocalFileSystem();
            this._SystemClock = systemClock ?? new SystemClock();
            this._Lock = new ReaderWriterLock();
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
                && (eventLogRecord.DataByte == null)
                && string.IsNullOrEmpty(eventLogRecord.DataText)) {
                eventLogRecord.DataByte = Utf8Json.JsonSerializer.Serialize(eventLogRecord.DataObject);
            }
            var utcNow = this._SystemClock.UtcNow;
            
            try {
                this._Lock.AcquireReaderLock(TimeSpan.FromMinutes(5));
                try {
                    var file = this.EnsureFile(utcNow, this.sideEffectEnsureFile);
                    if (file is object) {
                        ReadableLogUtil.WriteUtf8(eventLogRecord, file);
                    }
                } finally {
                    this._Lock.ReleaseReaderLock();
                }
            } catch (ApplicationException) {
            }
            return Task.CompletedTask;
        }

        public Stream? EnsureFile(DateTime utcNow, Func<string, FileMode, List<string>?, Stream?> sideEffect) {
            var dtName = utcNow.ToString("dd-HH");

            if (!string.IsNullOrEmpty(this._FilePath)
                && string.Equals(this._DtName, dtName, StringComparison.Ordinal)
                && (this._File is object)) {
                return this._File;
            } else {
                lock (this) {
                    if (!string.IsNullOrEmpty(this._FilePath)
                        && string.Equals(this._DtName, dtName, StringComparison.Ordinal)
                        && (this._File is object)) {
                        return this._File;
                    } else {
                        System.Threading.Volatile.Write(ref this._DtName, dtName);
                        var (filePath, fileMode, filesToDelete) = GetNextFileName(utcNow, dtName);
                        this._FilePath = filePath;
                        var file = sideEffect(filePath, fileMode, filesToDelete);
                        var oldFile = System.Threading.Interlocked.Exchange(ref this._File, file);
                        if (oldFile is object) {
                            try {
                                oldFile.Flush();
                                oldFile.Dispose();
                            } catch { }
                        }
                        return file;
                    }
                }
            }
        }
        private Stream? sideEffectEnsureFile(string filePath, FileMode fileMode, List<string>? filesToDelete) {
            var file = System.IO.File.Open(filePath, fileMode, FileAccess.Read);
            if (filesToDelete is object) {
                foreach (var fileToDelete in filesToDelete) {
                    try {
                        this._LocalFileSystem.FileDelete(fileToDelete);
                    } catch {
                    }
                }
            }
            return file;
        }

        public (
                string filePath,
                FileMode fileMode,
                List<string>? filesToDelete
            ) GetNextFileName(
                DateTime utcNow,
                string dtName
            ) {
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
                var pattern = "log-{dtName}{version}.log"
                    .Replace("{dtName}", dtName)
                    .Replace("{version}", "-")
                    ;
                filesToDelete = new List<string>(
                        this._LocalFileSystem.EnumerateFiles(this._BaseFolder, pattern, new EnumerationOptions() { })
                    );
            }
            return (filePath, fileMode, filesToDelete);
        }
    }

    public class ReferenceCount<T>
        :IDisposable
        where T:class, IDisposable
        {
        private int _Shared;
        private T? _Instance;
        private ReferenceCount<T>? _Parent;

        public ReferenceCount(T instance) {
            this._Instance = instance;
        }

        private ReferenceCount(ReferenceCount<T> parent) {
            this._Parent = parent;
            this._Instance = parent._Instance;
        }

        public bool TryGetInstance([MaybeNullWhen(false)]out T instance) {
            if (this._Instance is object) {
                instance = this._Instance;
                return true;
            } else {
                instance = default;
                return false;
            }
        }

        public ReferenceCount<T> Share() {
            System.Threading.Interlocked.Increment(this._Shared);
            return new ReferenceCount<T>(this);
        }

        private void Dispose(bool disposing) {
            if (this._Parent is null) {
                var oldInstance = System.Threading.Interlocked.Exchange(ref this._Instance, null);
                if (oldInstance is object) {
                    if (disposing) {
                    } else {
                    }
                }
            } else {
                var oldInstance = System.Threading.Interlocked.Exchange(ref this._Instance, null);
                if (oldInstance is object) {
                    if (disposing) {
                    } else {
                    }
                }
            }
            
        }

        ~ReferenceCount() {
            this.Dispose(disposing: false);
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
