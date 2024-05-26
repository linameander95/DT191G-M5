using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using puremy.Models;
using puremy.Data;
using Microsoft.EntityFrameworkCore;

namespace puremy.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogPostContext _context;

        public HomeController(BlogPostContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogPosts.ToListAsync());
        }
    }
}