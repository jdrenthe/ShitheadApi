using System.Threading.Tasks;
using Flurl.Http;
using ShitheadApi.Models;
using ShitheadApi.Models.Analytics;
using ShitheadApi.Utilities.Enums;

namespace ShitheadApi.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IRequestService _requestService;

        public AnalyticsService(IRequestService requestService)
        {
            _requestService = requestService;
        }

        /// <inheritdoc/>
        public async Task<DataBundel<Device>> AddOrUpdateDevice(Device device)
        {
            var request = _requestService.CreateRequest("AddOrUpdateDevice", "api/DeviceContoller", ApiDomainType.AnalyticsApi);
            return await request.PostJsonAsync(device).ReceiveJson<DataBundel<Device>>();
        }
    }
}
