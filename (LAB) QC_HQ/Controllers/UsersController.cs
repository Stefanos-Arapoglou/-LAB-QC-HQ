using _LAB__QC_HQ.Data;
using _LAB__QC_HQ.Models;
using _LAB__QC_HQ.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace _LAB__QC_HQ.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Users - List all users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.UserDepartments)
                .ThenInclude(ud => ud.Department)
                .Where(u => u.IsActive)
                .ToListAsync();
            return View(users);
        }

        // GET: UsersController/Details/5
        public async Task<IActionResult> Details(string id)  // Change to string
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.UserDepartments)
                .ThenInclude(ud => ud.Department)
                .FirstOrDefaultAsync(m => m.Id == id);  // Use == instead of .Equals()

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            var vm = new CreateUserVM
            {
                IsActive = true,
                Departments = _context.Departments
                    .Select(d => new UserDepartmentEditVM
                    {
                        DepartmentId = d.DepartmentId,
                        DepartmentName = d.Name,
                        IsSelected = false,
                        ClearanceLevel = 0
                    }).ToList()
            };

            return View(vm);
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM model)
        {
            if (!ModelState.IsValid) { 
                PopulateDepartmentNames(model);
            return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                JobTitle = model.JobTitle,
                HireDate = model.HireDate,
                IsActive = model.IsActive
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            foreach (var dept in model.Departments.Where(d => d.IsSelected))
            {
                _context.UserDepartments.Add(new UserDepartment
                {
                    UserId = user.Id,
                    DepartmentId = dept.DepartmentId,
                    ClearanceLevel = dept.ClearanceLevel
                });
            }
            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.ProfileImageFile.CopyToAsync(ms);
                user.ProfileImage = ms.ToArray();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: UsersController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.Users
                .Include(u => u.UserDepartments)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var allDepartments = await _context.Departments
                .Where(d => d.IsActive)
                .ToListAsync();

            var vm = new EditUserVM
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                JobTitle = user.JobTitle,
                HireDate = user.HireDate,
                IsActive = user.IsActive,
                Departments = allDepartments.Select(d =>
                {
                    var ud = user.UserDepartments
                        .FirstOrDefault(x => x.DepartmentId == d.DepartmentId);

                    return new UserDepartmentEditVM
                    {
                        DepartmentId = d.DepartmentId,
                        DepartmentName = d.Name, // ✅ THIS LINE
                        IsSelected = ud != null,
                        ClearanceLevel = ud?.ClearanceLevel ?? 0
                    };
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserVM vm)
        {
            if (!ModelState.IsValid)
            {
                foreach (var kv in ModelState)
                {
                    var errors = string.Join(", ", kv.Value.Errors.Select(e => e.ErrorMessage));
                    Debug.WriteLine($"Key: {kv.Key}, Invalid: {kv.Value.ValidationState}, Errors: {errors}");
                }
            }

            // Get user from DbContext including related departments
            var user = await _context.Users
                .Include(u => u.UserDepartments)
                .FirstOrDefaultAsync(u => u.Id == vm.Id);

            if (user == null) return NotFound();

            // Update basic user info
            user.UserName = vm.UserName;
            user.Email = vm.Email;
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.JobTitle = vm.JobTitle;
            user.HireDate = vm.HireDate;
            user.IsActive = vm.IsActive;

            // Remove old UserDepartments
            user.UserDepartments.Clear();

            // Add selected departments
            foreach (var d in vm.Departments.Where(x => x.IsSelected))
            {
                user.UserDepartments.Add(new UserDepartment
                {
                    DepartmentId = d.DepartmentId,
                    ClearanceLevel = d.ClearanceLevel,
                    AssignedAt = DateTime.UtcNow
                });
            }

            try
            {
                // Save all changes in one SaveChangesAsync call
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Optional: inspect errors for debugging
                ModelState.AddModelError("", "An error occurred while updating the user.");
                await PopulateDepartments(vm);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }




        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        private async Task<bool> UserExistsAsync(string id)
        {
            return await _userManager.Users.AnyAsync(e => e.Id == id);
        }

        private async Task PopulateDepartments(EditUserVM vm)
        {
            var allDepartments = await _context.Departments
                .Where(d => d.IsActive)
                .ToListAsync();

            foreach (var dept in vm.Departments)
            {
                dept.DepartmentName = allDepartments
                    .FirstOrDefault(d => d.DepartmentId == dept.DepartmentId)?.Name;
            }
        }

        private void PopulateDepartmentNames(EditUserVM model)
        {
            var departmentLookup = _context.Departments
                .ToDictionary(d => d.DepartmentId, d => d.Name);

            foreach (var dept in model.Departments)
            {
                dept.DepartmentName = departmentLookup[dept.DepartmentId];
            }
        }
    }
}
