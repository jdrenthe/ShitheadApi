using System.Collections.Generic;
using System.Linq;
using Flurl;
using Flurl.Http;
using ShitheadApi.Utilities.Enums;

namespace ShitheadApi.Services
{
    public class RequestService : IRequestService
    {
        /// <summary>
        /// Url for the analytics api
        /// </summary>
        public static string AnalyticsApiUrl => "https://localhost:44358/";

        /// <summary>
        /// Url for the notification api
        /// TODO: This url is the yasurf api url, update the url when the notification api is online
        /// </summary>
        public static string NotificationApiUrl => "https://localhost:44373/";

        /// <inheritdoc/>
        public IFlurlRequest CreateRequest(string resource,
                                           string apiPathSegment,
                                           ApiDomainType apiDomainType)
        {
            return CreateUrl(resource, apiPathSegment, apiDomainType).AllowAnyHttpStatus();
        }

        /// <inheritdoc/>
        public IFlurlRequest CreateRequest(string resource,
                                           Dictionary<string, string> queryParams,
                                           string apiPathSegment,
                                           ApiDomainType apiDomainType)
        {
            var url = CreateUrl(resource, apiPathSegment, apiDomainType);
            if (queryParams.Any()) url.SetQueryParams(queryParams);

            return url.AllowAnyHttpStatus();
        }

        /// <summary>
        /// Creates url
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static Url CreateUrl(string resource, string apiPathSegment, ApiDomainType type)
        {
            var url = type switch
            {
                ApiDomainType.AnalyticsApi => AnalyticsApiUrl,
                ApiDomainType.NotificationApi => NotificationApiUrl,
                _ => string.Empty
            };

            return new Url(url)
               .AppendPathSegment(apiPathSegment)
               .AppendPathSegment(resource);
        }
    }
}
