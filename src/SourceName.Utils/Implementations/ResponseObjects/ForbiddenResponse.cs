using SourceName.Utils.Constants;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class ForbiddenResponse : Response
    {
        public override GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null)
        {
            var response = new GenericResponse(StringUtils.GenericUnauthorizedException, content);
            // TODO: Logging - NOTE: make sure to get user info
            return response;
        }
    }
}
