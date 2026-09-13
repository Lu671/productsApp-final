using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using productsApp_improved.Data;
using productsApp_improved.Hubs;
using productsApp_improved.Models;
using WebPush;

namespace productsApp_improved.Controllers
{
    public class AccountController : Controller
    {
     
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _db;

        public AccountController(UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager, AppDbContext db)
    {
         _userManager = userManager;
         _signInManager = signInManager;
         _db = db;
        }

        // register
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string userName, string email, string password)
        {
             var user = new IdentityUser { UserName = userName, Email = email };
             var result = await _userManager.CreateAsync(user,password);

             if (result.Succeeded)
             {
                 await _userManager.AddToRoleAsync(user, "Customer");
                 await _signInManager.SignInAsync(user, false);
                 return RedirectToAction("Home", "Products");
             }
             foreach (var error in result.Errors)
             {
                 ModelState.AddModelError("", error.Description);
             }
             return View();
    }
        // login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
             var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

             if (result.Succeeded)
             {
                 TempData["ShowNotificationRequest"] = true;
                 var user = await _userManager.FindByEmailAsync(email);
                 if (await _userManager.IsInRoleAsync(user, "Admin"))
                     return RedirectToAction("InvMang", "Products");

                 return RedirectToAction("ListProducts", "Products");
             }
             
             ModelState.AddModelError("", "Emai or password is incorrect");
             
             return View();
        }

        public async Task<IActionResult> Logout()
        {
             await _signInManager.SignOutAsync();
             HttpContext.Session.Clear();
             return RedirectToAction("Login");
        }

        public async Task<IActionResult> UsersList()
        {
             var users = _userManager.Users.ToList();
             var userListWithRoles = new List<UserWithRoleViewModel>();

             foreach (var user in users)
             {
                 // جلب الأدوار لكل مستخدم
                 var roles = await _userManager.GetRolesAsync(user);

                 userListWithRoles.Add(new UserWithRoleViewModel
                 {
                     Email = user.Email,
                     UserName = user.UserName,
                     RoleName = roles.FirstOrDefault() ?? "No Role",
                     UserId = user.Id
                 });
             }

             return View(userListWithRoles);
         
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
             var user = await _userManager.FindByIdAsync(userId);

             var result = await _userManager.DeleteAsync(user);
             if (result.Succeeded)
             {
                 TempData["success"] = "User deleted successfully.";
             }
             else
             {
                 TempData["error"] = "Error";
             }

             return RedirectToAction("UsersList");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, string userRole)
        {
             // find usr
             var user = await _userManager.FindByIdAsync(userId);

             // save the current role
             var currentRoles = await _userManager.GetRolesAsync(user);
             string oldRolesText = currentRoles.Any() ? string.Join(", ", currentRoles) : "No Role";
             // delete the role (previous)
             await _userManager.RemoveFromRolesAsync(user, currentRoles);

             // change to new slected roel
             var result = await _userManager.AddToRoleAsync(user, userRole);

             if (result.Succeeded)
             {
                 string currentAdminEmail = User.Identity?.Name ?? "System/Admin";
                 var roleChangeLog = new AuditLog
                 {
                     UserEmail = currentAdminEmail,       
                     Action = "Modified",                
                     TableName = "IdentityUserRole",             
                     RecordId = user.Id,                 
                     Timestamp = DateTime.Now,
                     OldValues = $"Role: {oldRolesText}",     
                     NewValues = $"Role: {userRole} (Assigned to {user.Email})" 
                 };
                 _db.AuditLogs.Add(roleChangeLog); 
                 await _db.SaveChangesAsync();
                 TempData["success"] = "User role updated successfully.";
             }
             else
             {
                 TempData["error"] = "Error.";
             }

             return RedirectToAction("UsersList");
        }



    }



}
