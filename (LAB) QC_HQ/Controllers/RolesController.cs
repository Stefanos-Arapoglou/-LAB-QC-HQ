
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _LAB__QC_HQ.Models;

/*[Authorize(Roles = "Admin")] */
public class RolesController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RolesController(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // GET: /Roles
    public IActionResult Index()
    {
        var roles = _roleManager.Roles;
        return View(roles);
    }

    // GET: /Roles/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Roles/Create
    [HttpPost]
    public async Task<IActionResult> Create(string roleName)
    {
        if (!string.IsNullOrEmpty(roleName))
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
        }
        return View();
    }

    // GET: /Roles/Assign
    public async Task<IActionResult> Assign()
    {
        var users = _userManager.Users.ToList();
        var roles = _roleManager.Roles.ToList();

        ViewBag.Users = users;
        ViewBag.Roles = roles;

        return View();
    }

    // POST: /Roles/Assign
    [HttpPost]
    public async Task<IActionResult> Assign(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Remove existing roles and add new one
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, roleName);

            return RedirectToAction("Index");
        }
        return View();
    }

    // GET: /Roles/Users
    public async Task<IActionResult> Users()
    {
        var users = new List<UserRoleViewModel>();

        foreach (var user in _userManager.Users.ToList())
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            users.Add(new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles.ToList()
            });
        }

        return View(users);
    }

    // GET: /Roles/Delete
    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        // Optional: Check if role has users before allowing deletion
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        if (usersInRole.Any())
        {
            ViewBag.ErrorMessage = $"Cannot delete role '{role.Name}' because it has {usersInRole.Count} user(s) assigned. Remove users from this role first.";
            return View("Error");
        }

        return View(role);
    }

    // POST: /Roles/Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        // Check if it's a system role that shouldn't be deleted
        if (IsSystemRole(role.Name))
        {
            TempData["ErrorMessage"] = $"Cannot delete system role: {role.Name}";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Role '{role.Name}' deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(role);
        }
        catch (Exception ex)
        {
            // Log the exception
            TempData["ErrorMessage"] = $"Error deleting role: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // Optional helper method to protect system roles
    private bool IsSystemRole(string roleName)
    {
        var systemRoles = new[] { "Administrator", "SuperAdmin", "SystemAdmin", "User", "Admin" };
        return systemRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

}

public class UserRoleViewModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}