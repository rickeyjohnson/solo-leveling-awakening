namespace SLA.UI
{
    /// <summary>Window color by meaning: blue information, gold reward, red warning (GDD: The System UI).</summary>
    public enum SystemMessageKind
    {
        Info,
        Reward,
        Warning,
    }

    public readonly struct SystemMessage
    {
        public readonly SystemMessageKind Kind;
        public readonly string Title;
        public readonly string Body;
        public readonly float Seconds;

        public SystemMessage(SystemMessageKind kind, string title, string body = "", float seconds = 4f)
        {
            Kind = kind;
            Title = title;
            Body = body;
            Seconds = seconds;
        }

        /// <summary>The System's voice: short, cold, in brackets.</summary>
        public static string Bracket(string text) => $"[{text}]";

        public static SystemMessage LevelUp(int newLevel) =>
            new SystemMessage(SystemMessageKind.Reward, Bracket("Level up!"), $"Level {newLevel}");

        public static SystemMessage Warning(string text) =>
            new SystemMessage(SystemMessageKind.Warning, Bracket("Warning"), text, 6f);
    }
}
