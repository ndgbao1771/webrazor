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
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, ArticleContext articleContext) : base(roleManager, articleContext)
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

        public IdentityRoleClaim<string> claim {get; set;}

        public async Task<IActionResult> OnGet(int ? claimid)
        {
            if(claimid == null) return NotFound("Không tìm thấy role");
            claim = _articleContext.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("Không tìm thấy role");

            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int ? claimid)
        {
            if(claimid == null) return NotFound("Không tìm thấy role");
            claim = _articleContext.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("Không tìm thấy role");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            if((_articleContext.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id)))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;

            await _articleContext.SaveChangesAsync();
            
            StatusMessage = "Vừa cập nhật Claim";

            return RedirectToPage("./Edit", new{roleid = role.Id});
        }



        public async Task<IActionResult> OnPostDeleteAsync(int ? claimid)
        {
            if(claimid == null) return NotFound("Không tìm thấy role");
            claim = _articleContext.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy role");

            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("Không tìm thấy role");

            
            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));
            
            StatusMessage = "Vừa xóa Claim";

            return RedirectToPage("./Edit", new{roleid = role.Id});
        }
    }
}
