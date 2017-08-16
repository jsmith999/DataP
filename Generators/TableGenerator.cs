using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace DataPath.Generators
{
    class TableGenerator : BaseGenerator
    {
        enum Columns
        {
            Name,
            Type,
            TypeInfo1,  // size / autoinc / ...?
            TypeInfo2,  // precision / ...?
        }

        private StringBuilder result = new StringBuilder();

        protected override void StartGenerate(CommonDecl common)
        {
            result.AppendFormat("create table {0} (\r\n", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path));
            base.StartGenerate(common);
        }

        #region Generate line
        protected override void GenerateLine(CommonDecl common)
        {
            if (row.Count <= (int)Columns.Name)
                return;     // calculated column

            var type = GetColumnType(common);
            //var foreignKey = GetForeignKey(common);

            result.AppendFormat("{0} {1},\r\n", row[(int)Columns.Name].Value, type);

            var foreignKey = GetForeignKey(common);

            //var pk = common.PrimaryKey;
            // TODO : FK
            base.GenerateLine(common);
        }

        private string GetColumnType(CommonDecl common)
        {
            if (string.IsNullOrWhiteSpace(common.TypeName)) throw new Exception("Missing column type");

            var typeName = common.TypeName.Trim().ToLower();
            // check typeName
            if (typeName == "string")
            {
                // check row[2] exists 
                return !string.IsNullOrWhiteSpace(common.FieldSize) && int.Parse(common.FieldSize) >= 65535 ? 
                    "clob" : 
                    ("varchar2(" + common.FieldSize + ")");
            }
            else if (typeName == "int")
            {
                if (row.Count > (int)Columns.TypeInfo1 && row[(int)Columns.TypeInfo1].Value == "autoincrement")
                    return "integer auto";

                return "integer";
            }
            else if (typeName == "datetime")
            {
                return "date";
            }
            else if (typeName == "float")
            {
                return "float";     // TODO : decimals
            }
            else if (typeName == "bool")
            {
                return "bool";
            }
            else if (typeName == "byte[]")
            {
                return "blob";
            }

            throw new Exception("Unknown column type : " + typeName);
        }

        private string GetForeignKey(CommonDecl common)
        {
            if (string.IsNullOrEmpty(common.ForeignKey))
            {
                return string.Empty;
            }
            else
            {
                result.AppendFormat("FOREIGN KEY( {0} ) REFERENCES {0}(id) ,\r\n", common.ForeignKey);

                return string.Empty;
            }
        }
        #endregion

        protected override string GetResult()
        {
            return result.ToString() + ");";
        }

        public override void Save()
        {

            var path = this.Path.ToUpper() +".sql"; /*IoPath.GetDirectoryName(this.Path);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = this.Path;
            if (string.IsNullOrEmpty(IoPath.GetExtension(this.Path))) path += ".cs";*/

            using (var writer = new StreamWriter(path))
                writer.Write(this.Code);
        }
    }
}
