using Microsoft.CSharp;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Blaze
{
    internal static class Program
    {
        public static Thread MainThread { get; private set; }

        internal static void Main(string[] args)
        {
            MainThread = new Thread(MainThreadEntry);
            MainThread.Start(args);
        }

        internal static void MainThreadEntry(object state)
        {
            var args = (string[])state;

            if (args.Length < 2)
                throw new Exception("Dunno what to run.");

            var appDirectory = Path.GetFullPath(args[1]);
            var appFile = Path.Combine(appDirectory, "App.cs");
            var appConfigFile = Path.Combine(appDirectory, "BlazeApp.json.config");

            if (!File.Exists(appConfigFile))
                throw new Exception("App configuration file was not found.");
            if (!File.Exists(appFile))
                throw new Exception("No app entry found.");

            var appConfigFileContent = File.ReadAllText(appConfigFile);
            var appConfig = JsonConvert.DeserializeObject<AppConfiguration>(appConfigFileContent);

            var tempAssemblyPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            var codeProvider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", appConfig.CompilerVersion } });
            var compilerParameters = new CompilerParameters(appConfig.References, tempAssemblyPath, false)
            {
                GenerateInMemory = true
            };
            var compilerResult = codeProvider.CompileAssemblyFromFile(compilerParameters, new[] { appFile });

            if (compilerResult.Errors.Count > 0)
                throw new Exception("There were errors during compilation.");

            // TODO: Invoke compiled entry method -- http://stackoverflow.com/questions/14479074/c-sharp-reflection-load-assembly-and-invoke-a-method-if-it-exists
        }
    }
}
