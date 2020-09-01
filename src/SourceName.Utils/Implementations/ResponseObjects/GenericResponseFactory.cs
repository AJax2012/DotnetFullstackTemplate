using SourceName.Utils.Interfaces;
using System;

namespace SourceName.Utils.Implementations.ResponseObjects
{
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
