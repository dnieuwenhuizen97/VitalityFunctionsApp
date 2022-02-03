using Domains.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domains.Helpers
{
    public static class FollowerConversionHelper
    {
        public static FollowerDTO ToDTO(Follower follower)
        {
            return new FollowerDTO
            {
                UserFollowerId = follower.UserFollower
            };
        }

        public static List<FollowerDTO> FollowerListToDTO(List<Follower> followers)
        {
            List<FollowerDTO> followerDTOs = new List<FollowerDTO>();

            foreach (Follower f in followers)
            {
                followerDTOs.Add(new FollowerDTO { UserFollowerId = f.UserFollower });
            }

            return followerDTOs;
        }
    }
}
