using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using App.Models;


namespace App.Admin.User
{
    [AllowAnonymous]
    public class EditUserRoleClaimModel : PageModel
    {

        private readonly AppDbContext _articleContext;
        private readonly UserManager<AppUser> _userManager;

        public EditUserRoleClaimModel(AppDbContext articleContext, UserManager<AppUser> userManager)
        {
            _articleContext = articleContext;
            _userManager = userManager;
        }


        [TempData]
        public string StatusMessage { get; set; }

        public NotFoundObjectResult OnGet() => NotFound("Không được truy cập");

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

        public AppUser user {get; set;}

         public IdentityUserClaim<string> userClaim {get; set;}

        

        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if(user == null) return NotFound("Không tìm thấy user");
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if(user == null) return NotFound("Không tìm thấy user");

            if(!ModelState.IsValid) return Page();

            var claims = _articleContext.UserClaims.Where(c => c.UserId == user.Id);

            if(claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Đặc tính này đã có");
                return Page();
            }

            await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm đặc tính cho user";
            return RedirectToPage("./AddRole", new {Id = user.Id});

        }

        public IdentityUserClaim<string> userclaim {get; set;}

        public async Task<IActionResult> OnGetEditClaimAsync(int ? claimid)
        {
            if(claimid == null) return NotFound("Không tìm thấy user");

            userclaim = _articleContext.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userclaim.UserId);

            if(user == null) return NotFound("Không tìm thấy user");

            Input = new InputModel()
            {
                ClaimType = userclaim.ClaimType,
                ClaimValue = userclaim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int ? claimid)
        {
            if(claimid == null) return NotFound("Không tìm thấy user");

            userclaim = _articleContext.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userclaim.UserId);

            if(user == null) return NotFound("Không tìm thấy user");

            if(!ModelState.IsValid) return Page();

            if(_articleContext.UserClaims.Any(c => c.UserId == user.Id 
                                            &&  c.ClaimType == Input.ClaimType 
                                            && c.ClaimValue == Input.ClaimValue 
                                            && c.Id != userclaim.Id))
                                            {
                                                ModelState.AddModelError(string.Empty, "Claim này đã có");
                                                return Page();
                                            }

            userclaim.ClaimType = Input.ClaimType;
            userclaim.ClaimValue = Input.ClaimValue;

            await _articleContext.SaveChangesAsync();

            StatusMessage = "Bạn vừa cập nhật claim";

            return RedirectToPage("./AddRole", new{ID = user.Id});
        }


        public async Task<IActionResult> OnPostDeleteAsync(int ? claimid)
        {
            if(claimid == null) return NotFound("Không tìm thấy user");

            userclaim = _articleContext.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userclaim.UserId);

            if(user == null) return NotFound("Không tìm thấy user");

            await _userManager.RemoveClaimAsync(user, new Claim(userclaim.ClaimType, userclaim.ClaimValue));

            StatusMessage = "Bạn đã xóa claim";

            return RedirectToPage("./AddRole", new{ID = user.Id});
        }
    }
}
