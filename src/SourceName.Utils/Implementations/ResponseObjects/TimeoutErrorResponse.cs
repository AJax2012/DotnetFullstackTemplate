using SourceName.Utils.Constants;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class TimeoutErrorResponse : Response
    {
        public override GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null)
        {
            var response = new GenericResponse(StringUtils.GenericTimeoutException, content);
            // TODO: Logging - NOTE: Get as much detail as possible
            return response;
        }
    }
}
