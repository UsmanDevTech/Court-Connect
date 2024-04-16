using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        ////organization owner
        //var addOrganizationOwnerRole = new IdentityRole("organization.owner");

        //if (_roleManager.Roles.All(r => r.Name != addOrganizationOwnerRole.Name))
        //{
        //    await _roleManager.CreateAsync(addOrganizationOwnerRole);
        //}

        ////branch owner
        //var addbranchOwnerRole = new IdentityRole("branch.owner");

        //if (_roleManager.Roles.All(r => r.Name != addbranchOwnerRole.Name))
        //{
        //    await _roleManager.CreateAsync(addbranchOwnerRole);
        //}

        ////course view
        //var addCourseViewRole = new IdentityRole("course.view");

        //if (_roleManager.Roles.All(r => r.Name != addCourseViewRole.Name))
        //{
        //    await _roleManager.CreateAsync(addCourseViewRole);
        //}

        ////drill view
        //var addDrillViewRole = new IdentityRole("drill.view");

        //if (_roleManager.Roles.All(r => r.Name != addDrillViewRole.Name))
        //{
        //    await _roleManager.CreateAsync(addDrillViewRole);
        //}

        ////user view
        //var addUserViewRole = new IdentityRole("user.view");

        //if (_roleManager.Roles.All(r => r.Name != addUserViewRole.Name))
        //{
        //    await _roleManager.CreateAsync(addUserViewRole);
        //}

        ////user status update
        //var addUserStatusRole = new IdentityRole("userStatus.edit");

        //if (_roleManager.Roles.All(r => r.Name != addUserStatusRole.Name))
        //{
        //    await _roleManager.CreateAsync(addUserStatusRole);
        //}
    }
}