using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;
using MDD4All.UI.DataModels.ErrorList;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                if (value != _filename)
                {
                    _filename = value;
                    CheckTransformationAbility();
                }
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

        private bool VerifyFileExistence()
        {
            bool result = false;
            if (File.Exists(Filename))
            {
               
                result = true;
            }
            
            return result;
        }

        private bool InitializeInputData()
        {
            bool result = false;
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

                            result = true;
                        }
                        catch
                        {
                            Parameter.ParameterInstance = null;

                            result = false;
                        }
                    }
                }
            }


            return result;
        }


        public override void CheckTransformationAbility()
        {
            bool fileExists = false;
            bool inputDataValid = false;

            Errors = new List<IErrorListElement>();

            ReadyToRunTransformation = false;

            if (VerifyFileExistence())
            {
                fileExists = true;

                ReadyToRunTransformation = true;

                if (InitializeInputData())
                {
                    inputDataValid = true;

                    ReadyToRunTransformation = true;

                }
                else
                {
                    ReadyToRunTransformation = false;
                }
            }

            if (!fileExists)
            {
                Errors.Add(new ErrorListElement
                {
                    Description = "Error.NoFileSelected",
                    LocalizationParameters = new object[]
                        {
                            Filename,
                            Parameter.DomainParameterType.ToString(),
                            Parameter.Name,
                            Parameter.ParameterType
                        },
                    EffectedElement = "Error.Element"
                });
            }
            else
            {
                if (!inputDataValid)
                {
                    Errors.Add(new ErrorListElement
                    {
                        Description = "Error.NoValidData",
                        LocalizationParameters = new object[] 
                        { 
                            Filename, 
                            Parameter.DomainParameterType.ToString(), 
                            Parameter.Name,
                            Parameter.ParameterType
                        },
                        EffectedElement = "Error.Element"
                    });
                }
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
