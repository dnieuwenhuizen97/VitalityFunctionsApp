using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains
{
    [OpenApiExample(typeof(TimeLinePostCreationRequestExample))]
    public class TimelinePostCreationRequest
    {
        [OpenApiProperty(Description = "Gets or sets the text content of the timeline post")]
        public string Text { get; set; }

        [OpenApiProperty(Description = "Gets or sets the video content of the timeline post")]
        public List<StreamContentDTO> ImagesAndVideos { get; set; }

        public TimelinePostCreationRequest(string text)
        {
            this.Text = text;
        }

        public TimelinePostCreationRequest(string text, List<StreamContentDTO> imageOrVideo) : this(text)
        {
            ImagesAndVideos = imageOrVideo;
        }
    }

    public class TimeLinePostCreationRequestExample : OpenApiExample<TimelinePostCreationRequest>
    {
        public override IOpenApiExample<TimelinePostCreationRequest> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve(
                "Timeline post creation example",
                new TimelinePostCreationRequest(
                    "Vandaag 20 Jumping Jacks gedaan. Wie doet er morgen mee?"
                ),
                NamingStrategy));

            return this;
        }
    }

    public class StreamContentDTO
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Name { get; set; }
        public Stream Data { get; set; }
        public StreamContentDTO()
        {

        }

        public StreamContentDTO(string conentType, string fileName, string name, Stream data)
        {
            ContentType = conentType;
            FileName = fileName;
            Name = name;
            Data = data;
        }
    }

    public class ParametersKeys
    { 
        public string Text { get; set; }
        public string Data { get; set; }
    
    }
}
