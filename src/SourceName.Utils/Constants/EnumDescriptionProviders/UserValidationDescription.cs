using System;
using System.Collections.Generic;
using System.Text;

namespace SourceName.Utils.Constants.EnumDescriptionProviders
{
    public static class UserValidationDescription
    {
        public static string ToDescriptionString(this UserError error)
        {
            return error switch
            {
                UserError.EmailExists => "Email Already Exists",
                _ => throw new NotImplementedException()
            };
        }
    }
}
