using Microsoft.Extensions.Logging;

using PostSharp.Aspects;
using PostSharp.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    [PSerializable]
    public class VstoUnhandledExceptionAttribute : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            ExcelAddin.Globals.ThisAddIn.Logger.LogCritical(args.Exception, "Premium sync");
            args.FlowBehavior = FlowBehavior.Return;
        }
    }
}
