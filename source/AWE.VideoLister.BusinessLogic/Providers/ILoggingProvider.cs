namespace AWE.VideoLister.BusinessLogic.Providers
{
    public interface ILoggingProvider
    {
        public void LogInfo(string message);
        public void LogError(string message);
        public bool TryGetLastMessage(out string message);
    }
}
