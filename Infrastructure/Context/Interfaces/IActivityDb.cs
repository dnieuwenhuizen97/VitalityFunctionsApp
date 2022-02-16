using Domains;
using Domains.DTO;
using Domains.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Context.Interfaces
{
    public interface IActivityDb
    {
        Task<ActivityCategory> CreateActivityCategory(ActivityCategory activityCategory);
        Task<ActivityCategory> UpdateActivityCategory(ActivityCategoryUpdatePropertiesDTO propertiesToUpdate, string categoryId);   
        Task<ActivityCategory> DeleteActivityCategory(string categoryId); 
        Task<ActivityCategory> GetActivityCategory(string categoryId);
        Task<ActivityCategory[]> GetAllActivityCategories(int limit, int offset);

        Task<Activity> CreateActivity(Activity activity);   
        Task<Activity> UpdateActivity(ActivityUpdatePropertiesDTO propertiesToUpdate, string activityId);       
        Task<Activity> DeleteActivity(string activityId);
        Task<Activity> GetActivity(string activityId); 
        Task<Activity> GetAdvancedActivity(string activityId);
        Task<List<Activity>> GetActivities(string categoryId, int limit, int offset);
        Task<List<User>> GetActivityParticipants(string activityId);
        Task<string> UpdateProgess(string activityId, ChallengeProgress progress, string currentUserId);
        Task UpdateActivityImage(string activityId, string imageUrl);
        Task UpdateActivityVideo(string activityId, string videoUrl);
    }
}
