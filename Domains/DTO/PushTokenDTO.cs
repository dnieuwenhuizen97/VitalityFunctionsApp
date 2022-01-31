using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;

namespace Domains.DTO
{
    [OpenApiExample(typeof(PushTokenDTOExample))]
    public class PushTokenDTO
    {
        [OpenApiProperty(Description = "Gets or sets the user id for the pushtoken dto")]
        public string UserId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the pushtoken id for the pushtoken dto")]
        public string PushTokenId { get; set; }

        [OpenApiProperty(Description = "Gets or sets the device type for the pushtoken dto")]
        public DeviceType DeviceType { get; set; }

        [OpenApiProperty(Description = "Gets or sets if notifications are enabled for the pushtoken dto")]
        public bool NotificationEnabled { get; set; }

        public PushTokenDTO()
        {

        }
    }

    public class PushTokenDTOExample : OpenApiExample<PushTokenDTO>
    {
        public override IOpenApiExample<PushTokenDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Push Token DTO Example",
                new PushTokenDTO { 
                    UserId = Guid.NewGuid().ToString(),
                    PushTokenId = Guid.NewGuid().ToString(),
                    DeviceType = DeviceType.iOS,
                    NotificationEnabled = true
                }));
            return this;
        }

    }
}
