using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPath.Generators
{
    class CommonDecl : BaseGenerator
    {
        public enum Attributes
        {
            Type = 0,
            Size,
            Nullable,   // empty = non nullable
            ForeignKey,
            PrimaryKey,
        }

        private bool toClear;

        //[System.ComponentModel.DataAnnotations.Required()]
        public string TypeName { get { return GetIndexValue(Attributes.Type); } }
        public string FieldSize { get { return GetIndexValue(Attributes.Size); } }
        public bool IsNullable { get { return !string.IsNullOrEmpty(GetIndexValue(Attributes.Nullable)); } }
        public string ForeignKey { get { return GetIndexValue(Attributes.ForeignKey); } }
        public string PrimaryKey { get { return GetIndexValue(Attributes.PrimaryKey); } }

        private string GetIndexValue(Attributes att)
        {
            foreach (var x in row)
                if (x.Key == (int)att)
                    return x.Value;

            return string.Empty;
        }

        protected override string GetResult() { return string.Empty; }

        protected override void GenerateLine(CommonDecl common)
        {
            toClear = true;
            // do NOT call base.Generate()
        }

        public override void AddValue(int index, string value)
        {
            if (toClear)
            {
                row.Clear();
                toClear = false;
            }

            base.AddValue(index, value);
        }
    }
}
