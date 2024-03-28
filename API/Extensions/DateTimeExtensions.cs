namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            int age = today.Year - dob.Year;

            // Check if the birthday hasn't occurred yet this year
            if (dob.Month > today.Month || (dob.Month == today.Month && dob.Day > today.Day))
            {
                // Subtract one from the age
                age--;
            }
            // Check if the birthday falls on February 29th
            else if (dob.Month == 2 && dob.Day == 29)
            {
                // Check if today is February 28th or 29th and the birth year was a leap year
                if (today.Month == 2 && (today.Day == 28 || today.Day == 29) && DateTime.IsLeapYear(dob.Year))
                {
                    // No adjustment needed for leap years
                }
                else if (today.Month < 2 || (today.Month == 2 && today.Day < 28))
                {
                    // Subtract one from the age for non-leap years where today is before February 28th
                    age--;
                }
            }

            return age;
        }
    }
}


// public static int CalculateAge(this DateOnly dob)
// {
//     DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
//     int age = today.Year - dob.Year;
//     if (dob > today.AddYears(-age)) age--;
//     return age;
//     //! this is not the accurate way to calculate age