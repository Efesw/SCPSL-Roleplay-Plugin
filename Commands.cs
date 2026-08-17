using System;
using CommandSystem;
using Exiled.API.Features;

namespace RpPluginByEfes.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class DoCommand : ICommand
    {
        public string Command => "do";
        public string[] Aliases => new string[] { };
        public string Description => "Specifies a roleplay action.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);
            if (p == null || p.IsDead || p.Role.Type == PlayerRoles.RoleTypeId.Spectator)
            {
                response = "Bu komutu kullanmak için hayatta olmalısın.";
                return false;
            }

            if (Plugin.Instance.Config.CommandCooldownSeconds > 0)
            {
                if (Plugin.Instance.Manager.CommandCooldowns.TryGetValue(p.UserId, out DateTime lastTime))
                {
                    float remaining = (float)(lastTime.AddSeconds(Plugin.Instance.Config.CommandCooldownSeconds) - DateTime.Now).TotalSeconds;
                    if (remaining > 0)
                    {
                        response = string.Format(Plugin.Instance.Config.CommandCooldown, remaining.ToString("0.0"));
                        return false;
                    }
                }
                Plugin.Instance.Manager.CommandCooldowns[p.UserId] = DateTime.Now;
            }

            if (arguments.Count == 0)
            {
                response = "Kullanım: .do [mesaj]";
                return false;
            }

            string text = string.Join(" ", arguments);
            Plugin.Instance.Manager.AddMessage(p, text, "DO", Plugin.Instance.Config.DoColor);
            response = "DO mesajı başarıyla gönderildi.";
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class MeCommand : ICommand
    {
        public string Command => "me";
        public string[] Aliases => new string[] { };
        public string Description => "Specifies a personal roleplay state.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);
            if (p == null || p.IsDead || p.Role.Type == PlayerRoles.RoleTypeId.Spectator)
            {
                response = "Bu komutu kullanmak için hayatta olmalısın.";
                return false;
            }

            if (Plugin.Instance.Config.CommandCooldownSeconds > 0)
            {
                if (Plugin.Instance.Manager.CommandCooldowns.TryGetValue(p.UserId, out DateTime lastTime))
                {
                    float remaining = (float)(lastTime.AddSeconds(Plugin.Instance.Config.CommandCooldownSeconds) - DateTime.Now).TotalSeconds;
                    if (remaining > 0)
                    {
                        response = string.Format(Plugin.Instance.Config.CommandCooldown, remaining.ToString("0.0"));
                        return false;
                    }
                }
                Plugin.Instance.Manager.CommandCooldowns[p.UserId] = DateTime.Now;
            }

            if (arguments.Count == 0)
            {
                response = "Kullanım: .me [mesaj]";
                return false;
            }

            string text = string.Join(" ", arguments);
            Plugin.Instance.Manager.AddMessage(p, text, "ME", Plugin.Instance.Config.MeColor);
            response = "ME mesajı başarıyla gönderildi.";
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class OocCommand : ICommand
    {
        public string Command => "ooc";
        public string[] Aliases => new string[] { };
        public string Description => "Out of character chat.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);
            if (p == null || p.IsDead || p.Role.Type == PlayerRoles.RoleTypeId.Spectator)
            {
                response = "Bu komutu kullanmak için hayatta olmalısın.";
                return false;
            }

            if (Plugin.Instance.Config.CommandCooldownSeconds > 0)
            {
                if (Plugin.Instance.Manager.CommandCooldowns.TryGetValue(p.UserId, out DateTime lastTime))
                {
                    float remaining = (float)(lastTime.AddSeconds(Plugin.Instance.Config.CommandCooldownSeconds) - DateTime.Now).TotalSeconds;
                    if (remaining > 0)
                    {
                        response = string.Format(Plugin.Instance.Config.CommandCooldown, remaining.ToString("0.0"));
                        return false;
                    }
                }
                Plugin.Instance.Manager.CommandCooldowns[p.UserId] = DateTime.Now;
            }

            if (arguments.Count == 0)
            {
                response = "Kullanım: .ooc [mesaj]";
                return false;
            }

            string text = string.Join(" ", arguments);
            Plugin.Instance.Manager.AddMessage(p, text, "OOC", Plugin.Instance.Config.OocColor);
            response = "OOC mesajı başarıyla gönderildi.";
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class DiceCommand : ICommand
    {
        public string Command => "zarat";
        public string[] Aliases => new string[] { "zar", "dice", "roll" };
        public string Description => "Rolls a random dice from 1 to 6. / 1-6 arası zar atar.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);
            if (p == null || p.IsDead || p.Role.Type == PlayerRoles.RoleTypeId.Spectator)
            {
                response = "Bu komutu kullanmak için hayatta olmalısın.";
                return false;
            }

            if (Plugin.Instance.Config.CommandCooldownSeconds > 0)
            {
                if (Plugin.Instance.Manager.CommandCooldowns.TryGetValue(p.UserId, out DateTime lastTime))
                {
                    float remaining = (float)(lastTime.AddSeconds(Plugin.Instance.Config.CommandCooldownSeconds) - DateTime.Now).TotalSeconds;
                    if (remaining > 0)
                    {
                        response = string.Format(Plugin.Instance.Config.CommandCooldown, remaining.ToString("0.0"));
                        return false;
                    }
                }
                Plugin.Instance.Manager.CommandCooldowns[p.UserId] = DateTime.Now;
            }

            int dice = UnityEngine.Random.Range(1, 7);
            
            bool isTr = Plugin.Instance.Config.Language == PluginLanguage.Turkish;
            string msgTpl = isTr ? "{0} attı" : "rolled a {0}";
            
            // If it's TR, they want it to show "ZAR AT" instead of "DICE" (or whatever prefix they set)
            string prefix = (isTr && Plugin.Instance.Config.DicePrefix == "DICE") ? "ZAR AT" : Plugin.Instance.Config.DicePrefix;

            string text = string.Format(msgTpl, dice);
            Plugin.Instance.Manager.AddMessage(p, text, prefix, Plugin.Instance.Config.DiceColor);
            response = "Zar atıldı.";
            return true;
        }
    }
}
