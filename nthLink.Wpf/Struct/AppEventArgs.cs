namespace nthLink.Wpf.Struct
{
    class AppEventArgs
    {
        public string Message { get; }
        public AppEventArgs(string message)
        {
            Message = message;
        }

    public static class AppEventArgsMessage
    {
        public const string AppEvent = "AppEvent";
        public const string WindowActivated = "WindowActivated";
        public const string ApplicationShutdown = "ApplicationShutdown";
    }
    }
}
