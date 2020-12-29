using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Brimborium.Latrans.IO {
    public static class ReadableLogUtil {
        private static System.Globalization.CultureInfo? _InvariantCulture;

        public static void Read<TState>(
                TextReader textReader, 
                TState state, 
                Action<TState, EventLogRecord> read
            ) {
            var invariantCulture = (_InvariantCulture ??= System.Globalization.CultureInfo.InvariantCulture);
            StringBuilder sbValue = new StringBuilder(4096);
            var stringComparer = StringComparer.Ordinal;
            int ch;
            EventLogRecord result = new EventLogRecord() {
                LgId = 0,
                DT = DateTime.MinValue,
                Key = null,
                TypeName = null,
                DataText = null,
            };
            long ln = -1;
            ch = textReader.Read();
            bool valid = true;
            while (ch >= 0) {
                // -\r\n
                if (CheckAndRead(ref ch, '-', textReader)) {
                    if (ReadLineFeed(ref ch, textReader) >= 0 && ch >= 0) {

                        while (valid && CheckAndRead(ref ch, ' ', textReader)) {
                            // _lg:_ | _ln:_
                            if (valid && CheckAndRead(ref ch, 'l', textReader)) {
                                // _lg:_
                                if (valid = CheckAndRead(ref ch, 'g', textReader)) {
                                    if (valid = CheckAndRead(ref ch, ':', textReader)) {
                                        if (valid = CheckAndRead(ref ch, ' ', textReader)) {
                                            sbValue.Clear();
                                            ReadLineFeedExclude(ref ch, textReader, sbValue);
                                            if (ulong.TryParse(sbValue.ToString(), out var lgId)) {
                                                result.LgId = lgId;
                                            }
                                            continue;
                                        }
                                    }
                                }
                                // _ln:_
                                if (valid = CheckAndRead(ref ch, 'n', textReader)) {
                                    if (valid = CheckAndRead(ref ch, ':', textReader)) {
                                        if (valid = CheckAndRead(ref ch, ' ', textReader)) {

                                            sbValue.Clear();
                                            ReadLineFeedExclude(ref ch, textReader, sbValue);
                                            if (long.TryParse(sbValue.ToString(), out var lclLn)) {
                                                ln = lclLn;
                                            }
                                            continue;
                                        }
                                    }
                                }
                                continue;
                            }
                            // _at:_
                            if (valid && CheckAndRead(ref ch, 'a', textReader)) {
                                if (valid = CheckAndRead(ref ch, 't', textReader)) {
                                    if (valid = CheckAndRead(ref ch, ':', textReader)) {
                                        if (valid = CheckAndRead(ref ch, ' ', textReader)) {
                                            sbValue.Clear();
                                            ReadLineFeedExclude(ref ch, textReader, sbValue);
                                            // TryParseExact FormatProvider 
                                            // TryParseExact(string? s, string? format, IFormatProvider? provider, DateTimeStyles style, out DateTime result)
                                            if (DateTime.TryParseExact(
                                                sbValue.ToString(),
                                                "u",
                                                invariantCulture,
                                                System.Globalization.DateTimeStyles.RoundtripKind,
                                                out var dt)) {
                                                result.DT = dt;
                                            }
                                            continue;
                                        }
                                    }
                                }
                                continue;
                            }
                            // _ky:_
                            if (valid && CheckAndRead(ref ch, 'k', textReader)) {
                                if (valid = CheckAndRead(ref ch, 'y', textReader)) {
                                    if (valid = CheckAndRead(ref ch, ':', textReader)) {
                                        if (valid = CheckAndRead(ref ch, ' ', textReader)) {
                                            sbValue.Clear();
                                            ReadLineFeedExclude(ref ch, textReader, sbValue);
                                            result.Key = sbValue.ToString();
                                            continue;
                                        }
                                    }
                                }
                                continue;
                            }
                            // _ty:_
                            if (valid && CheckAndRead(ref ch, 't', textReader)) {
                                if (valid = CheckAndRead(ref ch, 'y', textReader)) {
                                    if (valid = CheckAndRead(ref ch, ':', textReader)) {
                                        if (valid = CheckAndRead(ref ch, ' ', textReader)) {
                                            sbValue.Clear();
                                            ReadLineFeedExclude(ref ch, textReader, sbValue);
                                            result.TypeName = sbValue.ToString();
                                            continue;
                                        }
                                    }
                                }
                                continue;
                            }
                            // _da:_
                            if (valid && CheckAndRead(ref ch, 'd', textReader)) {
                                if (valid = CheckAndRead(ref ch, 'a', textReader)) {
                                    if (valid = CheckAndRead(ref ch, ':', textReader)) {
                                        if (valid = CheckAndRead(ref ch, ' ', textReader)) {
                                            string data;
                                            if (CheckAndRead(ref ch, '|', textReader)) {
                                                ReadLineFeed(ref ch, textReader);

                                                while (CheckAndRead(ref ch, ' ', textReader)) {
                                                    if (CheckAndRead(ref ch, ' ', textReader)) {
                                                        sbValue.Clear();
                                                        ReadLineFeedInclude(ref ch, textReader, sbValue);
                                                    }
                                                }

                                                if ((sbValue.Length >= 2) && sbValue.ToString(sbValue.Length - 2, 2) == "\r\n") {
                                                    sbValue.Remove(sbValue.Length - 2, 2);
                                                } else if ((sbValue.Length >= 1) && (sbValue[sbValue.Length - 1] == '\r') || (sbValue[sbValue.Length - 1] == '\n')) {
                                                    sbValue.Remove(sbValue.Length - 1, 1);
                                                }
                                                data = sbValue.ToString();
                                            } else {
                                                sbValue.Clear();
                                                ReadLineFeedExclude(ref ch, textReader, sbValue);
                                                data = sbValue.ToString();
                                            }
                                            //
                                            if (ln > 0) {
                                                if (data.Length == ln) {
                                                    result.DataText = data;
                                                    read(state, result);
                                                    result = new EventLogRecord() {
                                                        LgId = 0,
                                                        DT = DateTime.MinValue,
                                                        Key = null,
                                                        TypeName = null,
                                                        DataText = null,
                                                    };
                                                } else {
                                                    valid = false;
                                                }
                                                ln = -1;
                                            } else {
                                                result.DataText = data;
                                                read(state, result);
                                                result = new EventLogRecord() {
                                                    LgId = 0,
                                                    DT = DateTime.MinValue,
                                                    Key = null,
                                                    TypeName = null,
                                                    DataText = null,
                                                };
                                            }
                                            continue;
                                        }
                                    }
                                }
                                continue;
                            }
                            //
                            if (!valid) {
                                break;
                            }
                        }
                        // while ' '
                    }
                }
                // invalid
                if (!valid) {
                    while (ch >= 0) {
                        ch = textReader.Read();
                        if (ch == '\r' || ch == '\n') {
                            ch = textReader.Read();
                            if (ch == '\r' || ch == '\n') {
                                ch = textReader.Read();
                            }
                            break;
                        }
                    }
                }
            }
        }

        private static int ReadLineFeedInclude(ref int ch, TextReader textReader, StringBuilder sb) {
            while (ch >= 0) {
                if (CheckAndRead(ref ch, '\r', textReader)) {
                    sb.Append((char)ch);
                    if (CheckAndRead(ref ch, '\n', textReader)) {
                        sb.Append((char)ch);
                        return 2;
                    } else {
                        return 1;
                    }
                } else if (CheckAndRead(ref ch, '\n', textReader)) {
                    sb.Append((char)ch);
                    return 1;
                }
                sb.Append((char)ch);
                ch = textReader.Read();
            }
            return (sb.Length > 0) ? 0 : -1;
        }

        private static int ReadLineFeedExclude(ref int ch, TextReader textReader, StringBuilder sb) {
            while (ch >= 0) {
                if (CheckAndRead(ref ch, '\r', textReader)) {
                    if (CheckAndRead(ref ch, '\n', textReader)) {
                        return 2;
                    } else {
                        return 1;
                    }
                } else if (CheckAndRead(ref ch, '\n', textReader)) {
                    return 1;
                }
                sb.Append((char)ch);
                ch = textReader.Read();
            }
            return (sb.Length > 0) ? 0 : -1;
        }

        private static int ReadLineFeed(ref int ch, TextReader textReader) {
            if (ch >= 0) {
                if (CheckAndRead(ref ch, '\r', textReader)) {
                    if (CheckAndRead(ref ch, '\n', textReader)) {
                        return 2;
                    } else {
                        return 1;
                    }
                } else if (CheckAndRead(ref ch, '\n', textReader)) {
                    return 1;
                } else {
                    return -1;
                }
            } else {
                return 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CheckAndRead(ref int ch, char test, TextReader textReader) {
            if (ch == (int)test) {
                ch = textReader.Read();
                return true;
            } else {
                return false;
            }
        }

        public static void Write(EventLogRecord readableLog, TextWriter textWriter) {
            //var hashMD5 = _HashMD5 ??= MD5.Create();
            textWriter.Write("-"); textWriter.Write('\n');
            textWriter.Write(" lg: "); textWriter.Write(readableLog.LgId); textWriter.Write('\n');
            if (readableLog.DT != DateTime.MinValue) {
                textWriter.Write(" at: "); textWriter.Write(readableLog.DT.ToString("u")); textWriter.Write('\n');
            }
            if (!string.IsNullOrEmpty(readableLog.Key)) {
                textWriter.Write(" ky: "); textWriter.Write(readableLog.Key); textWriter.Write('\n');
            }
            if (!string.IsNullOrEmpty(readableLog.TypeName)) {
                textWriter.Write(" ty: "); textWriter.Write(readableLog.TypeName); textWriter.Write('\n');
            }
            var data = readableLog.DataText;
            textWriter.Write(" ln: "); textWriter.Write(data.Length); textWriter.Write('\n');
            bool newLineFound = false;
            for (int idx = 0; idx < data.Length; idx++) {
                char ch = data[idx];
                if ((ch == '\r') || (ch == '\n')) {
                    newLineFound = true;
                    break;
                }
            }
            if (newLineFound) {
                textWriter.Write(" da: |"); textWriter.Write('\n');
                var dataLines = data.Replace("\n", "\n  ");
                textWriter.Write("  "); textWriter.Write(dataLines); textWriter.Write('\n');
            } else {
                textWriter.Write(" da: "); textWriter.Write(data); textWriter.Write('\n');
            }

        }
    }
}

