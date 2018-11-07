using System.Collections.Generic;
using TemperatureService2.Models;

namespace TemperatureService2.Test.Helpers
{
    public static class Utilities
    {
        public static void InitializeDbForTests(TempdataDbContext db)
        {
            db.Tempdata.AddRange(GetSeedingMessages());
            db.SaveChanges();
        }

        public static List<Tempdata> GetSeedingMessages()
        {
            return new List<Tempdata>()
            {
                new Tempdata { Sensor = "outdoor", Timestamp = 1541621897, Value = 7.25f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541622473, Value = 19.75f },
                new Tempdata { Sensor = "outdoor", Timestamp = 1541622474, Value = 7.19f },
                new Tempdata { Sensor = "leia", Timestamp = 1541622603, Value = 25.562f },
                new Tempdata { Sensor = "indoor2", Timestamp = 1541622605,Value = 24.937f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541623050, Value = 19.81f },
                new Tempdata { Sensor = "leia", Timestamp = 1541623503, Value = 25.5f },
                new Tempdata { Sensor = "indoor2", Timestamp = 1541623504,Value = 25.187f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541623627,Value = 19.81f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541624203,Value = 19.69f },
                new Tempdata { Sensor = "outdoor", Timestamp = 1541624204, Value = 7.19f },
                new Tempdata { Sensor = "leia", Timestamp = 1541624403, Value = 25.375f },
                new Tempdata { Sensor = "indoor2", Timestamp = 1541624405,Value = 24.625f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541624780, Value = 19.69f },
                new Tempdata { Sensor = "leia", Timestamp = 1541625303, Value = 25.312f },
                new Tempdata { Sensor = "indoor2", Timestamp = 1541625305,Value = 24.937f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541625357,Value = 19.69f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541625934,Value = 19.87f },
                new Tempdata { Sensor = "leia", Timestamp = 1541626203, Value = 25.25f },
                new Tempdata { Sensor = "indoor2", Timestamp = 1541626205, Value = 25.437f },
                new Tempdata { Sensor = "indoor", Timestamp = 1541626509, Value = 19.81f }
            };
        }
    }
}