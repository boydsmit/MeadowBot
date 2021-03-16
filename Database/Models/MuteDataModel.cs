namespace BunniBot.Database.Models
{
    public class MuteDataModel
    {
        public bool IsMuted;
        public string GivenTime;
        public long UnmuteTimeAsBinary;

        public MuteDataModel(bool isMuted, string givenTime, long unmuteTime)
        {
            IsMuted = isMuted;
            GivenTime = givenTime;
            UnmuteTimeAsBinary = unmuteTime;
        }

        public bool GetIsMuted()
        {
            return IsMuted;
        }

        public long GetUnmuteTimeAsBinary()
        {
            return UnmuteTimeAsBinary;
        }
    }
}