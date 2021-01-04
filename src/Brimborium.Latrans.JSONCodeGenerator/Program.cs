#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Brimborium.Latrans.JSONCodeGenerator {
    class CommandlineArguments {
        public List<string> InputFiles { get; private set; }
        public List<string> InputDirectories { get; private set; }
        public string OutputPath { get; private set; }
        public List<string> ConditionalSymbols { get; private set; }
        public string ResolverName { get; private set; }
        public string NamespaceRoot { get; private set; }
        public bool AllowInternal { get; private set; }

        public bool IsValid { get; set; }

        public CommandlineArguments(string[] args, TextWriter consoleOut) {
            InputFiles = new List<string>();
            InputDirectories = new List<string>();
            ConditionalSymbols = new List<string>();
            AllowInternal = false;
            NamespaceRoot = "Brimborium.Latrans.JSON";
            ResolverName = "GeneratedResolver";

            var option = new OptionSet()
            {
                { "i|inputFiles=", "[optional]Input path of cs files(',' separated)", x => { InputFiles.AddRange(x.Split(',')); } },
                { "d|inputDirs=",  "[optional]Input path of dirs(',' separated)", x =>  { InputDirectories.AddRange(x.Split(',')); } },
                { "o|output=", "[required]Output file path", x => { OutputPath = x; } },
                { "f|allowInternal", "[optional, default=false]Allow generate internal(friend)", x => { AllowInternal = true; } },
                { "c|conditionalsymbol=", "[optional, default=empty]conditional compiler symbol", x => { ConditionalSymbols.AddRange(x.Split(',')); } },
                { "r|resolvername=", "[optional, default=GeneratedResolver]Set resolver name", x => { ResolverName = x; } },
                { "n|namespace=", "[optional, default=MessagePack]Set namespace root name", x => { NamespaceRoot = x; } },
            };
            if (args.Length == 0) {
                ShowHelp();
                IsValid = false;
            } else {
                option.Parse(args);

                if ((InputFiles.Count == 0 && InputDirectories.Count == 0) || OutputPath == null) {
                    ShowInvalidArgument(args);
                    IsValid = false;
                    return;
                } else { 
                    IsValid = true;
                    return;
                }
            }
            
            void ShowInvalidArgument(string[] invalidArguments) {
                consoleOut.WriteLine("Invalid Argument:" + string.Join(" ", args));
                consoleOut.WriteLine();
                consoleOut.WriteLine("arguments help:");
                option.WriteOptionDescriptions(consoleOut);
            }

            void ShowHelp() {
                consoleOut.WriteLine("arguments help:");
                option.WriteOptionDescriptions(consoleOut);
            }
        }

        public string GetNamespaceDot() {
            return string.IsNullOrWhiteSpace(NamespaceRoot) ? "" : NamespaceRoot + ".";
        }
    }

    class Program {
        static void Main(string[] args) {
            //var inputFiles = new string[0];
            //var inputDirectories = new[] { @"C:\Users\y.kawai\Documents\Grani\IvoryLfs\Ivory\Ivory.Shared" };

            //var c = new TypeCollector(inputFiles, inputDirectories, new string[0], true);
            args = new string[] {
                // @"--inputDirs=C:\github\FlorianGrimm\Latrans\src\Brimborium.Latrans.Contracts\",
                @"--inputFiles=C:\github\FlorianGrimm\Latrans\src\Brimborium.Latrans.JSONCodeGenerator\CodeAnalysis\Definitions.cs",
                @"--output=C:\temp\gen\gen.cs"
            };


            var cmdArgs = new CommandlineArguments(args, System.Console.Out);
            if (!cmdArgs.IsValid) {
                return;
            }

            // Generator Start...

            var sw = Stopwatch.StartNew();
            Console.WriteLine("Project Compilation Start:" + string.Join(",", cmdArgs.InputFiles) + " " + string.Join(",", cmdArgs.InputDirectories));

            var collector = new TypeCollector(
                cmdArgs.InputFiles, 
                cmdArgs.InputDirectories, 
                cmdArgs.ConditionalSymbols, 
                !cmdArgs.AllowInternal);

            Console.WriteLine("Project Compilation Complete:" + sw.Elapsed.ToString());
            Console.WriteLine();

            sw.Restart();
            Console.WriteLine("Method Collect Start");

            var (objectInfo, genericInfo) = collector.Collect();

            Console.WriteLine("Method Collect Complete:" + sw.Elapsed.ToString());

            Console.WriteLine("Output Generation Start");
            sw.Restart();

            var objectFormatterTemplates = objectInfo
                .GroupBy(x => x.Namespace)
                .Select(x => new FormatterTemplate() {
                    Namespace = cmdArgs.GetNamespaceDot() + "Formatters" + ((x.Key == null) ? "" : "." + x.Key),
                    objectSerializationInfos = x.ToArray(),
                })
                .ToArray();

            var resolverTemplate = new ResolverTemplate() {
                Namespace = cmdArgs.GetNamespaceDot() + "Resolvers",
                FormatterNamespace = cmdArgs.GetNamespaceDot() + "Formatters",
                ResolverName = cmdArgs.ResolverName,
                registerInfos = genericInfo.Cast<IResolverRegisterInfo>().Concat(objectInfo).ToArray()
            };

            var sb = new StringBuilder();
            sb.AppendLine(resolverTemplate.TransformText());
            sb.AppendLine();

            foreach (var item in objectFormatterTemplates) {
                var text = item.TransformText();
                sb.AppendLine(text);
            }

            Output(cmdArgs.OutputPath, sb.ToString());

            Console.WriteLine("String Generation Complete:" + sw.Elapsed.ToString());
        }

        static void Output(string path, string text) {
            path = path.Replace("global::", "");

            const string prefix = "[Out]";
            Console.WriteLine(prefix + path);

            var fi = new FileInfo(path);
            if (!fi.Directory.Exists) {
                fi.Directory.Create();
            }

            System.IO.File.WriteAllText(path, text, Encoding.UTF8);
        }
    }
}
