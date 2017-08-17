using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPath.Generators {
    class ServiceHandlerGenerator : CodeGen {
        private StringBuilder dataToModel = new StringBuilder();
        private StringBuilder modelToData = new StringBuilder();
        private int rowNo;

        protected override void StartGenerate(CommonDecl common) {
            result.AppendLine(@"using System;
using System.Data;
using System.Diagnostics;
using Conta.DAL.Model;

namespace XmlDal.ServiceHandler {");

            this.Path = this.ContainerName;
        }

        protected override void GenerateLine(CommonDecl common) {
            if (row.Count == 0) return;
            var type = common.TypeName;

            if (!string.IsNullOrEmpty(common.PrimaryKey))
                result.AppendFormat(@"    class EmployeeServiceHandler : TableService<{0}, {1}> {{
        public EmployeeServiceHandler() {{
            TableName = ""{0}"";
            KeyName = ""{1}"";
        }}

        public override int GetKeyValue({0} item) {{ return item.{2}; }}

        protected override DataRow FindRow(DataTable table, {0} item) {{
            var results = table.Select(""{2} = "" + item.{2});
            Debug.Assert(results.Length <= 1);
            return results.Length == 0 ? null : results[0];
        }}
",
                this.Path,
                common.TypeName,
                row[0].Value);

            dataToModel.AppendFormat("item.{2} = ({0})row[{1}];\r\n",
                common.TypeName,
                rowNo,
                row[0].Value);
            modelToData.AppendFormat("{0}row[{1}]=item.{2};\r\n",
                string.IsNullOrEmpty(common.PrimaryKey) ? "" : "//",
                rowNo,
                row[0].Value);

            rowNo++;
            row.Clear();
        }

        protected override string GetResult() {
            result.AppendFormat("        protected override void DataToModel({0} item, DataRow row) {{\r\n", this.Path);
            result.AppendLine(dataToModel.ToString());
            result.AppendLine(@"        }

        protected override Employee DataToModel(DataRow row) {
            var result = new Employee();
            DataToModel(result, row);
            return result;
        }");

            result.AppendFormat("        protected override void DataToModel({0} item, DataRow row) {{\r\n",this.Path);
            result.AppendLine(modelToData.ToString());
            result.AppendLine(@"}
    }
}");

            return result.ToString();
        }
    }
}
