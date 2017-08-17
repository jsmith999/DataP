using System;
using System.IO;
using System.Text;

namespace DataPath.Generators {
    class XmlTableGenerator : BaseGenerator {
        enum Columns {
            Name,
            Type,
            TypeInfo1,  // size / autoinc / ...?
            TypeInfo2,  // precision / ...?
        }

        #region XML parts
        private const string  FilePrefixFormat = @"<?xml version='1.0' standalone='yes'?>
<NewDataSet>
  <xs:schema id='NewDataSet' xmlns='' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
    <xs:element name='NewDataSet' msdata:IsDataSet='true' msdata:MainDataTable='Client' msdata:UseCurrentLocale='true'>
      <xs:complexType>
        <xs:choice minOccurs='0' maxOccurs='unbounded'>
          <xs:element name='{0}'>
            <xs:complexType>
              <xs:sequence>
";
        private const string FileSuffix = @"              </xs:sequence>
            </xs:complexType>
          </xs:element>
        </xs:choice>
      </xs:complexType>
    </xs:element>
  </xs:schema>
</NewDataSet>
";
        #endregion

        private StringBuilder result = new StringBuilder();

        protected override void StartGenerate(CommonDecl common) {
            result.AppendFormat(FilePrefixFormat, string.IsNullOrWhiteSpace(this.Path) ? ContainerName : System.IO.Path.GetFileName(this.Path));
            base.StartGenerate(common);
        }


        #region Generate line
        protected override void GenerateLine(CommonDecl common) {
            if (row.Count <= (int)Columns.Name)
                return;     // calculated column

            var type = GetColumnType(common);
            //var foreignKey = GetForeignKey(common);

            result.AppendFormat("<xs:element name='{0}'{1}\r\n", row[(int)Columns.Name].Value, type);

            //?var foreignKey = GetForeignKey(common);

            //var pk = common.PrimaryKey;
            // TODO : FK
            base.GenerateLine(common);
        }

        private string GetColumnType(CommonDecl common) {
            if (string.IsNullOrWhiteSpace(common.TypeName)) throw new Exception("Missing column type");

            var typeName = common.TypeName.Trim().ToLower();
            // check typeName
            if (typeName == "string") {
                // check row[2] exists 
                return string.Format(@">
                  <xs:simpleType>
                    <xs:restriction base='xs:string'>
                      <xs:maxLength value='{0}' />
                    </xs:restriction>
                  </xs:simpleType>
                </xs:element>", common.FieldSize );
            } else if (typeName == "int") {
                if (row.Count > (int)Columns.TypeInfo1 && row[(int)Columns.TypeInfo1].Value == "autoincrement")
                    return " msdata:ReadOnly='true' msdata:AutoIncrement='true' type='xs:int' />";

                return " type='xs:int'/>";
            } else if (typeName == "datetime") {
                return " type='xs:dateTime' />";
            } else if (typeName == "float") {
                return " type='xs:float'/>";      // TODO : decimals
            } else if (typeName == "bool") {
                return " type='xs:boolean'/>"; ;
            } else if (typeName == "byte[]") {
                return " type='xs:byte[]'/>";
            }

            throw new Exception("Unknown column type : " + typeName);
        }
        #endregion

        protected override string GetResult() {
            return result.ToString() + FileSuffix;
        }

        public override void Save() {

            var path = this.Path.ToUpper() + ".xml";
            using (var writer = new StreamWriter(path))
                writer.Write(this.Code);
        }
    }
}
