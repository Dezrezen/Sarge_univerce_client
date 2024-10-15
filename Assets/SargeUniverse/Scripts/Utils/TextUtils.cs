using System.Globalization;

namespace SargeUniverse.Scripts.Utils
{
    public static class TextUtils
    {
        public static string NumberToTextWithSeparator(int number, string separator = " ")
        {
            var numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numberFormatInfo.NumberGroupSeparator = separator;

            return number.ToString("#,0", numberFormatInfo);
        }
    }
}