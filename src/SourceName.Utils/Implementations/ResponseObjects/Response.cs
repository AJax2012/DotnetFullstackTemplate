namespace SourceName.Utils.Implementations.ResponseObjects
{
    public abstract class Response
    {
        public abstract GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null);
    }
}
