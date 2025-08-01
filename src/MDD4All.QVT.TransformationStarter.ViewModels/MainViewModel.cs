using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.EAFacade.ModelTree.ViewModels;
using MDD4All.FileAccess.Contracts;
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

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RunTransformationCommand = new RelayCommand(ExecuteRunTransformation);
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

        public string StatusMessage { get; set; } = "Ready";

        public TransformationViewModel TransformationViewModel { get; set; }

        public ICommand RunTransformationCommand { get; private set; }

        

        private void ExecuteRunTransformation()
        {
            ActiveViewState = ViewState.TransformationRunning;

            TransformationViewModel.InitializeDomainObjects();

            TransformationViewModel.TransformationDescriptor.TransformationStarter.StartTransformation();

            TransformationViewModel.ProcessTransformationResults();
        }
    }
}
