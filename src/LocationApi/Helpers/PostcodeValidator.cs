using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LocationApi.Helpers
{
    public static class PostcodeValidator
    {
        private static readonly char[] InvalidChars = { '-', '+', '!', '@', '$', '£', '^', '*', '.', ',', '[', ']', '{', '}', '(', ')', '?', '"', '#', '%', '&', '/', '\\', ',', '>', '<', '\'', ':', ';', '|', '_', '~', '`', '<', '>', '=' };
        private const string ValidPostcodeRegex = @"^([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([AZa-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9]?[A-Za-z]))))[0-9][A-Za-z]{2})$";

        public static string Sanitize(string source)
        {
            var sb = new StringBuilder();

            foreach (var letter in source.Where(letter => !InvalidChars.Contains(letter)))
            {
                sb.Append(letter);
            }

            return sb.ToString();
        }

        public static bool IsValid(string source)
        {
            var rx = new Regex(ValidPostcodeRegex);

            return rx.IsMatch(source);
        }
    }
}