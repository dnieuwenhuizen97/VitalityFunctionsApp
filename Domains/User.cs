using Domains.DAL;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domains
{
    [OpenApiExample(typeof(UserExample))]
    public class User : Account
    {
        [MaxLength(20)]
        [OpenApiProperty(Description = "Gets or sets the user firstName")]
        public string Firstname { get; set; }

        [MaxLength(25)]
        [OpenApiProperty(Description = "Gets or sets the user lastName")]
        public string Lastname { get; set; }

        [MaxLength(100)]
        [OpenApiProperty(Description = "Gets or sets the user job title")]
        public string JobTitle { get; set; }

        [MaxLength(100)]
        [OpenApiProperty(Description = "Gets or sets the user location")]
        public string Location { get; set; }

        [MaxLength(150)]
        [OpenApiProperty(Description = "Gets or sets the user description")]
        public string Description { get; set; }

        [OpenApiProperty(Description = "Gets or sets the users list of challenges")]
        public virtual ICollection<SubscribedChallenge> SubscribedChallenges { get; set; }

        [OpenApiProperty(Description = "Gets or sets the user id's of users that are currently following the user")]
        public virtual ICollection<Follower> Followers { get; set; }

        [MaxLength(500)]
        [OpenApiProperty(Description = "Gets or sets the user profile picture")]
        public string ProfilePicture { get; set; }

        [DefaultValue(false)]
        [OpenApiProperty(Description = "Gets or sets if the user account has been verified")]
        public bool IsVerified { get; set; }

        [DefaultValue(false)]
        [OpenApiProperty(Description = "Gets if the user account has admin priviledges")]
        public bool HasAdminPriviledges { get; set; }

        [OpenApiProperty(Description = "Gets or sets the user's total amount of points")]
        public int Points { get; set; }

        public virtual ICollection<TimelinePostDAL> TimelinePosts { get; set; }
        public virtual ICollection<NotificationDAL> Notifications { get; set; }
        public virtual ICollection<LikeDAL> Likes { get; set; }
        public virtual ICollection<CommentDAL> Comments { get; set; }

        public User(string userId, string email) : base(userId, email)
        {

        }

        public User(string userId) { }

        public User() { }

    }

    public class UserExample : OpenApiExample<User>
    {
        public override IOpenApiExample<User> Build(NamingStrategy namingStrategy = null)
        {
            var guid = Guid.NewGuid().ToString();
            Examples.Add(OpenApiExampleResolver.Resolve(
                "User Example",
               new User(guid, "gentlepossel@gmail.com")
               {
                   Firstname = "Gentle",
                   Lastname = "Possel",
                   Description = "Vitaliteit betekent voor mij lang leven!"
               }
            ));

            return this;
        }
    }
}
