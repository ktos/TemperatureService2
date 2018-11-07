using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TempHistory.Models;

namespace TempHistory.Controllers
{
    public class HomeController : Controller
    {
        private readonly TempdataDbContext _context;

        public HomeController(TempdataDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Tempdata.Where(x => x.Sensor == "outdoor").OrderByDescending(x => x.Timestamp).Take(100).ToList());            
        }        

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
