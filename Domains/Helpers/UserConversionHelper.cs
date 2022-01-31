using Domains.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Helpers
{
    public static class UserConversionHelper
    {
        public static UserDTO ToDTO(User user)
        {
            List<FollowerDTO> followers = new List<FollowerDTO>();
            if (user.Followers != null || user.Followers.Count > 0)
                followers = FollowerConversionHelper.FollowerListToDTO(user.Followers.ToList());

            List<SubscribedChallengeDTO> challenges = new List<SubscribedChallengeDTO>();
            if (user.SubscribedChallenges != null || user.SubscribedChallenges.Count > 0)
                challenges = SubscribedChallengeConversionHelper.ListToDTO(user.SubscribedChallenges.ToList());

            return new UserDTO
            {
                UserId = user.UserId,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                JobTitle = user.JobTitle,
                Location = user.Location,
                Description = user.Description,
                Challenges = challenges,
                Followers = followers,
                ProfilePicture = user.ProfilePicture,
                Points = user.Points
            };
        }

        public static UserDTO ToDTOWithFollowingProperty(User user, string currentUserId)
        {
            bool following = false;

            List<FollowerDTO> followers = new List<FollowerDTO>();
            if (user.Followers != null || user.Followers.Count > 0)
            {
                followers = FollowerConversionHelper.FollowerListToDTO(user.Followers.ToList());

                foreach (FollowerDTO follower in followers)
                {
                    if (follower.UserFollowerId == currentUserId)
                    {
                        following = true;
                    }
                }
            }

            List<SubscribedChallengeDTO> challenges = new List<SubscribedChallengeDTO>();
            if (user.SubscribedChallenges != null || user.SubscribedChallenges.Count > 0)
                challenges = SubscribedChallengeConversionHelper.ListToDTO(user.SubscribedChallenges.ToList());

            return new UserDTO
            {
                UserId = user.UserId,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                JobTitle = user.JobTitle,
                Location = user.Location,
                Description = user.Description,
                Challenges = challenges,
                Followers = followers,
                ProfilePicture = user.ProfilePicture,
                Points = user.Points,
                Following = following
            };
        }

        public static UserSearchDTO ToSearchDTO(User user)
        {
            return new UserSearchDTO
            {
                UserId = user.UserId,
                FullName = $"{user.Firstname} {user.Lastname}",
                JobTitle = user.JobTitle,
                Location = user.Location,
                ProfilePicture = user.ProfilePicture
            };
        }

        public static List<UserSearchDTO> ListToDTO(List<User> users)
        {
            List<UserSearchDTO> userDTOs = new List<UserSearchDTO>();

            foreach (User user in users)
            {
                userDTOs.Add(ToSearchDTO(user));
            }

            return userDTOs;
        }

        public static List<ScoreboardUserDTO> ToScoreboardUserDTO(List<User> users)
        {
            List<ScoreboardUserDTO> userDTOs = new List<ScoreboardUserDTO>();

            foreach (User user in users)
            {
                userDTOs.Add(
                    new ScoreboardUserDTO
                    {
                        UserId = user.UserId,
                        FullName = $"{user.Firstname} {user.Lastname}",
                        ProfilePicture = user.ProfilePicture,
                        Points = user.Points
                    });
            }

            return userDTOs;
        }
    }
}
