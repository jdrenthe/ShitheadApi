using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShitheadApi.Models;
using ShitheadApi.Models.Analytics;
using ShitheadApi.Services;

namespace ShitheadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpPost]
        public async Task<DataBundel<Device>> AddOrUpdateDevice(Device device)
        {
            try
            {
                if (HttpContext == null || HttpContext.Request == null || HttpContext.Request.Headers == null) return new(null, false, null);

                var userAgent = HttpContext.Request.Headers["User-Agent"];
                if (string.IsNullOrEmpty(userAgent)) return new(null, false, null);

                device.UserAgent = userAgent;

                // Se origin: https://stackoverflow.com/a/41335701/6572268
                var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
                device.IPAddress = remoteIpAddress.MapToIPv4().ToString();
            }
            catch
            {
                device.IPAddress = null;
            }

            return await _analyticsService.AddOrUpdateDevice(device);
        }
    }
}
