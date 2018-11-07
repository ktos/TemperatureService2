using System.Collections.Generic;
using TemperatureService2.Models;

namespace TemperatureService2.Repository
{
    public interface ITempdataRepository
    {
        IEnumerable<Tempdata> GetLast100OutdoorData();
    }
}