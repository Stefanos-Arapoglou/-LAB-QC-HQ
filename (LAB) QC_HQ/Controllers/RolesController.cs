/* NOTES 
 
This controller manages user roles within the application, allowing administration to create roles, assign roles to users, view users and their roles, and delete roles. 
It ensures that only users with the "Admin" role can access these functionalities.
It also provides an overall view of all users and their assigned role
Everything goes through Microsoft Identity RoleManager and UserManager!

SUMMARY:
    1) HANDLES LISTING of all roles
    2) HANDLES CREATION of new roles
    3) HANDLES ASSIGNMENT of roles to users
    4) HANDLES VIEWING of all users and their roles
    5) HANDLES DELETION of roles (with checks to prevent deletion of system roles or roles with assigned users)
  
 */


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;


[Authorize(Roles = "Admin")]
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
    // Lists all roles
    public IActionResult Index()
    {
        var roles = _roleManager.Roles;
        return View(roles);
    }



    // GET: /Roles/Create
    // Directs to the role creation view
    public IActionResult Create()
    {
        return View();
    }



    // POST: /Roles/Create
    // creates a new role
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
    // Directs to the role assignment view
    public async Task<IActionResult> Assign()
    {
        var users = _userManager.Users.ToList();
        var roles = _roleManager.Roles.ToList();

        ViewBag.Users = users;
        ViewBag.Roles = roles;

        return View();
    }



    // POST: /Roles/Assign
    // Assigns a role to a user
    [HttpPost]
    public async Task<IActionResult> Assign(string userId, string roleName)
    {
        //first we get the user by their ID
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
    // An overview of all users and their roles
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
    // Directs to the role deletion confirmation view
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

        //Check if role has users before allowing deletion
        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        if (usersInRole.Any())
        {
            ViewBag.ErrorMessage = $"Cannot delete role '{role.Name}' because it has {usersInRole.Count} user(s) assigned. Remove users from this role first.";
            return View("Error");
        }

        return View(role);
    }



    // POST: /Roles/Delete
    // Deletes the role after confirmation
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



    // Helper method to protect system roles
    private bool IsSystemRole(string roleName)
    {
        var systemRoles = new[] { "Administrator", "SuperAdmin", "SystemAdmin", "User", "Admin" };
        return systemRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

}

