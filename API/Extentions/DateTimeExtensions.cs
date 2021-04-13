using System;

namespace API.Extentions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var tooday = DateTime.Today;
            var age = tooday.Year - dateOfBirth.Year;
            if(dateOfBirth.Date > tooday.AddYears(-age)) age--;
            return age;

        }
    }
}