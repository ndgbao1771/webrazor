using Microsoft.AspNetCore.Authorization;

namespace App.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        public ArticleUpdateRequirement(int year = 2022, int month = 9, int date = 19)
        {
            Year = year;
            Month = month;
            Date = date;
        }

        public int Year {get; set;}

        public int Month {get; set;}
        
        public int Date {get; set;}
    }
}