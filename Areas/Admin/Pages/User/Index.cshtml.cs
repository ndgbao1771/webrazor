using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razorweb.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace razorweb.Admin.User
{
    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public class UserAndRole : AppUser
        {
            public string RoleNames {get; set;}
        }
        public List<UserAndRole> users { get; set; }

        // số phần tử hiển thị trên 1 trang
        public const int ITEM_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "pages")]
        public int currentPage { get; set; }
        public int countPages { get; set; }
        public int totalUsers { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGet()
        {
            // users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var qr = _userManager.Users.OrderBy(u => u.UserName);

            totalUsers = await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers / ITEM_PER_PAGE);

            if (currentPage < 1)
                currentPage = 1;
            if (currentPage > countPages)
                currentPage = countPages;

            var qr1 = qr.Skip((currentPage - 1) * ITEM_PER_PAGE)
                     .Take(ITEM_PER_PAGE)
                     .Select(u => new UserAndRole() {
                        Id = u.Id,
                        UserName = u.UserName,
                     });

            users = await qr1.ToListAsync();

            foreach(var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join("," , roles);
            }

        }


        public void OnPost() => RedirectToPage();
    }
}
