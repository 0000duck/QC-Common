using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using QuantumConcepts.Common.Net.Rest.Transmission;

namespace QuantumConcepts.Common.Net.Rest
{
    public interface IRestApi
    {
        /// <summary>The Application Name - this will be used as (among other things) the User Agent for each request.</summary>
        string ApplicationName { get; }

        /// <summary>The number of times to retry each request.</summary>
        int NumberOfRetries { get; }

        /// <summary>Allows the API to make adjustments to the request before it is sent. For instance, adding authentication information.</summary>
        /// <param name="url">The URL of the request - this does not include the BaseUrl.</param>
        /// <param name="method">The HTTP method of the request.</param>
        /// <param name="apiParameters">The API Parameters which are included in the request.</param>
        /// <param name="apiMimeParameters">The MIME parameters which are included in the request.</param>
        /// <returns>The modified full URL, or null.</returns>
        string BeforeSendRequest(string url, HttpMethod method, List<ApiParameter> apiParameters, List<ApiMimeParameter> apiMimeParameters);
    }
}
