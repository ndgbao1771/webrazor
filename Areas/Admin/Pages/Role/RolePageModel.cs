using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorweb.Models;

namespace razorweb.Admin.Role
{
    public class RolePageModel : PageModel
    {
        protected readonly RoleManager<IdentityRole> _roleManager;

        protected readonly ArticleContext _articleContext;
        
        [TempData]
        public string StatusMessage {get; set;}

        public RolePageModel(RoleManager<IdentityRole> roleManager, ArticleContext articleContext){
            _roleManager = roleManager;
            _articleContext = articleContext;
        }
    }

}