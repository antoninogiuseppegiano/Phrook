namespace Phrook.Models.Util
{
    public class Utility
    {
        public static string _getShortTitle (string title){
			string newTitle;
			const int maxChar = 20;
			if (title.Length > maxChar)
			{
				newTitle = string.Concat($"{title.Substring(0, maxChar)}", "...");
			}
			else
			{
				newTitle = title;
			}
			return newTitle;
		}
    }
}