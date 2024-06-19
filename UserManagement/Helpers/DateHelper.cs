namespace UserManagement.Helpers;

public class DateHelper
{
    public string intToMonthName(int n)
    {
        string[] Months = new string[]
        {
            "فروردین", 
            "اردیبهشت", 
            "خرداد", 
            "تیر", 
            "مرداد", 
            "شهریور", 
            "مهر", 
            "آبان", 
            "آذر", 
            "دی", 
            "بهمن", 
            "اسفند" 
        };
        return Months[n - 1];
    }
}