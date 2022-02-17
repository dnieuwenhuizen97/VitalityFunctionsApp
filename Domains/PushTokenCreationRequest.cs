using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    [OpenApiExample(typeof(PushTokenCreationRequestExample))]
    public class PushTokenCreationRequest
    {
        [OpenApiProperty(Description = "Gets or sets the pushtoken")]
        [JsonRequired]
        public string PushToken { get; set; }

        [OpenApiProperty(Description = "Gets or sets the device type for the pushtoken")]
        [JsonRequired]
        public DeviceType DeviceType { get; set; }
    }

    public class PushTokenCreationRequestExample : OpenApiExample<PushTokenCreationRequest>
    {
        public override IOpenApiExample<PushTokenCreationRequest> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Push token creation request Example",
                new PushTokenCreationRequest
                {
                    PushToken = Guid.NewGuid().ToString(),
                    DeviceType = DeviceType.Android
                }));
            return this;
        }

    }
}
