using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using razorweb.Models;
using Microsoft.AspNetCore.Authorization;

namespace razorweb.Admin.Role
{
    [Authorize(Roles = "Admin")]
    [Authorize(Roles = "Vip")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, ArticleContext articleContext) : base(roleManager, articleContext)
        {

        }


        public List<IdentityRole> roles {get; set;}

        public async Task OnGet()
        {
            roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }


        public void OnPost() => RedirectToPage();
    }
}
