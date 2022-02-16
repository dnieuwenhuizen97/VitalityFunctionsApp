using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.DTO
{
    [OpenApiExample(typeof(ActivityCategoryUpdatePropertiesDTO))]
    public class ActivityCategoryUpdatePropertiesDTO
    {
        [OpenApiProperty(Description = "Gets or sets the title of the activity category that is being updated")]
        public string Title { get; set; }

        [OpenApiProperty(Description = "Gets or sets the description of the activity category that is being updated")]
        public string Description { get; set; }

        [OpenApiProperty(Description = "Gets or sets the imagelink of the activity category that is being updated")]
        public string ImageLink { get; set; }

        [OpenApiProperty(Description = "Gets or sets the activity state of the activity category that is being updated")]
        public bool IsActive { get; set; }
    }

    public class ActivityCategoryUpdatePropertiesExample : OpenApiExample<ActivityCategoryUpdatePropertiesDTO>
    {
        public override IOpenApiExample<ActivityCategoryUpdatePropertiesDTO> Build(NamingStrategy namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Activity Category Update Example",
               new ActivityCategoryUpdatePropertiesDTO
               {
                   Title = "Yoga",
                   Description = "In deze categorie staan yoga activiteiten centraal",
                   IsActive = true
               }
            ));

            return this;
        }
    }
}
