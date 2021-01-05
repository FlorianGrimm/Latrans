using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Brimborium.Latrans.IO {
    public interface ILocalFileSystem {
        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The file to check.</param>
        /// <returns>
        /// true if the caller has the required permissions and path contains the name of an existing file;
        //  otherwise, false.
        /// </returns>
        public bool FileExists(string path);

        /// <summary>
        ///     Returns the date and time, in coordinated universal time (UTC), that the specified
        ///     file or directory was last written to.
        /// </summary>
        /// <param name="path">The file or directory for which to obtain write date and time information.</param>
        /// <returns>
        ///     A System.DateTime structure set to the date and time that the specified file
        ///     or directory was last written to. This value is expressed in UTC time.
        /// </returns>
        DateTime FileGetLastWriteTimeUtc(string path);
        IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption);
        void FileDelete(string path);
    }

    public class LocalFileSystem : ILocalFileSystem {
        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption) {
            return System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption);
        }

        public void FileDelete(string path) {
            System.IO.File.Delete(path);
        }

        public bool FileExists(string path) {
            return System.IO.File.Exists(path);
        }

        public DateTime FileGetLastWriteTimeUtc(string path) {
            return System.IO.File.GetLastWriteTimeUtc(path);
        }
    }
}
