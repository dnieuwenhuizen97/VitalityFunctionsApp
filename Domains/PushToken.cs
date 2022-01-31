using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;

namespace Domains
{
    [OpenApiExample(typeof(PushTokenExample))]
    public class PushToken
    {
        [OpenApiProperty(Description = "Gets or sets the userId for the PushToken")]
        public string UserId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the PushTokenId")]
        public string PushTokenId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the type of the Device. [Android or iOS]")]
        public DeviceType DeviceType { get; set; }

        [OpenApiProperty(Description = "Gets or sets if the user wants to recieve notifications")]
        public bool NotificationEnabled { get; set; }
    }

    public class PushTokenExample : OpenApiExample<PushToken>
    {
        public override IOpenApiExample<PushToken> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "PushTokenExample",
                new PushToken()
                {
                    UserId = Guid.NewGuid().ToString(),
                    PushTokenId = Guid.NewGuid().ToString(),
                    DeviceType = DeviceType.Android,
                    NotificationEnabled = true
                }));
            return this;
        }
    }
}
