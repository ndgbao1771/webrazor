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
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, ArticleContext articleContext) : base(roleManager, articleContext)
        {

        }

        public class RoleModel : IdentityRole
        {
            public string[] Claims {get; set;}
        }

        public List<RoleModel> roles {get; set;}

        public async Task OnGet()
        {
            var r = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            roles = new List<RoleModel> ();
            foreach(var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);
                var claimsString = claims.Select(c => c.Type + "=" + c.Value);

                var rm = new RoleModel()
                {
                    Name = _r.Name,
                    Id = _r.Id,
                    Claims = claimsString.ToArray()
                };
                roles.Add(rm);
            }
        }


        public void OnPost() => RedirectToPage();
    }
}
