using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dt) {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dt.Year;

            if(dt > today.AddYears(-age)) age--;

            return age;
        }
    }
}