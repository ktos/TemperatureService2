using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TemperatureService3.Models;
using TemperatureService3.Repository;
using TemperatureService3.ViewModels;

namespace TemperatureService3.Controllers
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
            var sensors = _repository.GetAllSensorsWithLastValues();

            ViewData["Title"] = "Dashboard";
            return View(new IndexViewModel(sensors));
        }

        public IActionResult Sensor(string name)
        {
            var sensor = _repository.GetSensor(name);

            if (sensor != null)
            {
                ViewData["Title"] = sensor.Name;

                var vm = new SensorPageViewModel
                {
                    AllSensors = new IndexViewModel(_repository.GetAllSensors()).Sensors,
                    Sensor = SensorViewModel.FromSensor(sensor),
                    Last24Hours = _repository.GetSensorHistoryLast24Hours(name),
                    LastWeek = _repository.GetSensorHistoryLastDays(name, 7),
                    LastMonth = _repository.GetSensorHistoryLastDays(name, 30),
                    LastYear = _repository.GetSensorHistoryLastDays(name, 365),
                    LastYearByMonth = _repository.GetSensorHistoryLastYear(name)
                };

                return View(vm);
            }
            else
                return NotFound();
        }

        [Authorize]
        [FormatFilter]
        [HttpPost("/{name}"), HttpPut("/{name}")]
        public IActionResult UpdateSensor([FromRoute]string name, [FromBody]SensorDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var sensor = _repository.GetSensor(name);

            if (model.Name == null)
                model.Name = name;

            var result = (sensor != null) ? _repository.UpdateSensor(model) : _repository.AddSensor(model);
            if (!result)
                return BadRequest();

            var svm = SensorViewModel.FromSensor(_repository.GetSensor(name));
            return CreatedAtAction("Sensor", svm);
        }

        [FormatFilter]
        public IActionResult SensorInAnotherFormat(string name)
        {
            var sensor = _repository.GetSensor(name);

            if (sensor != null)
                return Ok(SensorViewModel.FromSensor(sensor));
            else
                return NotFound();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ErrorCode(int code)
        {
            return View(new ErrorCodeViewModel { ErrorCode = code });
        }
    }
}