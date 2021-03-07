namespace BunniBot.Database.Models
{
    public class ActionsModel
    {
        public string ActionType;
        public string ActionReason;

        public ActionsModel(string actionType, string actionReason)
        {
            ActionType = actionType;
            ActionReason = actionReason;
        }
        
        public string GetActionType()
        {
            return ActionType;
        }

        public string GetReason()
        {
            return ActionReason;
        }
    }
}