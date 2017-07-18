using DataPath.Generators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace DataPath {
    class Program {
        static void Main(string[] args) {
            Generate(args[0]);
            //Tests.Example.Run();
        }

        private static void Generate(string source) {
            var doc = new XmlDocument();
            doc.Load(source);
            var ns = new XmlNamespaceManager(doc.NameTable);
            ns.AddNamespace("x", "urn:schemas-microsoft-com:office:spreadsheet");
            ns.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
            // Debug.WriteLine("root " + doc.FirstChild.Name);
            foreach (XmlNode wkSheet in doc.SelectNodes("x:Workbook/x:Worksheet", ns))
                new SheetParser(wkSheet, ns).Generate();
        }
    }
}
