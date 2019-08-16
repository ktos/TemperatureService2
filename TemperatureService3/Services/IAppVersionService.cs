using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemperatureService3.Services
{
    public interface IAppVersionService
    {
        string Version { get; }
    }
}