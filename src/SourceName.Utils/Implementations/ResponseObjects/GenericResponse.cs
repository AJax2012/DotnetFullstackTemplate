using SourceName.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class GenericResponse
    {
        public int? ResponseCode { get; set; }
        public ICollection<string> Errors => new List<string>();
        public bool IsSuccessful => !Errors.Any();
        public object Content { get; set; }

        public GenericResponse() {}

        public GenericResponse(string error, object content = null)
        {
            Errors.Add(error);
            Content = content;
        }
    }

    public class GenericResponseFactory : IGenericResponseFactory
    {
        public GenericResponse Create(ResponseType responseCode, string exceptionMessage = null, object content = null, string requestedObjectIdentifier = null)
        {
            var namespaceString = typeof(GenericResponseFactory).Namespace;

            try
            {
                return (GenericResponse)Activator.CreateInstance(
                        Type.GetType($"{namespaceString}.{responseCode}Response"),
                        new object[] { exceptionMessage, content, requestedObjectIdentifier });
            }
            catch
            {
                return null;
            }
        }
    }
}
