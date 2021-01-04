#nullable disable

using Brimborium.Latrans.JSON.Internal;
using Brimborium.Latrans.JSON.Resolvers;

namespace Brimborium.Latrans.JSON {
    /// <summary>
    /// High-Level API of Brimborium.Latrans.JSON.
    /// </summary>
    public static partial class JsonSerializer {
        static IJsonFormatterResolver defaultResolver;

        /// <summary>
        /// FormatterResolver that used resolver less overloads. If does not set it, used StandardResolver.Default.
        /// </summary>
        public static IJsonFormatterResolver DefaultResolver {
            get {
                if (defaultResolver == null) {
                    defaultResolver = StandardResolver.Default;
                }

                return defaultResolver;
            }
        }

        /// <summary>
        /// Is resolver decided?
        /// </summary>
        public static bool IsInitialized {
            get {
                return defaultResolver != null;
            }
        }


        /// <summary>
        /// Set default resolver of Utf8Json APIs.
        /// </summary>
        /// <param name="resolver"></param>
        public static void SetDefaultResolver(IJsonFormatterResolver resolver) {
            defaultResolver = resolver;
        }
    }
}
