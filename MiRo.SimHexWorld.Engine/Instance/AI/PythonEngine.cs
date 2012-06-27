using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.IO;
using System.Windows.Forms;
using MiRo.SimHexWorld.Engine.UI;
using MiRo.SimHexWorld.Engine.World.Entities;

namespace MiRo.SimHexWorld.Engine.AI
{
    public interface IScriptable
    {
        void ScriptCallback(string message, object body);
    }

    public class PythonEngine
    {
        private IScriptable _callback;
        private ScriptEngine _pyEngine;
        private ScriptScope _pyScope;
        private readonly ScriptHandler _scriptHandler;

        static Dictionary<IScriptable, PythonEngine> _engines = new Dictionary<IScriptable, PythonEngine>();

        public PythonEngine(IScriptable callback)
        {
            _callback = callback;
            _scriptHandler = new ScriptHandler(_callback);

            _engines.Add(callback, this);
        }

        public void Initialize()
        {
            _pyEngine = Python.CreateEngine();
            _pyScope = _pyEngine.CreateScope();
            _pyScope.SetVariable("handler", _scriptHandler); // ([name], [value]) 
            _pyScope.SetVariable("game", MainWindow.Game);
            _pyScope.SetVariable("config", MainWindow.Config);
        }

        #region python method invocation
        public void Invoke(string varName, string methodName)
        {
            Invoke(varName, methodName, new object[] { });
        }

        public void Invoke(string varName, string methodName, params object[] param)
        {
            try
            {
                var instance = _pyScope.GetVariable(varName);

                // Invoke a method of the class
                _pyEngine.Operations.InvokeMember(instance, methodName, param);
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Method not implemented: " + methodName, "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        #endregion python method invocation

        #region python script imports

        private IEnumerable<string> Import(string rulesDir)
        {
            var files = new List<string>();

            foreach (string path in Directory.GetDirectories(rulesDir))
                files.AddRange(Import(path.Replace(AppDomain.CurrentDomain.BaseDirectory, "")));

            files.AddRange(Directory.GetFiles(rulesDir).Where(path => path.ToLower().EndsWith(".py")));

            return files;
        }

        public void Import()
        {
            string rootDir = AppDomain.CurrentDomain.BaseDirectory;
            string rulesDir = rootDir;

            IEnumerable<string> files = Import(rulesDir);

            foreach (string file in files)
                CompileFileAndExecute(file);
        }

        public void CompileFileAndExecute(string file)
        {
            ScriptSource source = _pyEngine.CreateScriptSourceFromFile(file, Encoding.ASCII, SourceCodeKind.File);
            CompiledCode compiled = source.Compile();
            compiled.Execute(_pyScope);
        }

        public void CompileSourceAndExecute(string code)
        {
            try
            {
                ScriptSource source = _pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                CompiledCode compiled = source.Compile();
                compiled.Execute(_pyScope);
            }
            catch (SyntaxErrorException ex)
            {
                MessageBox.Show("Syntax error in line: " + ex.Line + Environment.NewLine + "Error: " + ex.Message, "Error");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        #endregion python script imports

        // handle the python callbacks
        public class ScriptHandler
        {
            IScriptable _callback;

            public ScriptHandler(IScriptable callback)
            {
                _callback = callback;
            }

            // can be accessed from pyhton via:
            // handler.Callback('test',null)
            public void Callback(string text, object obj = null)
            {
                _callback.ScriptCallback(text, obj);
                //MessageBox.Show(text, "text");
            }
        }

    }
}
