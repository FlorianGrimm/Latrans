﻿#nullable disable
namespace Brimborium.Latrans.JSONCodeGenerator {
    using System.Linq;

    /// <summary>
    /// Class to produce the template output
    /// </summary>

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class FormatterTemplate : FormatterTemplateBase {
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText() {
            this.Write("#pragma warning disable 618\r\n");
            this.Write("#pragma warning disable 612\r\n");
            this.Write("#pragma warning disable 414\r\n");
            this.Write("#pragma warning disable 219\r\n");
            this.Write("#pragma warning disable 168\r\n");
            this.Write("\r\n");
            this.Write("namespace ");

            this.Write(this.ToStringHelper.ToStringWithCulture(this.Namespace));

            this.Write("\r\n");
            this.Write("{\r\n");
            this.Write("    using System;\r\n");
            this.Write("\r\n");

            foreach (var objInfo in this.objectSerializationInfos) {
                this.Write("\r\n");
                this.Write("    public sealed class ");

                this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.Name));

                this.Write("Formatter : global::Brimborium.Latrans.JSON.IJsonFormatter<");

                this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.FullName));

                this.Write(">\r\n");
                this.Write("    {\r\n");
                this.Write("        private readonly global::Brimborium.Latrans.JSON.JsonSerializationInfo ____JsonSerializationInfo;\r\n");
                this.Write("\r\n");
                this.Write("        public ");

                this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.Name));

                this.Write("Formatter()\r\n");
                this.Write("        {\r\n");
                this.Write("            this.____JsonSerializationInfo = (new global::Brimborium.Latrans.JSON.JsonSerializationInfoBuilder())\r\n");
                foreach (var x in objInfo.GetMembers()) {
                    this.Write("                .Add(\"");
                    this.Write(this.ToStringHelper.ToStringWithCulture(x.Name));
                    this.Write("\", ");
                    this.Write(this.ToStringHelper.ToStringWithCulture(x.Order));
                    this.Write(", ");
                    this.Write(x.IsReadable?"true":"false");
                    this.Write(", ");
                    this.Write(x.IsWritable?"true":"false");
                    this.Write(")\r\n");
                }
                this.Write("                .Build();\r\n");
                this.Write("        }\r\n");
                this.Write("\r\n");
                this.Write("        public void Serialize(global::Brimborium.Latrans.JSON.JsonWriter writer, ");

                this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.FullName));

                this.Write(" value, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)\r\n");
                this.Write("        {\r\n");

                if (objInfo.IsClass) {

                    this.Write("            if (value == null)\r\n");
                    this.Write("            {\r\n");
                    this.Write("                writer.WriteNull();\r\n");
                    this.Write("                return;\r\n");
                    this.Write("            }\r\n");

                }

                this.Write("            \r\n");
                this.Write("\r\n");
                
                this.Write("            writer.WriteBeginObject();\r\n");

                // index = 0;
                foreach (var x in objInfo.GetMembers()) {
                    if (x.IsReadable) {
                        this.Write("            writer.WriteStartProperty(this.____JsonSerializationInfo,");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.Order));
                        this.Write(");\r\n");

                        this.Write("            ");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.GetSerializeMethodString()));
                        this.Write(";\r\n");
                    } else {
                        this.Write("            // ");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.Order));
                        this.Write("\r\n");
                    }
                }

                //writer.WritePropertyName(this.____JsonSerializationInfo, 0);

                this.Write("            \r\n");
                this.Write("            writer.WriteEndObject();\r\n");
                this.Write("        }\r\n");
                this.Write("\r\n");
                this.Write("        public ");

                this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.FullName));

                this.Write(" Deserialize(global::Brimborium.Latrans.JSON.JsonReader reader, global::Brimborium.Latrans.JSON.IJsonFormatterResolver formatterResolver)\r\n        {\r\n            if (reader.ReadIsNull())\r\n            {\r\n");

                if (objInfo.IsClass) {
                    this.Write("                return null;\r\n");
                } else {
                    this.Write("                throw new InvalidOperationException(\"typecode is null, struct not supported\");\r\n");
                }
                this.Write("            }\r\n");
                this.Write("            \r\n");

                if (!objInfo.HasConstructor) {
                    this.Write("            \r\n");
                    this.Write("            throw new InvalidOperationException(" +
                            "\"generated serializer for IInterface does not support deserialize.\");\r\n");
                } else {
                    this.Write("\r\n");
                    foreach (var x in objInfo.GetMembers()) {
                        this.Write("            var __v__");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.MemberName));
                        this.Write(" = default(");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.Type));
                        this.Write(");\r\n");

                        this.Write("            var __s__");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.MemberName));
                        this.Write(" = false;\r\n");
                    }
                    this.Write("            var ____count = 0;\r\n");
                    this.Write("            reader.ReadIsBeginObjectWithVerify();\r\n");
                    this.Write("            //\r\n");
                    this.Write("            while (!reader.ReadIsEndObjectWithSkipValueSeparator(ref ____count))\r\n");
                    this.Write("            {\r\n");
                    this.Write("                int key;\r\n");
                    this.Write("                if (reader.TryGetParameterValue(this.____JsonSerializationInfo, out key))\r\n");
                    this.Write("                {\r\n");
                    this.Write("                    reader.ReadNextBlock();\r\n");
                    this.Write("                    continue;\r\n");
                    this.Write("                } else {\r\n");
                    this.Write("                    switch (key)\r\n");
                    this.Write("                    {\r\n");
                    foreach (var x in objInfo.GetMembers()) {
                        this.Write("                         case ");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.Order));
                        this.Write(":\r\n");
                        this.Write("                             __v__");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.MemberName));
                        this.Write(" = ");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.GetDeserializeMethodString()));
                        this.Write(";\r\n");
                        this.Write("                             __s__");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.MemberName));
                        this.Write(" = true;\r\n");
                        this.Write("                             break;\r\n");
                    }
                    this.Write("                    default:\r\n");
                    this.Write("                        reader.ReadNextBlock();\r\n");
                    this.Write("                        break;\r\n");
                    this.Write("                    }\r\n");
                    this.Write("                }\r\n");
                    this.Write("            }\r\n");
                    this.Write("\r\n");
                    this.Write("            var ____result = new ");
                    this.Write(this.ToStringHelper.ToStringWithCulture(objInfo.GetConstructorString()));
                    this.Write(";\r\n");
                    foreach (var x in objInfo.GetMembers().Where(x => x.IsWritable && !x.IsConstructorParameter)) {
                        this.Write("            if(__s__");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.MemberName));
                        this.Write(") { ____result.");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.MemberName));
                        this.Write(" = __v__");
                        this.Write(this.ToStringHelper.ToStringWithCulture(x.MemberName));
                        this.Write("; }\r\n");
                    }
                    this.Write("\r\n");
                    this.Write("            return ____result;\r\n");
                }
                this.Write("        }\r\n");
                this.Write("    }\r\n");
            }
            this.Write("}\r\n");
            this.Write("\r\n");
            this.Write("#pragma warning disable 168\r\n");
            this.Write("#pragma warning restore 219\r\n");
            this.Write("#pragma warning restore 414\r\n");
            this.Write("#pragma warning restore 618\r\n");
            this.Write("#pragma warning restore 612");
            return this.GenerationEnvironment.ToString();
        }
    }

#region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class FormatterTemplateBase {
#region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
#endregion
#region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment {
            get {
                if ((this.generationEnvironmentField == null)) {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors {
            get {
                if ((this.errorsField == null)) {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths {
            get {
                if ((this.indentLengthsField == null)) {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent {
            get {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session {
            get {
                return this.sessionField;
            }
            set {
                this.sessionField = value;
            }
        }
#endregion
#region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend) {
            if (string.IsNullOrEmpty(textToAppend)) {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0)
                        || this.endsWithNewline)) {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture)) {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0)) {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline) {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            } else {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend) {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args) {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args) {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message) {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message) {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent) {
            if ((indent == null)) {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent() {
            string returnValue = "";
            if ((this.indentLengths.Count > 0)) {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0)) {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent() {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
#endregion
#region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper {
            private System.IFormatProvider formatProviderField = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider {
                get {
                    return this.formatProviderField;
                }
                set {
                    if ((value != null)) {
                        this.formatProviderField = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert) {
                if ((objectToConvert == null)) {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null)) {
                    return objectToConvert.ToString();
                } else {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper {
            get {
                return this.toStringHelperField;
            }
        }
#endregion
    }
#endregion
}
