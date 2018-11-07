using System.Collections.Generic;
using TempHistory.Models;

namespace TempHistory.Repository
{
    public interface ITempdataRepository
    {
        IEnumerable<Tempdata> GetLast100OutdoorData();
    }
}