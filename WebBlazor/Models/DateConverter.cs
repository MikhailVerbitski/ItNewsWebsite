using System;

namespace WebBlazor.Models
{
    public class DateConverter
    {
        public static string GetTime(DateTime time)
        {
            string res;
            var now = DateTime.Now;
            var difference = now - time;
            if (difference.Days > 0)
            {
                res = $"{difference.Days} дней";
            }
            else if (difference.Hours > 0)
            {
                res = $"{difference.Hours} часов";
            }
            else if (difference.Minutes > 0)
            {
                res = $"{difference.Minutes} минут";
            }
            else
            {
                res = $"{difference.Seconds} секунд";
            }
            res += " назад";
            return res;
        }
    }
}
