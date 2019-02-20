using Rocket.Unturned.Player;

namespace KillLimiter
{
    public class PlayerUtil : UnturnedPlayerComponent
    {
        public int Kills { get; set; }
        public long CanKillTime { get; set; }
        public bool canKill { get; set; }

        public PlayerUtil()
        {
            Kills = 0;
            CanKillTime = 0;
            canKill = true;
        }

    }
}
