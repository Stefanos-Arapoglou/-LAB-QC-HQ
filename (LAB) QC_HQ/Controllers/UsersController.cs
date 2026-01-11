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
                ModelState.AddModelError(nameof(model.Password), error.Description);
                PopulateDepartmentNames(model);
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
                await PopulateDepartments(vm);
                return View(vm);
            }

            // ✅ ALWAYS load Identity user via UserManager
            var user = await _userManager.Users
                .Include(u => u.UserDepartments)
                .FirstOrDefaultAsync(u => u.Id == vm.Id);

            if (user == null) return NotFound();

            // ✅ Identity-safe updates
            if (user.UserName != vm.UserName)
            {
                var result = await _userManager.SetUserNameAsync(user, vm.UserName);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    await PopulateDepartments(vm);
                    return View(vm);
                }
            }

            if (user.Email != vm.Email)
            {
                var result = await _userManager.SetEmailAsync(user, vm.Email);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    await PopulateDepartments(vm);
                    return View(vm);
                }
            }

            // ✅ Non-Identity fields (safe to set directly)
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.JobTitle = vm.JobTitle;
            user.HireDate = vm.HireDate;
            user.IsActive = vm.IsActive;

            // ✅ Update departments
            user.UserDepartments.Clear();

            foreach (var d in vm.Departments.Where(x => x.IsSelected))
            {
                user.UserDepartments.Add(new UserDepartment
                {
                    DepartmentId = d.DepartmentId,
                    ClearanceLevel = d.ClearanceLevel,
                    AssignedAt = DateTime.UtcNow
                });
            }

            // ✅ Persist everything
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }


        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users
                .Include(u => u.UserDepartments)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            // Remove related records FIRST
            _context.UserDepartments.RemoveRange(user.UserDepartments);

            // Remove user
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
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


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
