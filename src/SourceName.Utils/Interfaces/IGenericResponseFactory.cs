using SourceName.Utils.Implementations.ResponseObjects;

namespace SourceName.Utils.Interfaces
{
    public interface IGenericResponseFactory
    {
        GenericResponse Create(ResponseType responseCode, string exceptionMessage = null, object content = null, string requestedObjectIdentifier = null);
    }
}
