using SourceName.Utils.Constants;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class UnsupportedMediaTypeResponse : Response
    {
        public override GenericResponse CreateResponse(string exceptionMessage, object content = null, string requestedObjectIdentifier = null)
        {
            return new GenericResponse(StringUtils.DynamicUnsupportedMediaTypeError(exceptionMessage), content);
        }
    }
}
