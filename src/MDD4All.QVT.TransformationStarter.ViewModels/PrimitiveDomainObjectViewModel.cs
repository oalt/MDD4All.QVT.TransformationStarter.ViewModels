using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public class PrimitiveDomainObjectViewModel : DomainObjectViewModel
    {
        public PrimitiveDomainObjectViewModel(ParameterDescriptor parameter, 
                                              IFileLoader fileLoader, 
                                              IFileSaver fileSaver) : base(parameter,
                                                                           fileLoader, 
                                                                           fileSaver)
        {
        }

        // TODO: Check validity of data
        public override bool ReadyToRunTransformation
        {
            get
            {
                return true;
            }
        }

        public override void InitializeDomainObject()
        {
            if(Parameter.ParameterType.ToLower() == "string")
            {
                Parameter.ParameterInstance = "";
            }
        }

        public override void ProcessTransformationResult()
        {
            ;
        }
    }
}
