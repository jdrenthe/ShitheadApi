using System.Collections.Generic;
using Flurl.Http;
using ShitheadApi.Utilities.Enums;

namespace ShitheadApi.Services
{
    public interface IRequestService
    {
        /// <summary>
        /// Creates a request to the api.
        /// </summary>
        /// <param name="resource">Api method name as a string.</param>
        /// <param name="apiPathSegment">Api path segment as a string</param>
        /// <returns>A request to set up a connection with the api.</returns>
        IFlurlRequest CreateRequest(string resource, string apiPathSegment, ApiDomainType apiDomainType);

        /// <summary>
        /// Creates a request to the api.
        /// </summary>
        /// <param name="resource">Api method name as a string.</param>
        /// <param name="queryParams"></param>
        /// <param name="apiPathSegment">Api path segment as a string</param>
        /// <returns>A request to set up a connection with the api.</returns>
        IFlurlRequest CreateRequest(string resource, Dictionary<string, string> queryParams, string apiPathSegment, ApiDomainType apiDomainType);
    }
}
