using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace VitalityTimerTrigger
{
    public class RecoveryTokenTimerTrigger
    {
        private IUserService _userService { get; }

        public RecoveryTokenTimerTrigger(IUserService userService)
        {
            this._userService = userService;
        }

        [Function(nameof(TokenTimerTrigger))]
        public async Task TokenTimerTrigger([TimerTrigger("0 59 23 * * *")] MyInfo myTimer, FunctionContext context)
        {
            ILogger logger = context.GetLogger("RecoveryTokenTimerTrigger");

            try
            {
                await _userService.DeleteOldRecoveryTokens();
            } 
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}