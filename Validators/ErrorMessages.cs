using Daprify.Models;

namespace Daprify.Validation
{
    public static class ErrorMessage
    {
        private static readonly string NotFoundError = "Could not find the specified {PropertyName}: {PropertyValue}";
        private static readonly string InvalidError = "The {PropertyName} is invalid: {PropertyValue}";


        public static string NotFoundErrorFunc(string propertyName, object propertyValue)
        {
            return NotFoundError.Replace("{PropertyName}", propertyName)
                                .Replace("{PropertyValue}", propertyValue.ToString());
        }

        public static string InvalidErrorFunc(string propertyName, List<Value> propertyValue)
        {
            string values = string.Join(", ", propertyValue);
            return InvalidError.Replace("{PropertyName}", propertyName)
                                .Replace("{PropertyValue}", values);
        }
    }
}
