using Domains.DTO;
using Domains.Enums;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    public class ActivityCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the category ID")]
        public string CategoryId { get; set; }

        [MaxLength(150)]
        [OpenApiProperty(Description = "Gets or sets the category title")]
        public string Title { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the challenge description")]
        public string Description { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the challenge image")]
        public string ImageLink { get; set; }
 
        [OpenApiProperty(Description = "Gets or sets the category (in)active")]
        public bool IsActive { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }

        public ActivityCategory(string title, string description, string imageLink, bool isActive)
        {
            this.CategoryId = Guid.NewGuid().ToString();
            this.Title = title;
            this.Description = description;
            this.ImageLink = ImageLink;
            this.IsActive = isActive;
        }
    }
}

