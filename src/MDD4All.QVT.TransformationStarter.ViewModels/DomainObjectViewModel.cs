using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;
using MDD4All.UI.DataModels.ErrorList;
using System.Collections.Generic;
using System.Windows.Input;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public abstract class DomainObjectViewModel : ViewModelBase, IErrorList
    {
        
        protected ITransformationConfiguration _configuration;
        protected IFileLoader _fileLoader;
        protected IFileSaver _fileSaver;

        public DomainObjectViewModel(ParameterDescriptor parameter, 
                                     ITransformationConfiguration configuration,
                                     IFileLoader fileLoader,
                                     IFileSaver fileSaver) 
        {
            Parameter = parameter;
            _configuration = configuration;
            _fileLoader = fileLoader;
            _fileSaver = fileSaver;
            
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SelectFileToLoadCommand = new RelayCommand<string>(ExecuteSelectFileToLoad);
            SelectFileToSaveCommand = new RelayCommand<string>(ExecuteSelectFileToSave);
        }

        public ParameterDescriptor Parameter { get; private set; }

        public string Title
        {
            get
            {
                string result = string.Empty;
                switch(Parameter.DomainParameterType)
                {
                    case DomainParameterType.CheckOnly:
                        result += "Check only domain";
                        break;
                    case DomainParameterType.Enforce:
                        result += "Enforce domain";
                        break;
                    case DomainParameterType.Primitive:
                        result += "Primitve domain";
                        break;
                }

                result += " for ";

                result += Parameter.Name + " :" + Parameter.ParameterType;

                return result;
            }
        }

        public string DomainLocalizeTitle
        {
            get
            {
                string result = string.Empty;
                switch (Parameter.DomainParameterType)
                {
                    case DomainParameterType.CheckOnly:
                        result = "Label.DomainCheckOnly";
                        break;
                    case DomainParameterType.Enforce:
                        result += "Label.DomainEnforce";
                        break;
                    case DomainParameterType.Primitive:
                        result += "Label.DomainPrimitive";
                        break;
                }
                return result;
            }
        }

        public string Name
        {
            get
            {
                return Parameter.Name;
            }
        }

        public string Namespace
        {
            get
            {
                return Parameter.Namespace;
            }
        }

        private bool _readyToRunTransformation = false;

        public virtual bool ReadyToRunTransformation 
        { 
            get
            {
                return _readyToRunTransformation;
            }
            protected set
            {
                _readyToRunTransformation = value;
                RaisePropertyChanged("ReadyToRunTransformation");
            }
        }

        public abstract void CheckTransformationAbility();

        public abstract void InitializeDomainObject();

        public abstract void ProcessTransformationResult();

        public string SelectedFilename { get; set; } = string.Empty;

        public bool FileSelectionResult { get; set; } = false;

        public ICommand SelectFileToLoadCommand { get; private set; }

        public ICommand SelectFileToSaveCommand { get; private set;}

        public List<IErrorListElement> Errors { get; set; } = new List<IErrorListElement>();

        public bool ShowErrorCode { get; set; } = false;

        private void ExecuteSelectFileToLoad(string title)
        {
            string selectedFile = string.Empty;
            bool openResult = _fileLoader.ShowOpenFileDialog(out selectedFile, title: title);

            if (openResult)
            {
                SelectedFilename = selectedFile;
                FileSelectionResult = true;
            }
            else
            {
                SelectedFilename = string.Empty;
                FileSelectionResult = false;
            }
            RaisePropertyChanged("FileSelectionResult");
        }

        private void ExecuteSelectFileToSave(string title)
        {
            string selectedFile = string.Empty;
            bool openResult = _fileSaver.ShowFileSaveDialog(out selectedFile, title: title);

            if (openResult)
            {
                SelectedFilename = selectedFile;
                FileSelectionResult = true;
            }
            else
            {
                SelectedFilename = string.Empty;
                FileSelectionResult = false;
            }
            RaisePropertyChanged("FileSelectionResult");
            RaisePropertyChanged("ReadyToRunTransformation");
        }
    }
}
