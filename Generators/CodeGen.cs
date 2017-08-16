using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IoPath = System.IO.Path;

namespace DataPath.Generators {
    class CodeGen : BaseGenerator {

        protected StringBuilder result = new StringBuilder();

        public override void Save() {
            var path = IoPath.GetDirectoryName(this.Path);
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path)) Directory.CreateDirectory(path);

            path = this.Path;
            if (string.IsNullOrEmpty(IoPath.GetExtension(this.Path))) path += ".cs";

            using (var writer = new StreamWriter(path))
                writer.Write(this.Code);
        }

        protected override void StartGenerate(CommonDecl common) {
            base.StartGenerate(common);

            if (!string.IsNullOrWhiteSpace(this.Path))
                result.AppendFormat("using System;\r\n\r\nnamespace {0} {{\r\n", GetNamespace());

            result.AppendFormat("public partial class {0}{{\r\n", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : IoPath.GetFileName(this.Path));
            CreateConstructor();
        }

        //private string GetNamespace() {
        //    return IoPath.GetDirectoryName(this.Path).Replace("\\", ".").Replace("/", ".");
        //}

        protected override void GenerateLine(CommonDecl common) {
            if (row.Count != 0) {
                var type = common.TypeName;
                if (string.IsNullOrWhiteSpace(common.ForeignKey)) {
                    if (!string.IsNullOrEmpty(common.PrimaryKey))
                        result.AppendLine("[Key]");
                    result.AppendFormat("public {1} {0} {{ get; set;}}\r\n", row[0].Value, type);
                } else {
                    var refType = GetNamespace() + "." + common.ForeignKey;
                    result.AppendFormat(@"private {1} {0}Key {{ get; set;}}
public {2} {0} {{ get; set;}}
", row[0].Value, type, refType);
                }
            }

            base.GenerateLine(common);
        }

        protected override string GetResult() {
            //result.AppendLine("}");
            //if (!string.IsNullOrWhiteSpace(this.Path))
            //    result.AppendLine("}");

            return result.ToString() + "}\r\n" + (string.IsNullOrWhiteSpace(this.Path) ? string.Empty : "}\r\n");
        }

        protected virtual void CreateConstructor() {
            result.AppendFormat(@"public {0}(){{ }}
", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path));
        }
    }
}
