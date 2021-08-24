using System;
using System.Text.RegularExpressions;

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
        public static DateTime _getDateTimeFromyyyy_MM_dd (string yyyy_MM_dd){
			if(String.IsNullOrWhiteSpace(yyyy_MM_dd) || !Regex.IsMatch(yyyy_MM_dd, @"^(\d{4})-(\d{2})-(\d{2})$"))
			{
				// throw new ArgumentException("Date format not valid");
				return DateTime.MinValue.Date;
			}
			int year = Convert.ToInt32(yyyy_MM_dd.Substring(0, 4)),
				month = Convert.ToInt32(yyyy_MM_dd.Substring(5, 2)),
				day = Convert.ToInt32(yyyy_MM_dd.Substring(8, 2));
			return new DateTime(year, month, day);
		}
    }
}