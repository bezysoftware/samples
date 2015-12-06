namespace NameDayClient
{
    class ResultName
    {
        public string DisplayValue { get; set; }

        public bool IsSpecial { get; set; }

        // when it's holiday, it's also special. But there can be special days, which are not holidays (e.g. Mothers day)
        public bool IsHoliday { get; set; }
    }
}
