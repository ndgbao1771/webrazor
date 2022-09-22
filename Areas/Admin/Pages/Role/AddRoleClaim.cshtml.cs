using System.Security.Claims;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using razorweb.Models;
using Microsoft.AspNetCore.Authorization;

namespace razorweb.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, ArticleContext articleContext) : base(roleManager, articleContext)
        {
        }

        public class InputModel
        {
            [Display(Name = "Tên Claim")]
            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {0} đến {1} kí tự.")]
            public string ClaimType { get; set; }

            [Display(Name = "Gía trị")]
            [Required(ErrorMessage = "{0} không được bỏ trống")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải có độ dài từ {0} đến {1} kí tự.")]
            public string ClaimValue { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IdentityRole role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            role = await _roleManager.FindByIdAsync(roleid);

            if (role == null) return NotFound("Không tìm thấy role");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            role = await _roleManager.FindByIdAsync(roleid);

            if (role == null) return NotFound("Không tìm thấy role");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if((await _roleManager.GetClaimsAsync(role)).Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, newClaim);

            if(!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => {
                    ModelState.AddModelError(string.Empty, e.Description);
                });
                return Page();
            }
            
            StatusMessage = "Vừa thêm đặc tính (claim) mới";

            return RedirectToPage("./Edit", new{roleid = role.Id});
        }
    }
}
