using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;
using Newtonsoft.Json;
using System.IO;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public class ObjectDeserializationViewModel : DomainObjectViewModel
    {
        public ObjectDeserializationViewModel(ParameterDescriptor parameter,
                                              IFileLoader fileLoader,
                                              IFileSaver fileSaver) : base(parameter,
                                                                           fileLoader,
                                                                           fileSaver)
        {

        }

        private string _filename;

        public string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
                VerifyFileExistance();
            }
        }

        private string _format = "JSON";

        public string Format
        {
            get
            {
                return _format;
            }

            set
            {
                _format = value;
            }
        }

        private void VerifyFileExistance()
        {
            if(File.Exists(Filename))
            {
                _readyToRunTransformation = true;
            }
            else
            {
                _readyToRunTransformation = false;
            }
            RaisePropertyChanged(nameof(ReadyToRunTransformation));
        }

        private void InitializeInputData()
        {
            Parameter.ParameterInstance = null;

            if (!string.IsNullOrEmpty(_filename))
            {
                if (File.Exists(_filename))
                {
                    if (_format == "JSON")
                    {
                        try
                        {
                            string json = File.ReadAllText(_filename);

                            Parameter.ParameterInstance = JsonConvert.DeserializeObject(json, Parameter.DotNetType);

                            _readyToRunTransformation = true;
                        }
                        catch
                        {
                            Parameter.ParameterInstance = null;

                            _readyToRunTransformation = false;
                        }
                    }
                }
            }


            RaisePropertyChanged(nameof(ReadyToRunTransformation));
        }

        private bool _readyToRunTransformation = false;

        public override bool ReadyToRunTransformation
        {
            get
            {
                return _readyToRunTransformation;
            }
        }

        public override void InitializeDomainObject()
        {
            InitializeInputData();
        }

        public override void ProcessTransformationResult()
        {
            ;
        }
    }
}
