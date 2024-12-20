using System.Text.RegularExpressions;
namespace UserSystem.API.Infrastructure.Security
{
    public static class PasswordValidator
    {
        /*
        The regular expression below cheks that a password:
            - Has minimum 8 characters in length. Adjust it by modifying {8,}
            - At least one uppercase English letter.
            - At least one lowercase English letter.
            - At least one digit.
            - At least one special character.
        */
        public static bool IsPasswordStrong(string password)
        {
            // Validate strong password
            Regex regEx = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            return regEx.IsMatch(password);
        }
    }
}