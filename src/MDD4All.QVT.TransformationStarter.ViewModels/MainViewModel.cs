using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.EAFacade.ModelTree.ViewModels;
using MDD4All.FileAccess.Contracts;
using Microsoft.Extensions.Localization;
using System.Windows.Input;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IFileLoader _fileLoader;
        private IFileSaver _fileSaver;

        public MainViewModel(ITransformationDescriptor transformationDescriptor,
                             IFileLoader fileLoader,
                             IFileSaver fileSaver)
        {
            _fileLoader = fileLoader;
            _fileSaver = fileSaver;

            TransformationViewModel = new TransformationViewModel(transformationDescriptor, _fileLoader, _fileSaver);

            TransformationViewModel.PropertyChanged += OnTransformationViewModelPropertyChanged;

            InitializeCommands();

            TransformationViewModel.CheckTransformationAbility();
        }

        

        private void InitializeCommands()
        {
            RunTransformationCommand = new RelayCommand(ExecuteRunTransformation);
            ResetCommand = new RelayCommand(ExecuteResetCommand);
        }

        private void OnTransformationViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshStatus();
        }

        public RepositoryTreeViewModel RepositoryTreeViewModel { get; set; }

        private ViewState _activeViewState = ViewState.TransformationStart;

        public ViewState ActiveViewState
        {
            get
            {
                return _activeViewState;
            }
            set
            {
                _activeViewState = value;
                RaisePropertyChanged("ActiveViewState");
            }
        }

        private string _statusMessageTitle = "Status.Ready";

        public string StatusMessageTitle
        {
            get
            {
                return _statusMessageTitle;
            }

            set
            {
                _statusMessageTitle = value;
                RaisePropertyChanged(nameof(StatusMessageTitle));
            }
        }

        public TransformationViewModel TransformationViewModel { get; set; }

        public ICommand RunTransformationCommand { get; private set; }

        public ICommand ResetCommand { get; private set; }

        private void ExecuteRunTransformation()
        {
            TransformationViewModel.CheckTransformationAbility();
            
            if (TransformationViewModel.ReadyToRunTransformation)
            {
                TransformationViewModel.InitializeDomainObjects();

                ActiveViewState = ViewState.TransformationRunning;

                TransformationViewModel.TransformationDescriptor.TransformationStarter.StartTransformation();

                TransformationViewModel.ProcessTransformationResults();
                ActiveViewState = ViewState.TransformationFinished;
            }
        }

        private void RefreshStatus()
        {
            if (TransformationViewModel.ReadyToRunTransformation)
            {
                StatusMessageTitle = "Status.Ready";
            }
            else
            {
                StatusMessageTitle = "Status.Error";
            }
        }

        private void ExecuteResetCommand()
        {
            ActiveViewState = ViewState.TransformationStart;
        }
    }
}
