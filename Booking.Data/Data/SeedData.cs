using Bogus;
using Booking.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Data
{
    public class SeedData
    {
        private static ApplicationDbContext db = default!;
        private static RoleManager<IdentityRole> roleManager = default!;
        private static UserManager<ApplicationUser> userManager = default!;
        public static async Task InitAsync(ApplicationDbContext context, IServiceProvider services, string adminPW)
        {
            ArgumentNullException.ThrowIfNull(nameof(context));
            db = context;
            if (db.GymClasses.Any())
                return;
            ArgumentNullException.ThrowIfNull(nameof(services));
            ArgumentNullException.ThrowIfNull(adminPW, nameof(adminPW));

            roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            ArgumentNullException.ThrowIfNull(roleManager);

            userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            ArgumentNullException.ThrowIfNull(userManager);

            var roleNames = new[] { "Member", "Admin" };
            var adminEmail = "admin@gym.se";

            var gymClasses = GetGymClasses(20);
            await db.AddRangeAsync(gymClasses);

            await AddRolesAsync(roleNames);

            var admin = await AddAdminAsync(adminEmail, adminPW);

            await AddToRolesAsync(admin, roleNames);
        }

        private static async Task AddToRolesAsync(ApplicationUser admin, string[] roleNames)
        {
            foreach (var role in roleNames)
            {
                if (await userManager.IsInRoleAsync(admin, role))
                    continue;
                var result = await userManager.AddToRoleAsync(admin, role);
                if (!result.Succeeded)
                    throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static async Task<ApplicationUser> AddAdminAsync(string adminEmail, string adminPW)
        {
            var found = await userManager.FindByEmailAsync(adminEmail);

            if (found != null)
                return null!;

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                TimeOfRegistration = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, adminPW);
            if (!result.Succeeded)
                throw new Exception(string.Join("\n", result.Errors));

            return admin;
        }

        private static async Task AddRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;
                var role = new IdentityRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                    throw new Exception(string.Join("\n", result.Errors));
            }
        }

        private static IEnumerable<GymClass> GetGymClasses(int nrOfGymclasses)
        {
            var faker = new Faker("sv");

            var gymClasses = new List<GymClass>();

            for (int i = 0; i < nrOfGymclasses; i++)
            {
                var temp = new GymClass
                {
                    Name = faker.Company.CatchPhrase(),
                    Description = faker.Hacker.Verb(),
                    Duration = new TimeSpan(0, 55, 0),
                    StartTime = DateTime.Now.AddDays(faker.Random.Int(-5, 5))
                };

                gymClasses.Add(temp);
            }

            return gymClasses;
        }

        private static IEnumerable<GymClass> GetGymClasses2(int nrOfGymclasses)
        {
            var faker = new Faker<GymClass>("sv").Rules((f, g) =>
            {
                g.Name = f.Company.CatchPhrase();
                g.Description = f.Hacker.Verb();
                g.Duration = new TimeSpan(0, 55, 0);
                g.StartTime = DateTime.Now.AddDays(f.Random.Int(-5, 5));
            });

            return faker.Generate(nrOfGymclasses);
        }

        private static IEnumerable<GymClass> GetGymClasses3(int nrOfGymclasses)
        {
            var faker = new Faker<GymClass>("sv")
                .RuleFor(g => g.Name, f => f.Company.CatchPhrase())
                .RuleFor(g => g.Description, f => f.Hacker.Verb())
                .RuleFor(g => g.Duration, new TimeSpan(0, 55, 0))
                .RuleFor(g => g.StartTime, f => DateTime.Now.AddDays(f.Random.Int(-5, 5)));

            return faker.Generate(nrOfGymclasses);
        }
    }
}
