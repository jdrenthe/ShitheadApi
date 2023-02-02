using ShitheadApi.Models.Analytics;
using ShitheadApi.Models;
using System.Threading.Tasks;

namespace ShitheadApi.Services
{
    public interface IAnalyticsService
    {
        /// <summary>
        /// Adds device or updates device
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        Task<DataBundel<Device>> AddOrUpdateDevice(Device device);
    }
}
