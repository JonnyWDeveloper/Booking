using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booking.Core.Entities;
using Booking.Web.Data;
using Booking.Data.Data;
using Booking.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Booking.Web.Extensions;
using System.Security.Claims;

namespace Booking.Web.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                //return NotFound();
                return BadRequest();
            }

            var userId = _userManager.GetUserId(User); //USER from ApplicationUser (IdentityUser)
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);

            var userName = _userManager.GetUserName(User);
            var userMail = _userManager.Users.FirstOrDefault(u => u.Id == userId).Email;

            var userClaimEmail = User.FindFirstValue(ClaimTypes.Email);
            var userClaimId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //This only finds a BOOKED user otherwise results in null
            //var user = _context.ApplicationUserGymClass.FirstOrDefault(u => u.ApplicationUserId == userId);
            //var applicationUserID = user.ApplicationUserId;


            var allUsersEmailAddresses = _userManager.Users.Any(u => u.Email.Length > 0);//Gets all emails


            if (userId == null)
            {
                return NotFound();
            }

            //////// TEST

            var gymClass = await _context.GymClasses
               .Include(g => g.AttendingMembers)
                .FirstOrDefaultAsync(m => m.Id == id);

            var attendingMember = gymClass?.AttendingMembers.FirstOrDefault(a => a.ApplicationUserId == userId);

            //////// END TEST

            //var attendingMember = await _context.ApplicationUserGymClass.FindAsync(userId, id);

            if (attendingMember == null)
            {
                var booking = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    ApplicationUser = user, //Navigational: Adds missing ApplicationUser 
                    GymClassId = (int)id,
                    GymClass = gymClass //Navigational: Adds missing GymClass

                };

                _context.ApplicationUserGymClass.Add(booking);
            }
            else
            {
                _context.ApplicationUserGymClass.Remove(attendingMember);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        // GET: GymClasses
        public async Task<IActionResult> Index()
        {
            var model = await _context.GymClasses
                 .IgnoreQueryFilters()
                 .OrderByDescending(g => g.StartTime)
                 .ToListAsync();
            //var model = await _context.GymClasses.ToListAsync();
            return View(model);
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(c => c.Id == id);

            var gymClassResult = _context.GymClasses
               .FirstOrDefaultAsync(c => c.Id == id).Result;//Not using await

            var userId = _userManager.GetUserId(User); //Logged in current user

            var applicationUserGymClass = _context.ApplicationUserGymClass.Where(a => a.GymClassId == id);
            //All rows with the current gym class and its connected users

            //foreach (var row in applicationUserGymClass) 
            //{
            //    gymClass.AttendingMembers = row.GymClass.AttendingMembers.ToList();
            //}

            var allUsers = _userManager.Users; //All users including not logged in.

            //var allBookedUsers = new List<ApplicationUser>();

            foreach (var user in allUsers) //UserManager work // Alternative WORKS!
            {
                foreach (var row in applicationUserGymClass) //Coupling class/table work
                {
                    //if (user.Id == row.ApplicationUserId)
                    //{
                    //    allBookedUsers.Add(user);
                    //}
                }
            } //This seem to work AUTOMAGICAL!?


            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClassResult);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return Request.IsAjax() ? PartialView("CreatePartial") : View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {

            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                //return Request.IsAjax() ? PartialView("GymClassesPartial", await _context.GymClasses.ToListAsync()) : RedirectToAction(nameof(Index));
                return Request.IsAjax() ? PartialView("GymClassPartial", gymClass) : RedirectToAction(nameof(Index));
            }

            if (Request.IsAjax())
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return PartialView("CreatePartial", gymClass);
            }

            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses.FindAsync(id);

            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GymClasses == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GymClasses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.GymClasses'  is null.");
            }
            var gymClass = await _context.GymClasses.FindAsync(id);
            if (gymClass != null)
            {
                _context.GymClasses.Remove(gymClass);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return (_context.GymClasses?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
