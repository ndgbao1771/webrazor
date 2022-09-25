using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;

        protected readonly AppDbContext _articleContext;
        
        [TempData]
        public string StatusMessage {get; set;}

        public RolePageModel(RoleManager<IdentityRole> roleManager, AppDbContext articleContext){
            _roleManager = roleManager;
            _articleContext = articleContext;
        }
    }

}