using SourceName.Utils.Interfaces;
using System;

namespace SourceName.Utils.Implementations
{
    public class DateTimeProvider : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;

        public string ToShortDateString() => Now.ToShortDateString();
        public string ToLongDateString() => Now.ToShortDateString();

        public string ToShortTimeString() => Now.ToShortTimeString();
        public string ToLongTimeString() => Now.ToLongTimeString();

        public string ToShortDateTimeString() => ToShortDateString() + " " + ToShortTimeString();
        public string ToLongDateTimeString() => ToLongDateString() + " " + ToLongTimeString();
    }
}
