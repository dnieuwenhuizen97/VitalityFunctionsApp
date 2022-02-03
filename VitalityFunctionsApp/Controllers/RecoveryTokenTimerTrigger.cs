using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace VitalityFunctionsApp.Controllers
{
    public class RecoveryTokenTimerTrigger
    {
        IUserService _userService;

        public RecoveryTokenTimerTrigger(IUserService userService)
        {
            this._userService = userService;
        }

        [Function(nameof(RemoveRecoveryTokens))]
        public async Task RemoveRecoveryTokens([TimerTrigger("30 8 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("RecoveryTokenTimerTrigger");
            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            await _userService.RemoveOldRecoveryTokens();

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
