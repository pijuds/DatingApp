namespace API.Extensions;

public static class DatetimeExtension
{
    public static int CalculateAge(this DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        int age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

}