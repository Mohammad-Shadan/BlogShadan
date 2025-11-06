using System.Text.RegularExpressions;

namespace BlogShadan.Helper
{
    public static class RemoveHTMLTagHelper
    {
        public static string RemoveHTMLTag(string input )
        {
            return Regex.Replace(input,"<.*?>|&.*?;",string.Empty);
        }
    }
}
