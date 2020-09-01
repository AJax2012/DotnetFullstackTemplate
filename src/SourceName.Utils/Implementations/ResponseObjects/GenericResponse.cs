using System.Collections.Generic;
using System.Linq;

namespace SourceName.Utils.Implementations.ResponseObjects
{
    public class GenericResponse
    {
        public int? ResponseCode { get; set; }
        public ICollection<string> Errors => new List<string>();
        public bool IsSuccessful => !Errors.Any();
        public object Content { get; set; }

        public GenericResponse() {}

        public GenericResponse(string error, object content = null)
        {
            Errors.Add(error);
            Content = content;
        }
    }
}
