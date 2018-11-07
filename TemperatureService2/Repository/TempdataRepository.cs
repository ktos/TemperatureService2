using System.Collections.Generic;
using System.Linq;
using TempHistory.Models;

namespace TempHistory.Repository
{
    public class TempdataRepository : ITempdataRepository
    {
        private readonly TempdataDbContext _context;

        public TempdataRepository(TempdataDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Tempdata> GetLast100OutdoorData()
        {
            return _context.Tempdata.Where(x => x.Sensor == "outdoor").OrderByDescending(x => x.Timestamp).Take(100).ToList();
        }
    }
}