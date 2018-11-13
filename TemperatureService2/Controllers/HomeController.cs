using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TemperatureService2.Models;
using TemperatureService2.Repository;
using TemperatureService2.ViewModels;

namespace TemperatureService2.Controllers
{
    public class HomeController : Controller
    {        
        private readonly ISensorRepository _repository;

        public HomeController(ISensorRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View(_repository.GetAllSensors());
        }

        public IActionResult Sensor(string name)
        {
            var sensor = _repository.GetSensor(name);
            return View(new SensorViewModel(sensor));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}