using DataPath.Generators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DataPath {
    class SheetParser {
        private XmlNode wkSheet;
        private XmlNamespaceManager ns;

        private string worksheetName;
        private CommonDecl commonDecl;
        private List<IGenerator> actions = new List<IGenerator>();

        public SheetParser(XmlNode wkSheet, XmlNamespaceManager ns) {
            this.wkSheet = wkSheet;
            this.ns = ns;
        }

        public void Generate() {
            worksheetName = wkSheet.SelectSingleNode("@ss:Name", ns).InnerText;
            Debug.WriteLine("sheet " + worksheetName);

            //CommonDecl commonDecl = null;
            //var actions = new List<IGenerator>();
            ReadRow(wkSheet.SelectNodes("x:Table/x:Row[1]/x:Cell", ns), ReadHeaders);
            ReadRow(wkSheet.SelectNodes("x:Table/x:Row[2]/x:Cell", ns), ReadPaths);

            // TODO : rework
            if (commonDecl == null)
                return;     // TODO : throw

            // gen columns / properties
            foreach (XmlNode row in wkSheet.SelectNodes("x:Table/x:Row[position() > 2]", ns)) {
                int index = 0;
                int actionIndex = 0;
                foreach (XmlNode cell in row.SelectNodes("x:Cell", ns)) {
                    var indexNode = cell.SelectSingleNode("@ss:Index", ns);
                    if (indexNode != null && !string.IsNullOrWhiteSpace(indexNode.InnerText))
                        index = int.Parse(indexNode.InnerText);
                    else
                        index++;

                    if (actionIndex < actions.Count - 1) {
                        if (actions[actionIndex + 1].StartCol <= index) actionIndex++;
                    }

                    actions[actionIndex].AddValue(index - actions[actionIndex].StartCol, cell.SelectSingleNode("ss:Data", ns).InnerText);
                }

                foreach (var action in actions)
                    action.Generate(commonDecl);
            }

            foreach (var action in actions)
                action.Save();
        }

        private bool ReadPaths(int index, XmlNode col) {
            foreach (var action in actions)
                if (action.StartCol >= index) {
                    action.Path = col.InnerText;
                    return true;
                }

            return false;
        }

        private bool ReadHeaders(int index, XmlNode actionNode) {
            var actionName = actionNode.SelectSingleNode("ss:Data", ns).InnerText;
            if (actionName == "common") actions.Add(commonDecl = new CommonDecl { StartCol = index, ContainerName = worksheetName, });
            else if (actionName == "table") actions.Add(new TableGenerator { StartCol = index, ContainerName = worksheetName, });
            else if (actionName == "dal") actions.Add(new DalGenerator { StartCol = index, ContainerName = worksheetName, });
            else if (actionName == "ui") actions.Add(new UIGenerator { StartCol = index, ContainerName = worksheetName, });
            else throw new Exception("unknown generator type " + actionName);
            return true;
        }

        private void ReadRow(XmlNodeList xmlRow, Func<int, XmlNode, bool> execute) {
            var index = 0;
            foreach (XmlNode actionNode in xmlRow) {
                var indexNode = actionNode.SelectSingleNode("@ss:Index", ns);
                if (indexNode != null && !string.IsNullOrWhiteSpace(indexNode.InnerText))
                    index=int.Parse(indexNode.InnerText);
                else
                    index++;

                if (!execute(index, actionNode))
                    break;
            }
        }
    }
}
