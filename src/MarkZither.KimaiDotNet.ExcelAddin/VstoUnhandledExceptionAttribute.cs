using Microsoft.Extensions.Logging;

using PostSharp.Aspects;
using PostSharp.Serialization;

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
