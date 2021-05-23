using System;

namespace Hookah_Advisor
{
    public class BotSettings
    {
        public static string Name { get; set; } = "<BOT_NAME>";
        public static string Token { get; set; } = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");
    }
}