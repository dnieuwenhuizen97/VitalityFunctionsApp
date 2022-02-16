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
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [OpenApiProperty(Description = "Gets or sets the activity ID")]
        public string ActivityId { get; set; }

        [MaxLength(450)]
        [OpenApiProperty(Description = "Gets or sets the comment's timelinepost id")]
        public virtual ActivityCategory Category { get; set; }


        [MaxLength(150)]
        [OpenApiProperty(Description = "Gets or sets the activity title")]
        public string Title { get; set; }

        [OpenApiProperty(Description = "Gets or sets the activity type")]
        public ChallengeType ChallengeType { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the activity description")]
        public string Description { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the activity image")]
        public string ImageLink { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the activity video")]
        public string VideoLink { get; set; }

        [OpenApiProperty(Description = "Gets or sets the activity startDate")]
        public DateTime StartDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the activity endDate")]
        public DateTime EndDate { get; set; }

        [OpenApiProperty(Description = "Gets or sets the activity url")]
        public string Url { get; set; }

        [MaxLength(100)]
        [OpenApiProperty(Description = "Gets or sets the activity location")]
        public string Location { get; set; }

        [OpenApiProperty(Description = "Gets or sets the activity points")]
        public int Points { get; set; }

        public virtual ICollection<SubscribedChallenge> SubscribedChallenges { get; set;}

        public Activity()
        {

        }

        public Activity(string title, ChallengeType challengeType, string description, DateTime startDate, DateTime endDate, string location, int points)
        {
            this.ActivityId = Guid.NewGuid().ToString();
            this.Title = title;
            this.ChallengeType = challengeType;
            this.Description = description;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Location = location;
            this.Points = points;
        }
    }
}

