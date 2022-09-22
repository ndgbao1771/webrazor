using System.Security.Claims;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorweb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace razorweb.Admin.Role
{
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, ArticleContext articleContext) : base(roleManager, articleContext)
        {
        }

        public class InputModel
        {
            [Display(Name = "Tên role")]
            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {0} đến {1} kí tự.")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<IdentityRoleClaim<string>> Claims {get; set;}

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(roleid);

            if (role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };

                Claims = await _articleContext.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();

                return Page();
            }
            return NotFound("Không tìm thấy role");
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null) return NotFound("Không tìm thấy role");

            Claims = await _articleContext.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa đổi tên role {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
