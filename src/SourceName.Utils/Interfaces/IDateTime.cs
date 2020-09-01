using System;

namespace SourceName.Utils.Interfaces
{
    public interface IDateTime
    {
        public DateTime Now { get; }
        public DateTime UtcNow { get; }
        public string ToLongDateString();
        public string ToShortDateString();
        public string ToLongTimeString();
        public string ToShortTimeString();
        public string ToLongDateTimeString();
        public string ToShortDateTimeString();
    }
}
