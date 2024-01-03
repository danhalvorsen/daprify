namespace Daprify.Validation
{
    public static class ErrorMessage
    {
        public static readonly string NotFoundError = "Could not find the specified {PropertyName}: {PropertyValue}";

        public static string NotFoundErrorFunc(string propertyName, object propertyValue)
        {
            return NotFoundError.Replace("{PropertyName}", propertyName)
                                .Replace("{PropertyValue}", propertyValue.ToString());
        }
    }
}
