namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class SuccessResponse : Response
    {
        public override GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null)
        {
            var response = new GenericResponse(string.Empty, content);
            return response;
        }
    }
}
