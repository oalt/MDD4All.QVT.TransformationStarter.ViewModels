using GalaSoft.MvvmLight;
using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public class TransformationViewModel : ViewModelBase
    {
        private IFileLoader _fileLoader;
        private IFileSaver _fileSaver;

        public TransformationViewModel(ITransformationDescriptor transformationDescriptor,
                                       IFileLoader fileLoader,
                                       IFileSaver fileSaver) 
        {
            TransformationDescriptor = transformationDescriptor;
            _fileLoader = fileLoader;
            _fileSaver = fileSaver;
        }


        public ITransformationDescriptor TransformationDescriptor { get; private set; }

        public string Name
        {
            get
            {
                string result = string.Empty;

                result = TransformationDescriptor.TransformationName;

                return result;
            }
        }

        public bool ReadyToRunTransformation
        {
            get
            {
                bool result = true;
                foreach(DomainObjectViewModel enforceDomainViewModel in EnforceViewModels)
                {
                    if (result == false)
                    {
                        break;
                    }
                    result &= enforceDomainViewModel.ReadyToRunTransformation;
                    
                }

                foreach (DomainObjectViewModel checkDomainViewModel in CheckOnlyViewModels)
                {
                    if (result == false)
                    {
                        break;
                    }
                    result &= checkDomainViewModel.ReadyToRunTransformation;
                }

                foreach (PrimitiveDomainObjectViewModel primitiveDomainViewModel in PrimitiveDomainViewModels)
                {
                    if (result == false)
                    {
                        break;
                    }
                    result &= primitiveDomainViewModel.ReadyToRunTransformation;
                }
                return result;
            }
        }

        public string TopRelationName
        {
            get
            {
                string result = string.Empty;
                result = TransformationDescriptor.TopRelationName;
                return result;
            }
        }

        private List<ParameterDescriptor> _checkOnlyParameters = null;

        public List<ParameterDescriptor> CheckOnlyParameters
        {
            get
            {
                if (_checkOnlyParameters == null)
                {
                    _checkOnlyParameters = new List<ParameterDescriptor>();
                    
                    List<ParameterDescriptor> checkOnlyParameters = TransformationDescriptor.Parameters.Where(parameter => parameter.DomainParameterType == DomainParameterType.CheckOnly).ToList();
                    _checkOnlyParameters.AddRange(checkOnlyParameters);
                    
                }
                return _checkOnlyParameters;
            }
        }

        List<DomainObjectViewModel> _checkOnlyViewModels = null;

        public List<DomainObjectViewModel> CheckOnlyViewModels
        {
            get
            {
                if(_checkOnlyViewModels == null)
                {
                    _checkOnlyViewModels = new List<DomainObjectViewModel>();

                    foreach (ParameterDescriptor parameterDescriptor in CheckOnlyParameters)
                    {
                        

                        if(parameterDescriptor.Namespace == "EA")
                        {
                            EaObjectSelectionViewModel eaObjectSelectionViewModel = new EaObjectSelectionViewModel(parameterDescriptor,
                                                                                                                   _fileLoader,
                                                                                                                   _fileSaver);

                            eaObjectSelectionViewModel.PropertyChanged += OnDomainViewModelPropertyChanged;

                            _checkOnlyViewModels.Add(eaObjectSelectionViewModel);
                        }
                        else
                        {
                            /// TODO
                        }
                        
                    }
                }
                return _checkOnlyViewModels;
            }
        }

        private List<ParameterDescriptor> _enforceParameters = null;

        public List<ParameterDescriptor> EnforceParameters
        {
            get
            {
                if (_enforceParameters == null)
                {
                    _enforceParameters = new List<ParameterDescriptor>();
                    
                        List<ParameterDescriptor> enforceParameters = TransformationDescriptor.Parameters.Where(parameter => parameter.DomainParameterType == DomainParameterType.Enforce).ToList();
                        _enforceParameters.AddRange(enforceParameters);
                    
                }
                return _enforceParameters;
            }
        }

        private List<ParameterDescriptor> _primitiveParameters = null;

        public List<ParameterDescriptor> PrimitiveParameters
        {
            get
            {
                if (_primitiveParameters == null)
                {
                    _primitiveParameters = new List<ParameterDescriptor>();
                    
                    List<ParameterDescriptor> primitiveParameters = TransformationDescriptor.Parameters.Where(parameter => parameter.DomainParameterType == DomainParameterType.Primitive).ToList();
                    _primitiveParameters.AddRange(primitiveParameters);
                    
                }
                return _primitiveParameters;
            }
        }

        private List<DomainObjectViewModel> _enforceViewModels = null;

        public List<DomainObjectViewModel> EnforceViewModels
        {
            get
            {
                if (_enforceViewModels == null)
                {
                    _enforceViewModels = new List<DomainObjectViewModel>();

                    foreach (ParameterDescriptor parameterDescriptor in EnforceParameters)
                    {
                        if (parameterDescriptor.Namespace == "EA")
                        {
                            EaObjectSelectionViewModel eaObjectSelectionViewModel = new EaObjectSelectionViewModel(parameterDescriptor,
                                                                                                                   _fileLoader,
                                                                                                                   _fileSaver);

                            eaObjectSelectionViewModel.PropertyChanged += OnDomainViewModelPropertyChanged;
                            _enforceViewModels.Add(eaObjectSelectionViewModel);
                        }
                        else
                        {
                            ObjectSerializationViewModel domainObjectViewModel = new ObjectSerializationViewModel(parameterDescriptor, 
                                                                                                                  _fileLoader,
                                                                                                                  _fileSaver);
                            domainObjectViewModel.PropertyChanged += OnDomainViewModelPropertyChanged;
                            _enforceViewModels.Add(domainObjectViewModel);
                        }
                        
                    }
                }
                return _enforceViewModels;
            }
        }

        

        private List<PrimitiveDomainObjectViewModel> _primitiveDomainViewModels = null;

        public List<PrimitiveDomainObjectViewModel> PrimitiveDomainViewModels
        {
            get
            {
                if(_primitiveDomainViewModels == null)
                {
                    _primitiveDomainViewModels= new List<PrimitiveDomainObjectViewModel>();

                    foreach(ParameterDescriptor parameterDescriptor in PrimitiveParameters)
                    {
                        PrimitiveDomainObjectViewModel domainObjectViewModel = new PrimitiveDomainObjectViewModel(parameterDescriptor,
                                                                                                                  _fileLoader,
                                                                                                                  _fileSaver);
                        
                        domainObjectViewModel.PropertyChanged += OnDomainViewModelPropertyChanged;
                        
                        _primitiveDomainViewModels.Add(domainObjectViewModel);
                        
                        
                    }
                }
                return _primitiveDomainViewModels;
            }
        }

        private void OnDomainViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ReadyToRunTransformation")
            {
                RaisePropertyChanged("ReadyToRunTransformation");
            }
        }


        public void InitializeDomainObjects()
        {
            foreach (DomainObjectViewModel domainObjectViewModel in CheckOnlyViewModels)
            {
                domainObjectViewModel.InitializeDomainObject();
            }

            foreach(DomainObjectViewModel domainObjectViewModel in EnforceViewModels)
            {
                domainObjectViewModel.InitializeDomainObject();
            }

            foreach(DomainObjectViewModel domainObjectViewModel in PrimitiveDomainViewModels)
            {
                domainObjectViewModel.InitializeDomainObject();
            }
        }

        public void ProcessTransformationResults()
        {
            foreach (DomainObjectViewModel domainObjectViewModel in CheckOnlyViewModels)
            {
                domainObjectViewModel.ProcessTransformationResult();
            }

            foreach (DomainObjectViewModel domainObjectViewModel in EnforceViewModels)
            {
                domainObjectViewModel.ProcessTransformationResult();
            }

            foreach (DomainObjectViewModel domainObjectViewModel in PrimitiveDomainViewModels)
            {
                domainObjectViewModel.ProcessTransformationResult();
            }
        }

    }
}
