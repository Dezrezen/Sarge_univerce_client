using System;
using UnityEngine;

namespace SargeUniverse.Scripts.Utils
{
    public class Tools
    {
        public static string SecondsToTimeFormat(TimeSpan time)
        {
            var result = "";
            var days = Mathf.FloorToInt((float)time.TotalDays);
            var hours = Mathf.FloorToInt((float)time.TotalHours);
            var mins = Mathf.FloorToInt((float)time.TotalMinutes);
            if (days > 0)
            {
                hours -= (days * 24);
                if (hours > 0)
                {
                    result = days.ToString() + "d " + hours.ToString() + "H";
                }
                else
                {
                    result = days.ToString() + "d";
                }
            }
            else if (hours > 0)
            {
                mins -= (hours * 60);
                if (mins > 0)
                {
                    result = hours.ToString() + "H " + mins.ToString() + "M";
                }
                else
                {
                    result = hours.ToString() + "H";
                }
            }
            else if (mins > 0)
            {
                var sec = Mathf.FloorToInt((float)time.TotalSeconds) - (mins * 60);
                if (sec > 0)
                {
                    result = mins.ToString() + "M " + sec.ToString() + "s";
                }
                else
                {
                    result = mins.ToString() + "M";
                }
            }
            else
            {
                result = ((int)time.TotalSeconds).ToString() + "s";
            }
            return result;
        }
        
        public static string SecondsToTimeFormat(int seconds)
        {
            return SecondsToTimeFormat(TimeSpan.FromSeconds(seconds));
        }
    }
}