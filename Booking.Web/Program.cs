using Booking.Core.Entities;
using Booking.Core.Repositories;
using Booking.Data.Data;
using Booking.Data.Repositories;
using Booking.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Booking.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;

            })
                 .AddRoles<IdentityRole>()

                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews(options =>
            {
                // options.Filters.Add<AuthorizeFilter>();

           
                //var policy = new AuthorizationPolicyBuilder()
                //                    .RequireAuthenticatedUser()
                //                    .RequireRole("Member")
                //                    .Build();

                //options.Filters.Add(new AuthorizeFilter(policy));

                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "The field is required");
            });

            //builder.Services.AddAuthorization(opt =>
            //{
            //    opt.AddPolicy("Test", policy =>
            //    {
            //        policy.RequireRole("Admin");
            //        policy.RequireClaim("Test");
            //    });
            //});

            var app = builder.Build();

            await app.SeedDataAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/GymClasses/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseMiddlewareTest();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=GymClasses}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}