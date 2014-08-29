using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Interpreter
{
    public class InstantCSharp
    {
        private StringBuilder _code = new StringBuilder();
        private readonly CodeDomProvider _compiler = new CSharpCodeProvider();
        private readonly CompilerParameters _parameters = new CompilerParameters();

        public InstantCSharp()
        {
                _parameters.GenerateInMemory = true;

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        _parameters.ReferencedAssemblies.Add(assembly.GlobalAssemblyCache ? assembly.GetName().Name : assembly.Location);
                    }
                    catch (NotSupportedException)
                    {
                    }
                }
        }

        public void Run([NotNull] string cmd, [NotNull] IEnumerable<string> usings)
        {
            var code = new StringBuilder();
            string returnline = null;
            bool haserrors = false;

            if (!cmd.StartsWith("return")) returnline = "return null;";

            foreach (var @using in usings) code.AppendLine(@using);

            code.AppendLine(@"public class Runner
            {
                public object Run()
                {
                    " + _code + cmd + returnline + @"
                }
            }");

            CompilerResults compiled =
                _compiler.CompileAssemblyFromSource(_parameters, code.ToString());

            foreach (CompilerError e in compiled.Errors)
            {
                Console.WriteLine(e.ErrorText);
                if (!e.IsWarning) haserrors = true;
            }
            if (haserrors) return;

            object obj = compiled.CompiledAssembly.CreateInstance("Runner");
            compiled.CompiledAssembly.GetType("Runner")
                    .InvokeMember(
                        "Run",
                        BindingFlags.InvokeMethod,
                        null, obj, new object[] {}
                );

            _code.Append(cmd); // All ok so include the line
            if (returnline == null) // A return call means end of the session
                _code = new StringBuilder();
        }
    }
}
