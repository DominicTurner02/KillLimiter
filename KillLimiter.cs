using System;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;


namespace KillLimiter
{
    public class KillLimiter : RocketPlugin<ConfigurationKillLimiter>
    {
        public static KillLimiter Instance { get; private set; }

        protected override void Load()
        {
            base.Load();
            Instance = this;
            Logger.LogWarning("\n Loading KillLimiter, made by Mr.Kwabs...");
            Logger.LogWarning($"\n Kill Limit: {Instance.Configuration.Instance.KillLimit}");
            Logger.LogWarning($"\n Cooldown (Seconds): {Instance.Configuration.Instance.LimitCooldown}");
            Logger.LogWarning("\n Successfully loaded KillLimiter, made by Mr.Kwabs!");


            UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
            DamageTool.playerDamaged += OnDamage;
        }

        protected override void Unload()
        {

            UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
            DamageTool.playerDamaged -= OnDamage;
            Instance = null;
            base.Unload();
        }

        private void OnPlayerDeath(UnturnedPlayer uVictim, EDeathCause Cause, ELimb Limb, CSteamID Murderer)
        {
            if (UnturnedPlayer.FromCSteamID(Murderer).HasPermission("killlimiter.ignore")) { return; }

            UnturnedPlayer uKiller = UnturnedPlayer.FromCSteamID(Murderer);

            uKiller.GetComponent<PlayerUtil>().Kills++;

            if (uKiller.GetComponent<PlayerUtil>().Kills == Instance.Configuration.Instance.KillLimit)
            {
                uKiller.GetComponent<PlayerUtil>().CanKillTime = GetLongTime();
                uKiller.GetComponent<PlayerUtil>().canKill = false;
                UnturnedChat.Say(uKiller, $"You can no longer damage other players for {Instance.Configuration.Instance.LimitCooldown} seconds!", Color.red);
            }
            return;
        }

        int Frames = 0;
        private void FixedUpdate()
        {
            Frames++;
            if (Frames > 59)
            {
                if (Provider.clients.Count > 0)
                {
                    foreach (SteamPlayer sPlayer in Provider.clients)
                    {
                        PlayerUtil PlayerU = UnturnedPlayer.FromSteamPlayer(sPlayer).GetComponent<PlayerUtil>();

                        if (!PlayerU.canKill)
                        {
                            if (PlayerU.CanKillTime >= GetLongTime())
                            {
                                PlayerU.canKill = true;
                                UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(sPlayer), "You can now do damage again!", Color.yellow);
                            }
                        }
                    }
                    Frames = 0;
                    return;
                }
            }
        }

        private void OnDamage(Player Player, ref EDeathCause Cause, ref ELimb Limb, ref CSteamID Murderer, ref Vector3 Direction, ref float Damage, ref float Times, ref bool canDamage)
        {
            UnturnedPlayer uVictim = UnturnedPlayer.FromPlayer(Player);
            UnturnedPlayer uKiller = UnturnedPlayer.FromCSteamID(Murderer);
            if (uVictim.HasPermission("killlimiter.ignore") || Player == null || uVictim == null || Murderer == Provider.server || uVictim.CSteamID == Murderer || uKiller == null) { return; }

            if (!uKiller.GetComponent<PlayerUtil>().canKill)
            {
                UnturnedChat.Say(uKiller, "You have reached your kill limit!", Color.red);
                Damage = 0;
                canDamage = false;
                return;
            }
            return;
        }

        private long GetLongTime() { return (DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond; }        
    }
}
