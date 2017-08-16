using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IoPath = System.IO.Path;

namespace DataPath.Generators {
    class DalGenerator : CodeGen {

        protected override void StartGenerate(CommonDecl common) {
            if (!string.IsNullOrWhiteSpace(this.Path))
                result.Append(@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace  Conta.DAL.Model {
");

            result.AppendFormat(@"public partial class {0} {{
", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : IoPath.GetFileName(this.Path));
            CreateConstructor();
        }
    }
}
