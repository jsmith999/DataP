using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using IoPath = System.IO.Path;

namespace DataPath.Generators {
    abstract class BaseGenerator : IGenerator {
        protected List<KeyValuePair<int, string>> row = new List<KeyValuePair<int, string>>();

        public string ContainerName { get; set; }
        public string Path { get; set; }
        public string Code { get { return GetResult(); } }
        public int StartCol { get; set; }

        private bool isFirstRow = true;

        public virtual void AddValue(int index, string value) {
            if (value[0] != '{' && value[value.Length - 1] != '}')
                row.Add(new KeyValuePair<int, string>(index, value));
            Debug.WriteLine("[{0}]={1}", index, value);
        }

        public void Generate(CommonDecl common) { 
            if (isFirstRow) {
                StartGenerate(common);
                isFirstRow = false;
            }

            GenerateLine(common);
        }

        public virtual void Save() { Debug.WriteLine(this.Code); }

        protected virtual void StartGenerate(CommonDecl common) { }

        protected virtual void GenerateLine(CommonDecl common) { row.Clear(); }

        protected abstract string GetResult();

        protected  string GetNamespace() {
            return IoPath.GetDirectoryName(this.Path).Replace("\\", ".").Replace("/", ".");
        }

    }
}
