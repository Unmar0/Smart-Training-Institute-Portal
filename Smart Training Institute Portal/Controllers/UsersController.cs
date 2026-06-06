using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Smart_Training_Institute_Portal.Models;
using Smart_Training_Institute_Portal.ViewModels;

namespace Smart_Training_Institute_Portal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.InstructorProfile)
                .OrderBy(u => u.Email)
                .ToListAsync();

            var model = new List<UserListItemViewModel>();

            foreach (var user in users)
            {
                model.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    Role = await GetPrimaryRoleAsync(user),
                    EmailConfirmed = user.EmailConfirmed,
                    HasStudentProfile = user.StudentProfile?.IsDeleted != true && user.StudentProfile != null,
                    HasInstructorProfile = user.InstructorProfile?.IsDeleted != true && user.InstructorProfile != null
                });
            }

            return View(model);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.InstructorProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(await BuildDetailsViewModelAsync(user));
        }

        public async Task<IActionResult> Create()
        {
            await PopulateRolesAsync();
            return View(new UserCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            await ValidateRoleAsync(model.SelectedRole);

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    EmailConfirmed = model.EmailConfirmed
                };

                var createResult = await _userManager.CreateAsync(user, model.Password);

                if (createResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);

                    if (roleResult.Succeeded)
                    {
                        TempData["Success"] = "User created successfully.";
                        return RedirectToAction(nameof(Index));
                    }

                    await _userManager.DeleteAsync(user);
                    AddIdentityErrors(roleResult);
                }
                else
                {
                    AddIdentityErrors(createResult);
                }
            }

            await PopulateRolesAsync(model.SelectedRole);
            return View(model);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserEditViewModel
            {
                Id = user.Id,
                UserName = user.Email ?? user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                SelectedRole = roles.FirstOrDefault() ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed
            };

            await PopulateRolesAsync(model.SelectedRole);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                return BadRequest();
            }

            await ValidateRoleAsync(model.SelectedRole);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == user.Id && model.SelectedRole != "Admin")
            {
                ModelState.AddModelError(nameof(model.SelectedRole), "You cannot remove the Admin role from the currently signed-in account.");
            }

            if (ModelState.IsValid)
            {
                user.EmailConfirmed = model.EmailConfirmed;
                user.PhoneNumber = model.PhoneNumber;

                var userNameResult = await _userManager.SetUserNameAsync(user, model.Email);
                if (!userNameResult.Succeeded)
                {
                    AddIdentityErrors(userNameResult);
                }

                var emailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!emailResult.Succeeded)
                {
                    AddIdentityErrors(emailResult);
                }

                if (ModelState.IsValid)
                {
                    var updateResult = await _userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        AddIdentityErrors(updateResult);
                    }
                }

                if (ModelState.IsValid)
                {
                    var existingRoles = await _userManager.GetRolesAsync(user);
                    var rolesToRemove = existingRoles.Where(r => r != model.SelectedRole).ToList();

                    if (rolesToRemove.Count > 0)
                    {
                        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                        if (!removeRolesResult.Succeeded)
                        {
                            AddIdentityErrors(removeRolesResult);
                        }
                    }

                    if (ModelState.IsValid && !existingRoles.Contains(model.SelectedRole))
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                        if (!addRoleResult.Succeeded)
                        {
                            AddIdentityErrors(addRoleResult);
                        }
                    }
                }

                if (ModelState.IsValid && !string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
                    if (!passwordResult.Succeeded)
                    {
                        AddIdentityErrors(passwordResult);
                    }
                }

                if (ModelState.IsValid)
                {
                    TempData["Success"] = "User updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await PopulateRolesAsync(model.SelectedRole);
            return View(model);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.InstructorProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(await BuildDetailsViewModelAsync(user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.StudentProfile)
                .Include(u => u.InstructorProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == user.Id)
            {
                ModelState.AddModelError(string.Empty, "You cannot delete the currently signed-in admin account.");
                return View("Delete", await BuildDetailsViewModelAsync(user));
            }

            if (user.StudentProfile?.IsDeleted != true && user.StudentProfile != null)
            {
                ModelState.AddModelError(string.Empty, "Delete the linked student profile before deleting this user.");
            }

            if (user.InstructorProfile?.IsDeleted != true && user.InstructorProfile != null)
            {
                ModelState.AddModelError(string.Empty, "Delete the linked instructor profile before deleting this user.");
            }

            if (!ModelState.IsValid)
            {
                return View("Delete", await BuildDetailsViewModelAsync(user));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result);
                return View("Delete", await BuildDetailsViewModelAsync(user));
            }

            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GetPrimaryRoleAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "No Role";
        }

        private async Task<UserDetailsViewModel> BuildDetailsViewModelAsync(ApplicationUser user)
        {
            return new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Role = await GetPrimaryRoleAsync(user),
                EmailConfirmed = user.EmailConfirmed,
                HasStudentProfile = user.StudentProfile?.IsDeleted != true && user.StudentProfile != null,
                HasInstructorProfile = user.InstructorProfile?.IsDeleted != true && user.InstructorProfile != null
            };
        }

        private async Task PopulateRolesAsync(string? selectedRole = null)
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name!)
                .ToListAsync();

            ViewData["SelectedRole"] = new SelectList(roles, selectedRole);
        }

        private async Task ValidateRoleAsync(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("SelectedRole", "Please select a role.");
                return;
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                ModelState.AddModelError("SelectedRole", "Selected role does not exist.");
            }
        }

        private void AddIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
