using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataPath.Generators {
    class UIGenerator : CodeGen/*, System.ComponentModel.INotifyPropertyChanged*/  {
        protected override void StartGenerate(CommonDecl common) {

            result.Append(@"using Conta.Dal;
using Conta.DAL;
using Conta.DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Conta.Model {
");

            result.AppendFormat(@"    public partial class Ui{0} : UiBase {{
", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path));
            CreateConstructor();
        }

        protected override void GenerateLine(CommonDecl common) {
            if (row.Count == 0) return;

            var type = common.TypeName;
            var annotations = new StringBuilder();
            if (type == "string" && !string.IsNullOrWhiteSpace(common.FieldSize))
                annotations.AppendFormat("[StringLength({0})]\r\n", common.FieldSize);
            if (!common.IsNullable)
                annotations.Append("[Required()]\r\n");

            foreach (var r in (from x in row orderby x.Key descending select x)) {
                if (r.Key == 0) {
                    result.Append(annotations);
                    if (string.IsNullOrWhiteSpace(common.ForeignKey)) {
                        result.AppendFormat("public {1} {0} {{\r\n get {{ return original.{0}; }}\r\n set {{ SetProp(original.{0}, value, v => original.{0} = v, \"{0}\"); }}\r\n }}\r\n", r.Value, type);
                    } else {
                        var refType = GetNamespace() + "." + common.ForeignKey;
                        result.AppendFormat("public {1} {0} {{\r\n get {{ return original.{0}; }}\r\n set {{ SetProp(original.{0}, value, v => original.{0} = v, \"{0}\"); }}\r\n }}\r\n", row[0].Value, refType);
                    }
                    result.AppendLine();
                } else if (r.Key == 1)
                    annotations.AppendFormat("[System.ComponentModel.DisplayName(\"{0}\")]\r\n", r.Value);
            }

            //result.AppendLine();

            row.Clear();
        }

        protected override void CreateConstructor() {
            result.AppendFormat(@"        #region Service
        private static TheService service;
        
        public static IDataClientService Service {{ get {{ return service; }} }}
        
        public static void InitService() {{
            if (service != null)
                service = null; //Service.Dispose();
            service = new TheService();
        }}
        #endregion

        internal readonly {0} original;

        public Ui{0}({0} original) :base() {{
            this.original = original;
        }}

", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path));
        }

        protected override string GetResult() {
            var codeLen = this.result.Length;
            var baseResult = base.GetResult();
            return baseResult.Substring(0, codeLen) +
                string.Format(@"        public override IDataClientService GetService() {{ return Service; }}

        #region service implementation
        class TheService : BaseUiService<{0}, Ui{0}> {{

            internal TheService() : base(XmlDal.DataContext.{0}s, new[] KeyValuePair<string, Type>{{ /*add forward refs here*/ }}) {{ }}

            protected override {0} GetOriginal(Ui{0} item) {{ return item.original; }}

            protected override Ui{0} Create({0} original) {{ return new Ui{0}(original); }}
        }}
        #endregion
", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path)) +
          baseResult.Substring(codeLen);
        }

        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        //void x() {
        //    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(""));
        //}
        public override void Save() {
            base.Save();
            // Conta only:
            var newFileName = System.IO.Path.GetDirectoryName(this.Path) + "\\Ui" + System.IO.Path.GetFileName(this.Path) + ".cs";
            //new System.IO.FileInfo(this.Path).re
            if (File.Exists(newFileName))
                File.Delete(newFileName);
            File.Move(this.Path+".cs", newFileName);
        }
    }
}
