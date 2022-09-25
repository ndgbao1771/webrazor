using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AppDbContext _articleContext;
        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext articleContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _articleContext = articleContext;
        }


        [TempData]
        public string StatusMessage { get; set; }

        public AppUser user { get; set; }

        [BindProperty]
        [Display(Name = "Các role gán cho User")]
        public string[] RoleName { get; set; }

        public SelectList allRoles { get; set; }

        public List<IdentityRoleClaim<string>> claimsInRole { get; set; }

        public List<IdentityUserClaim<string>> claimsInUserClaim { get; set; }


        async Task GetClaims(string id)
        {
            var listRoles = from r in _articleContext.Roles
                            join ur in _articleContext.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            var _claimsInRole = from c in _articleContext.RoleClaims
                                join r in listRoles on c.RoleId equals r.Id
                                select c;

            claimsInRole = await _claimsInRole.ToListAsync();

            claimsInUserClaim = await (from c in _articleContext.UserClaims
            where c.UserId == id select c).ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có user");
            }

            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user có id {id}.");
            }

            RoleName = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            allRoles = new SelectList(roleNames);

            await GetClaims(id);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Không có user");
            }

            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không thấy user có id {id}.");
            }

            // rolename

            await GetClaims(id);

            var oldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            var deleteRole = oldRoleNames.Where(r => !RoleName.Contains(r));

            var addRole = RoleName.Where(r => !oldRoleNames.Contains(r));

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleteRole);

            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRole);

            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }


            StatusMessage = $"Bạn vừa cập nhật Role cho user: {user.UserName}.";

            return RedirectToPage("./Index");
        }
    }
}
