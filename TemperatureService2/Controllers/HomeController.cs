﻿using Microsoft.AspNetCore.Authorization;
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

            if (sensor != null)
                return View(new SensorViewModel(sensor));
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

            var svm = new SensorViewModel(_repository.GetSensor(name));
            return CreatedAtAction("Sensor", svm);
        }

        [FormatFilter]
        public IActionResult SensorInAnotherFormat(string name)
        {
            var sensor = _repository.GetSensor(name);

            if (sensor != null)
                return Ok(new SensorViewModel(sensor));
            else
                return NotFound();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}