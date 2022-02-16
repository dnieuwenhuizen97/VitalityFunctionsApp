using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(PushTokenExample))]
    public class PushToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the PushTokenId")]
        public string PushTokenId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the token")]
        public string Token { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the userId for the PushToken")]
        public virtual User User { get; set; }

        [OpenApiProperty(Description = "Gets or sets the type of the Device. [Android or iOS]")]
        public DeviceType DeviceType { get; set; }

        [OpenApiProperty(Description = "Gets or sets if the user wants to receive notifications")]
        public bool NotificationEnabled { get; set; }

        public PushToken()
        {

        }
    }

    public class PushTokenExample : OpenApiExample<PushToken>
    {
        public override IOpenApiExample<PushToken> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "PushTokenExample",
                new PushToken()
                {
                    User = new User(),
                    PushTokenId = Guid.NewGuid().ToString(),
                    DeviceType = DeviceType.Android,
                    NotificationEnabled = true
                }));
            return this;
        }
    }
}
