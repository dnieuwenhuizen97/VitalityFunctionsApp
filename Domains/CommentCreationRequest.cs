using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    [OpenApiExample(typeof(CommentCreationRequestExample))]
    public class CommentCreationRequest
    {
        [OpenApiProperty(Description = "Gets or sets the text content of the comment")]
        public string Text { get; set; }

        public CommentCreationRequest(string text)
        {
            this.Text = text;
        }
    }

    public class CommentCreationRequestExample : OpenApiExample<CommentCreationRequest>
    {
        public override IOpenApiExample<CommentCreationRequest> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Comment creation example",
                new CommentCreationRequest(
                    "Lekker bezig! Ik sluit aan bij de volgende challenge.."
                ),
                NamingStrategy));

            return this;
        }
    }
}
