namespace ArtConnect.Framework
{
    public static class Extentions
    {
        public static string ToDateText(this DateTime? datetime)
        {
            return datetime.HasValue ? datetime.Value.ToString("dd MMM yyyy") : "-";
        }

        public static string ToDateText(this DateTime datetime)
        {
            return datetime.ToString("dd MMM yyyy");
        }

        public static string ToMoney(this decimal amount)
        {
            return $"BDT {amount:N2}";
        }
    }
}
