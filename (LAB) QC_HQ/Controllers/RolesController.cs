
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using _LAB__QC_HQ.Models;

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
}

public class UserRoleViewModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}