using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;
using Newtonsoft.Json;
using System;
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
                VerifyDataReadability();
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
                VerifyDataReadability();
            }
        }

        private void VerifyDataReadability()
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
                        }
                        catch
                        {
                            Parameter.ParameterInstance = null;
                        }
                    }
                }
            }


            RaisePropertyChanged(nameof(ReadyToRunTransformation));
        }

        public override bool ReadyToRunTransformation
        {
            get
            {
                return Parameter.ParameterInstance != null;
            }
        }

        public override void InitializeDomainObject()
        {
            ;
        }

        public override void ProcessTransformationResult()
        {
            ;
        }
    }
}
