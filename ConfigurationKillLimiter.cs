using Rocket.API;

namespace KillLimiter
{
    public class ConfigurationKillLimiter : IRocketPluginConfiguration
    {
        public int KillLimit;
        public int LimitCooldown;

        public void LoadDefaults()
        {
            KillLimit = 3;
            LimitCooldown = 180;

        }
    }
}
