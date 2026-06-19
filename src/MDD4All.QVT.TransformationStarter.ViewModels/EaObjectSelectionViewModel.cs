using GalaSoft.MvvmLight.Command;
using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.EAFacade.DataAccess.Cached;
using MDD4All.EAFacade.DataModels.Contracts;
using MDD4All.EAFacade.DataModels.Contracts.Extensions;
using MDD4All.EAFacade.ModelTree.ViewModels;
using MDD4All.FileAccess.Contracts;
using MDD4All.UI.DataModels.ErrorList;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public class EaObjectSelectionViewModel : DomainObjectViewModel
    {
        public EaObjectSelectionViewModel(ParameterDescriptor parameter,
                                          ITransformationConfiguration transformationConfiguration,
                                          IFileLoader fileLoader,
                                          IFileSaver fileSaver) : base(parameter,
                                                                       transformationConfiguration,
                                                                       fileLoader,
                                                                       fileSaver)
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ShowSelectionDialogCommand = new RelayCommand(ExecuteShowSelectionDialogCommand, CanExecuteShowSelectionDialogCommand);
            CloseSelectionDialogCommand = new RelayCommand(ExecuteCloseSelectionDialog);
        }

        private string _connectionString = string.Empty;

        public string ConnectionString 
        {
            get
            {
                return _connectionString;
            }
            set 
            {
                if (value != _connectionString)
                {
                    if(EaRepository != null)
                    {
                        EaRepository.Exit();
                        EaRepository = null;
                    }
                    _connectionString = value;
                }
            } 
        }

        public RepositoryObjectDescriptor SelectedObject { get; set; } = null;

        public bool ShowEaElementSelectionDialog { get; set; } = false;

        public EA.Repository EaRepository { get; set; }

        public bool KeepModelOpen { get; set; } = true;

        public bool ModelOpenErrorOccured { get; set; } = false;

        public RepositoryTreeViewModel RepositoryTreeViewModel { get; set; }


        #region COMMAND_DEFINITIONS
        public ICommand ShowSelectionDialogCommand { get; private set; }

        public ICommand CloseSelectionDialogCommand { get; private set; }

        #endregion

        private void ExecuteShowSelectionDialogCommand()
        {
            RepositoryTreeViewModel = null;
            ShowEaElementSelectionDialog = true;
            Task.Run(OpenAndCacheEaModelAsync);
        }

        private CachedRepository _cachedRepository = null;

        private async Task OpenAndCacheEaModelAsync()
        {
            Task result = null;

            await Task.Run(() =>
            {
                try
                {
                    if (EaRepository == null)
                    {
                        string progId = "EA.Repository";
                        Type type = Type.GetTypeFromProgID(progId);
                        EaRepository = Activator.CreateInstance(type) as EA.Repository;

                        bool openResult = EaRepository.OpenFile(ConnectionString);

                        if (openResult)
                        {
                            EaRepository.ShowWindow(1);


                            _cachedRepository = new CachedRepository(EaRepository);
                            _cachedRepository.CacheAll();
                        }
                    }

                    if (_cachedRepository != null)
                    {
                        if (Parameter.ParameterType == "Package")
                        {
                            RepositoryTreeViewModel = new RepositoryTreeViewModel(_cachedRepository, false, false, false);
                        }
                        else if (Parameter.ParameterType == "Element")
                        {
                            RepositoryTreeViewModel = new RepositoryTreeViewModel(_cachedRepository, true, true, false);
                        }
                        else if (Parameter.ParameterType == "Diagram")
                        {
                            RepositoryTreeViewModel = new RepositoryTreeViewModel(_cachedRepository, false, false, true);
                        }
                    }
                }
                catch
                {
                    ModelOpenErrorOccured = true;
                    EaRepository = null;
                    
                }
                finally
                {
                    RaisePropertyChanged(nameof(RepositoryTreeViewModel));
                }
            });
        }

        private bool CanExecuteShowSelectionDialogCommand()
        {
            return !string.IsNullOrEmpty(ConnectionString);
        }

        private void ExecuteCloseSelectionDialog()
        {
            if (RepositoryTreeViewModel != null)
            {
                RepositoryObjectViewModel selectedObject = RepositoryTreeViewModel.SelectedRepositoryObject;

                if (selectedObject != null)
                {
                    SelectedObject = new RepositoryObjectDescriptor
                    {
                        ConnectionString = selectedObject.ConnectionString,
                        GUID = selectedObject.GUID,
                        Name = selectedObject.Name,
                        ObjectType = selectedObject.ObjectType,
                    };

                    ReadyToRunTransformation = true;
                }
            }
            else
            {
                ReadyToRunTransformation = false;
            }

            ModelOpenErrorOccured = false;
            ShowEaElementSelectionDialog = false;
        }

        public override void CheckTransformationAbility()
        {
            if (SelectedObject != null)
            {
                ReadyToRunTransformation = true;
                Errors = new List<IErrorListElement>();
            }
        }

        public override void InitializeDomainObject()
        {

            if (EaRepository == null)
            {
                string progId = "EA.Repository";
                Type type = Type.GetTypeFromProgID(progId);
                EaRepository = Activator.CreateInstance(type) as EA.Repository;

                bool openResult = EaRepository.OpenFile(SelectedObject.ConnectionString);

                if (openResult)
                {
                    EaRepository.ShowWindow(1);
                }
            }

            if (SelectedObject.ObjectType == ObjectType.otPackage)
            {
                EA.Package package = EaRepository.GetPackageByGuid(SelectedObject.GUID);
                Parameter.ParameterInstance = package;
            }
            else if (SelectedObject.ObjectType == ObjectType.otElement)
            {
                EA.Element element = EaRepository.GetElementByGuid(SelectedObject.GUID);
                Parameter.ParameterInstance = element;
            }
            else if (SelectedObject.ObjectType == ObjectType.otDiagram)
            {
                EA.Diagram diagram = EaRepository.GetDiagramByGuid(SelectedObject.GUID) as EA.Diagram;
                Parameter.ParameterInstance = diagram;
            }
        }

        public override void ProcessTransformationResult()
        {
            if (EaRepository != null)
            {
                if(SelectedObject != null && SelectedObject.ObjectType == ObjectType.otPackage)
                {
                    EA.Package package = EaRepository.GetPackageByGuid(SelectedObject.GUID);
                    EaRepository.ReloadPackage(package.PackageID);
                }
                if (!KeepModelOpen)
                {
                    EaRepository.Exit();
                }
            }
        }

        
    }
}
