using LL.MDE.Components.Qvt.Common.DataModels;
using MDD4All.FileAccess.Contracts;
using MDD4All.UI.DataModels.ErrorList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MDD4All.QVT.TransformationStarter.ViewModels
{
    public class ObjectSerializationViewModel : DomainObjectViewModel
    {
        public ObjectSerializationViewModel(ParameterDescriptor parameter,
                                            ITransformationConfiguration configuration,
                                            IFileLoader fileLoader,
                                            IFileSaver fileSaver) : base(parameter,
                                                                         configuration,
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
                CheckTransformationAbility();
            }
        }

        public string Format { get; set; } = "JSON";

        public override void InitializeDomainObject()
        {
            Parameter.SerializationFilename = Filename;

            object domainObject = Activator.CreateInstance(Parameter.DotNetType);
            Parameter.ParameterInstance = domainObject;
        }

        public override void ProcessTransformationResult()
        {
            if (Format == "JSON")
            {
                Type type = Parameter.ParameterInstance.GetType();
                _configuration.JsonSerializer.SerializeToJsonFile(Parameter.SerializationFilename, type, Parameter.ParameterInstance);
            }
            else if (Format == "XML")
            {
                Type type = Parameter.ParameterInstance.GetType();
                XmlSerializer serializer = new XmlSerializer(type);

                TextWriter writer = new StreamWriter(Parameter.SerializationFilename);

                serializer.Serialize(writer, Parameter.ParameterInstance);
                writer.Close();
            }
        }

        public override void CheckTransformationAbility()
        {
            Errors = new List<IErrorListElement>();

            if (!string.IsNullOrEmpty(Filename))
            {
                ReadyToRunTransformation = true;

            }
            else
            {
                ReadyToRunTransformation = false;
                Errors.Add(new ErrorListElement
                {
                    Description = "Error.NoFileSelected",
                    EffectedElement = "Error.Element",
                    LocalizationParameters = new object[]
                                    {
                                        Filename,
                                        Parameter.DomainParameterType.ToString(),
                                        Parameter.Name,
                                        Parameter.ParameterType
                                    }
                });
            }
        }
    }
}
