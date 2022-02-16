using Domains;
using Domains.DTO;
using Domains.Enums;
using Infrastructure.Context.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public class ActivityDb : IActivityDb
    {
        private readonly DbContextDomains _dbContext;

        public ActivityDb(DbContextDomains dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ActivityCategory> CreateActivityCategory(ActivityCategory activityCategory)
        {
            try
            {
                await _dbContext.ActivityCategories.AddAsync(activityCategory);
                await _dbContext.SaveChangesAsync();

                return activityCategory;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ActivityCategory> DeleteActivityCategory(string categoryId)
        {
            try
            {
                ActivityCategory category = await _dbContext.ActivityCategories.FindAsync(categoryId);

                if (category is null) throw new DbUpdateException();

                _dbContext.ActivityCategories.Remove(category);
                await _dbContext.SaveChangesAsync();

                return category;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<Activity> CreateActivity(Activity activity)
        {
            try
            {
                await _dbContext.Activities.AddAsync(activity);
                await _dbContext.SaveChangesAsync();

                return activity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<Activity> DeleteActivity(string activityId)
        {
            try
            {
                Activity activity = await _dbContext.Activities.FindAsync(activityId);

                if (activity is null) throw new DbUpdateException();

                _dbContext.Activities.Remove(activity);
                await _dbContext.SaveChangesAsync();

                return activity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        Task<List<Activity>> IActivityDb.GetActivities(string categoryId, int limit, int offset)
        {
            throw new NotImplementedException();
        }

        Task<Activity> IActivityDb.GetActivity(string activityId)
        {
            throw new NotImplementedException();
        }

        Task<ActivityCategory> IActivityDb.GetActivityCategory(string categoryId)
        {
            throw new NotImplementedException();
        }

        Task<List<User>> IActivityDb.GetActivityParticipants(string activityId)
        {
            throw new NotImplementedException();
        }

        Task<Activity> IActivityDb.GetAdvancedActivity(string activityId)
        {
            throw new NotImplementedException();
        }

        Task<ActivityCategory[]> IActivityDb.GetAllActivityCategories(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public async Task<Activity> UpdateActivity(ActivityUpdatePropertiesDTO propertiesToUpdate, string activityId)
        {
            try
            {
                Activity activity = await _dbContext.Activities.FindAsync(activityId);

                foreach (var property in activity.GetType().GetProperties())
                {
                    var currentProperty = propertiesToUpdate.GetType().GetProperty(property.Name);

                    if (currentProperty != null)
                    {
                        property.SetValue(property, currentProperty.GetValue(currentProperty));
                    }
                }

                await _dbContext.SaveChangesAsync();

                return activity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        Task<ActivityCategory> IActivityDb.UpdateActivityCategory(ActivityCategoryUpdatePropertiesDTO propertiesToUpdate, string categoryId)
        {
            throw new NotImplementedException();
        }

        Task IActivityDb.UpdateActivityImage(string activityId, string imageUrl)
        {
            throw new NotImplementedException();
        }

        Task IActivityDb.UpdateActivityVideo(string activityId, string videoUrl)
        {
            throw new NotImplementedException();
        }

        Task<string> IActivityDb.UpdateProgess(string activityId, ChallengeProgress progress, string currentUserId)
        {
            throw new NotImplementedException();
        }
    }
}
