using System.Collections.Generic;
using System.Threading.Tasks;
using BunniBot.Database.Models;

namespace BunniBot.Database
{
    public class AdminLogHandler
    {
        public async Task AddLogAsync(long id, string username, string type, string reason)
        {
            var mongoDbHandler = new MongoDBHandler("UserLogs");
            var actionModel = new ActionsModel(type, reason);
            var currentEntry = mongoDbHandler.LoadRecordById<UserLogsModel>("Logs", id);
            var actions = new List<ActionsModel>();
            
            if (currentEntry != null)
            {
                actions = currentEntry.Actions;
            }
            
            actions.Add(actionModel);
            
            var userLogsModel = new UserLogsModel(id, username , actions);
            
            await mongoDbHandler.Upsert("Logs", userLogsModel.UserId, userLogsModel);
        }
    }
}