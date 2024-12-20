// custom exception class for throwing application specific exceptions (e.g. for validation) 
// that can be caught and handled within the application
namespace UserSystem.API.Models.Helpers
{
    public class CustomAppException : Exception
    {
        public CustomAppException() : base() { }

        public CustomAppException(string message) : base(message) { }
    }
}