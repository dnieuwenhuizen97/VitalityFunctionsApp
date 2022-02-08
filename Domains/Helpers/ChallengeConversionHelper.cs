using Domains.DTO;
using Domains.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Helpers
{
    public static class ChallengeConversionHelper
    {

        public static ChallengeDTO ToDTOWithProgress(Challenge challenge, ChallengeProgress progress, int totalSubscribers)
        {
            return new ChallengeDTO
            {
                ChallengeId = challenge.ChallengeId,
                Title = challenge.Title,
                ChallengeType = challenge.ChallengeType,
                Description = challenge.Description,
                ImageLink = challenge.ImageLink,
                VideoLink = challenge.VideoLink,
                StartDate = challenge.StartDate,
                EndDate = challenge.EndDate,
                Location = challenge.Location,
                Points = challenge.Points,
                ChallengeProgress = progress,
                TotalSubscribers = totalSubscribers
            };
        }

        public static ChallengeDTO ToDTOWithoutProgress(Challenge challenge)
        {
            return new ChallengeDTO
            {
                ChallengeId = challenge.ChallengeId,
                Title = challenge.Title,
                ChallengeType = challenge.ChallengeType,
                Description = challenge.Description,
                ImageLink = challenge.ImageLink,
                VideoLink = challenge.VideoLink,
                StartDate = challenge.StartDate,
                EndDate = challenge.EndDate,
                Location = challenge.Location,
                Points = challenge.Points
            };
        }

        public static Challenge ToDAL(ChallengeDTO challngeDAL)
        {
            return new Challenge
            {
                Title = challngeDAL.Title,
                ChallengeType = challngeDAL.ChallengeType,
                Description = challngeDAL.Description,
                ImageLink = challngeDAL.ImageLink,
                VideoLink = challngeDAL.VideoLink,
                StartDate = challngeDAL.StartDate,
                EndDate = challngeDAL.EndDate,
                Location = challngeDAL.Location,
                Points = challngeDAL.Points
            };
        }

        public static Challenge RequestToChallenge(ChallengeCreationRequest request)
        {
            return new Challenge
            {
                Title = request.Title,
                ChallengeType = request.ChallengeType,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Location = request.Location,
                Points = request.Points
            };
        }

        public static SubscribedUsersDTO SubscriberToDTO(User user)
        {
            return new SubscribedUsersDTO
            {
                UserId = user.UserId,
                ProfilePicture = user.ProfilePicture,
                FullName = $"{user.Firstname} {user.Lastname}",
                JobTitle = user.JobTitle
            };
        }

        //public static SubscribedUsersDAL SubscriberToDAL(SubscribedUsersDTO x)
        //{
        //    return new SubscribedUsersDAL
        //    {
        //        UserId = x.UserId,
        //        Timestamp = x.Timestamp,
        //        ImageUrl = x.ImageUrl,
        //        Firstname = x.FullName.Split(' ')[0].ToString(),
        //        Lastname = x.FullName.Split(' ')[1].ToString()
        //    };
        //}
    }
}
