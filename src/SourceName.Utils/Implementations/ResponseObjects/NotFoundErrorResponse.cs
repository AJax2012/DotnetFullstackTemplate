using SourceName.Utils.Constants;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class NotFoundErrorResponse : Response
    {
        public override GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null)
        {
            var errorMessage = StringUtils.DynamicNotFoundError(requestedObjectIdentifier);
            var response = new GenericResponse(errorMessage);
            // TODO: Logging - record identifier and user identifier
            return response;
        }
    }
}
