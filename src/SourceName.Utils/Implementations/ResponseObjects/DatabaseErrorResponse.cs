using SourceName.Utils.Constants;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class DatabaseErrorResponse : Response
    {
        public override GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null)
        {
            var response = new GenericResponse(StringUtils.GenericDatabaseError, content);
            // TODO ADD LOGGING
            return response;
        }
    }
}
