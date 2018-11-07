using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TemperatureService2.Models;
using TemperatureService2.Repository;

namespace TemperatureService2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITempdataRepository _repository;

        public HomeController(ITempdataRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetLast100OutdoorData());
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}