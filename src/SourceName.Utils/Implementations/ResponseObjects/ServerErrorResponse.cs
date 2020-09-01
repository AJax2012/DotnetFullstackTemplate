using SourceName.Utils.Constants;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class ServerErrorResponse : Response
    {
        public override GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null)
        {
            var response = new GenericResponse(StringUtils.GenericServerError, content);
            // TODO: Logging
            return response;        
        }
    }
}
