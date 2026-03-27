using Hackathon_Portal.Data;
using Hackathon_Portal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Controllers;

public class EventController : Controller
{
    private readonly ApplicationDbContext _context;

    public EventController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Event
    public async Task<IActionResult> Index()
    {
        var events = await _context.Events.ToListAsync();

        return View(events);
    }

    // GET: /Event/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.EventRegistrations)
            .ThenInclude(registration => registration.Team)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem == null)
        {
            return NotFound();
        }

        return View(eventItem);
    }

    // POST: /Event/Create
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,StartDate,EndDate,IsRegistrationOpen")] Event eventItem)
    {
        if (!ModelState.IsValid)
        {
            return View(eventItem);
        }

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /Event/Edit/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,StartDate,EndDate,IsRegistrationOpen")] Event eventItem)
    {
        if (id != eventItem.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(eventItem);
        }

        _context.Events.Update(eventItem);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /Event/Register
    [HttpPost]
    [Authorize(Roles = "Participant")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int eventId, int teamId)
    {
        var eventExists = await _context.Events.AnyAsync(e => e.Id == eventId);
        var teamExists = await _context.Teams.AnyAsync(t => t.Id == teamId);

        if (!eventExists || !teamExists)
        {
            return NotFound();
        }

        var alreadyRegistered = await _context.EventRegistrations
            .AnyAsync(er => er.EventId == eventId && er.TeamId == teamId);

        if (!alreadyRegistered)
        {
            _context.EventRegistrations.Add(new EventRegistration
            {
                EventId = eventId,
                TeamId = teamId
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Details), new { id = eventId });
    }
}