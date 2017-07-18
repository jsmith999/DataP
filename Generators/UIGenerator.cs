using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPath.Generators {
    class UIGenerator : CodeGen/*, System.ComponentModel.INotifyPropertyChanged*/  {
        protected override void StartGenerate(CommonDecl common) {
            //base.StartGenerate(common);
            //System.ComponentModel.INotifyPropertyChanged
            if (!string.IsNullOrWhiteSpace(this.Path))
                result.AppendFormat("using System;\r\n\r\nnamespace {0} {{\r\n", System.IO.Path.GetDirectoryName(this.Path).Replace("\\", ".").Replace("/", "."));

            result.AppendFormat("public class {0} : System.ComponentModel.INotifyPropertyChanged {{\r\n", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path));
            CreateConstructor();
        }

        protected override void GenerateLine(CommonDecl common) {
            if (row.Count == 0) return;

            var type = common.TypeName;
            if (type == "string" && !string.IsNullOrWhiteSpace(common.FieldSize))
                result.AppendFormat("[System.ComponentModel.DataAnnotations.StringLength({0})]\r\n", common.FieldSize);
            if(!common.IsNullable )
                result.Append("[System.ComponentModel.DataAnnotations.Required()]\r\n");

            foreach (var r in (from x in row orderby x.Key descending select x )) {
                if (r.Key == 0) {
                    if (string.IsNullOrWhiteSpace(common.ForeignKey)) {
                        result.AppendFormat("public {1} {0} {{ get {{ return original.{0}; }} set {{ RaiseChanged(original.{0}, value, v => original.{0} = v, \"{0}\"); }} }}\r\n", r.Value, type);
                    } else {
                        var refType = GetNamespace() + "." + common.ForeignKey;
                        //result.AppendFormat("private {1} {0}Key {{ get {{ return original.{0}Key; }} set {{ RaiseChanged(original.{0}Key, value, v => original.{0}Key = v, \"{0}Key\"); }} }}\r\n", r.Value, type);
                        result.AppendFormat("public {1} {0} {{ get {{ return original.{0}; }} set {{ RaiseChanged(original.{0}, value, v => original.{0} = v, \"{0}\"); }} }}\r\n", row[0].Value, refType);
                    }
                } else if (r.Key == 1)
                    result.AppendFormat("[System.ComponentModel.DisplayName(\"{0}\")]\r\n", r.Value);
            }
            
            result.AppendLine();

            row.Clear();
        }

        protected override void CreateConstructor() {
            result.AppendFormat(@"private readonly DAL.{0} original;
public {0}(DAL.{0} original){{
this.original = original;
}}
", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path));
        }

        protected override string GetResult() {
            var codeLen = this.result.Length;
            var baseResult = base.GetResult();
            return baseResult.Substring(0, codeLen) + @"        // INotifyPropertyChanged
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        private void RaiseChanged<T>(T current, T newValue, Action<T> setter, string propName) {
            if (object.Equals(current, newValue))
                return;
            setter(newValue);
            if (PropertyChanged != null) PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName));
        }" +
          baseResult.Substring(codeLen);
        }

        //public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        //void x() {
        //    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(""));
        //}
    }
}
