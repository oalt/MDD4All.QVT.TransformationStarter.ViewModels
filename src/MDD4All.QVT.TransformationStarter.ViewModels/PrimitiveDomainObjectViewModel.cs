using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;
using MDD4All.UI.DataModels.ErrorList;
using System.Collections.Generic;

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

        

        public override void CheckTransformationAbility()
        {
            Errors = new List<IErrorListElement>();
            ReadyToRunTransformation = true;
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
