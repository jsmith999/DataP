using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IoPath = System.IO.Path;

namespace DataPath.Generators {
    class DalGenerator : CodeGen {

        protected override void StartGenerate(CommonDecl common) {
            if (!string.IsNullOrWhiteSpace(this.Path))
                result.AppendFormat(@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace {0} {{
", GetNamespace());

            result.AppendFormat(@"public class {0} {{
        public static Dictionary<int, {0}> Data = new Dictionary<int, {0}>();
", string.IsNullOrWhiteSpace(this.Path) ? ContainerName : IoPath.GetFileName(this.Path));
            CreateConstructor();
        }
    }
}
