using GalaSoft.MvvmLight.Command;
using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.EAFacade.DataAccess.Cached;
using MDD4All.EAFacade.DataModels.Contracts;
using MDD4All.EAFacade.DataModels.Contracts.Extensions;
using MDD4All.EAFacade.ModelTree.ViewModels;
using MDD4All.FileAccess.Contracts;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public class EaObjectSelectionViewModel : DomainObjectViewModel
    {
        public EaObjectSelectionViewModel(ParameterDescriptor parameter,
                                          IFileLoader fileLoader,
                                          IFileSaver fileSaver) : base(parameter,
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

        public string ConnectionString { get; set; } = string.Empty;

        public RepositoryObjectDescriptor SelectedObject { get; set; } = null;

        public bool ShowEaElementSelectionDialog { get; set; } = false;

        public EA.Repository EaRepository { get; set; }

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

        private async Task OpenAndCacheEaModelAsync()
        {
            Task result = null;

            await Task.Run(() =>
            {
                string progId = "EA.Repository";
                Type type = Type.GetTypeFromProgID(progId);
                EaRepository = Activator.CreateInstance(type) as EA.Repository;

                bool openResult = EaRepository.OpenFile(ConnectionString);

                if (openResult)
                {
                    EaRepository.ShowWindow(1);


                    CachedRepository cachedRepository = new CachedRepository(EaRepository);
                    cachedRepository.CacheAll();



                    if (Parameter.ParameterType == "Package")
                    {
                        RepositoryTreeViewModel = new RepositoryTreeViewModel(cachedRepository, false, false, false);

                    }
                    else if (Parameter.ParameterType == "Element")
                    {
                        RepositoryTreeViewModel = new RepositoryTreeViewModel(cachedRepository, true, true, false);
                    }
                    else if (Parameter.ParameterType == "Diagram")
                    {
                        RepositoryTreeViewModel = new RepositoryTreeViewModel(cachedRepository, false, false, true);
                    }
                    RaisePropertyChanged("RepositoryTreeViewModel");
                }
            }
                );


        }

        private bool CanExecuteShowSelectionDialogCommand()
        {
            return !string.IsNullOrEmpty(ConnectionString);
        }

        private void ExecuteCloseSelectionDialog()
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

            if (EaRepository != null)
            {
                EaRepository.Exit();
            }

            ShowEaElementSelectionDialog = false;
        }

        public override void InitializeDomainObject()
        {
           

            string progId = "EA.Repository";
            Type type = Type.GetTypeFromProgID(progId);
            EaRepository = Activator.CreateInstance(type) as EA.Repository;

            bool openResult = EaRepository.OpenFile(SelectedObject.ConnectionString);

            if (openResult)
            {
                EaRepository.ShowWindow(1);
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
                EaRepository.Exit();
            }
        }
    }
}
