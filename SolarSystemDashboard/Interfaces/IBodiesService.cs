using SolarSystemDashboard.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SolarSystemDashboard.Interfaces;

public interface IBodiesService
{
    Task<Bodies> GetBodiesAsync(CancellationToken cancellationToken = default);
}
