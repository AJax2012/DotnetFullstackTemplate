namespace SourceName.Utils.Constants
{
    public static class StringResources
    {
        #region StaticStrings
        public static string GenericDatabaseError = "Encountered an error when connecting to the database. Please try again.";
        public static string GenericServerError = "Encountered an error on the server. Please try again.";
        public static string GenericUnauthorizedException = "Resource unauthorized.";
        public static string GenericTimeoutException = "Connection has timed out. Please try again.";

        #endregion
        #region DynamicStrings

        public static string GenericNotFoundError(string resource)
        {
            return $"Could not find {resource}.";
        }
        #endregion
    }
}
