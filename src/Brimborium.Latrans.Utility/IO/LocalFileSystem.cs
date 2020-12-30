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
    }
    public class LocalFileSystem : ILocalFileSystem {
        public bool FileExists(string path) {
            return System.IO.File.Exists(path);
        }
    }
}
