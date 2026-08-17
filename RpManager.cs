using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using MEC;
using Exiled.API.Features.Toys;
using UnityEngine;

namespace RpPluginByEfes
{
    public class RpMessage
    {
        public Player Sender { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string ColorHex { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

    public class RpManager
    {
        private CoroutineHandle _hudCoroutine;
        public List<RpMessage> ActiveMessages = new List<RpMessage>();
        public Dictionary<string, DateTime> CommandCooldowns = new Dictionary<string, DateTime>();

        public void OnRoundStarted()
        {
            if (_hudCoroutine.IsRunning)
                Timing.KillCoroutines(_hudCoroutine);

            _hudCoroutine = Timing.RunCoroutine(HudRoutine());
        }

        public void OnRestartingRound()
        {
            ActiveMessages.Clear();
            CommandCooldowns.Clear();
            if (_hudCoroutine.IsRunning)
                Timing.KillCoroutines(_hudCoroutine);
        }

        public void Destroy()
        {
            OnRestartingRound();
        }

        public void AddMessage(Player sender, string text, string type, string colorHex)
        {
            if (string.IsNullOrEmpty(text)) return;
            
            // Satır atlamayı sil ki üst üste binmesin
            text = text.Replace("\n", " ").Replace("\r", " ");
            
            // Güvenlik: Oyuncuların Rich Text (Zengin Metin) veya uzun metin exploitleri yapmasını engelle.
            text = text.Replace("<", "＜").Replace(">", "＞");
            // Sadece harf, rakam, boşluk ve temel noktalama işaretlerine izin ver. Emoji, zılgıt ve saçma unicode sembolleri engelle.
            text = Regex.Replace(text, @"[^\w\s\p{P}＜＞]", "");
            
            int limit = Plugin.Instance.Config.MaxMessageLength;
            if (text.Length > limit) text = text.Substring(0, limit) + "...";

            var msg = new RpMessage
            {
                Sender = sender,
                Text = text,
                Type = type,
                ColorHex = colorHex,
                ExpiryTime = DateTime.Now.AddSeconds(Plugin.Instance.Config.MessageDuration)
            };
            ActiveMessages.Add(msg);

            Log.Info($"[RpPluginByEfes] [{type}] {sender.Nickname} ({sender.Id}): {text}");

            foreach (var p in Player.List)
            {
                if (p == sender || (sender.Position - p.Position).sqrMagnitude <= Plugin.Instance.Config.MessageRange * Plugin.Instance.Config.MessageRange)
                {
                    try { p.Broadcast(1, "<color=#00000000>.</color>", global::Broadcast.BroadcastFlags.Normal, true); } catch { }
                    p.SendConsoleMessage($"[{type}] {sender.Nickname}: {text}", "green");
                }
            }
        }

        private IEnumerator<float> HudRoutine()
        {
            float tpsTimer = 0f;
            float cachedTps = 20f;
            string cachedTpsColor = "#00ff00";

            while (true)
            {
                yield return Timing.WaitForSeconds(0.5f);
                
                tpsTimer -= 0.5f;

                try
                {
                    for (int i = ActiveMessages.Count - 1; i >= 0; i--)
                    {
                        var msg = ActiveMessages[i];
                        if (DateTime.Now >= msg.ExpiryTime || msg.Sender == null || !msg.Sender.IsConnected)
                        {
                            ActiveMessages.RemoveAt(i);
                        }
                    }

                    if (tpsTimer <= 0f)
                    {
                        try
                        {
                            cachedTps = (float)Math.Round(Server.Tps, 1);
                            cachedTpsColor = cachedTps >= 15 ? "#00ff00" : (cachedTps >= 10 ? "#ffff00" : "#ff0000");
                        }
                        catch { }
                        
                        tpsTimer = Plugin.Instance.Config.TpsUpdateInterval;
                    }

                    int playerCount = Player.List.Count(x => x.IsConnected);
                    int maxPlayers = Server.MaxPlayerCount;

                    foreach (Player p in Player.List)
                    {
                        if (p == null || !p.IsConnected) continue;
                        
                        string dispName = string.IsNullOrEmpty(p.CustomName) ? p.Nickname : p.CustomName;

                        var pd = HintServiceMeow.Core.Utilities.PlayerDisplay.Get(p.ReferenceHub);
                        if (pd == null) continue;

                        // --- CINFO HINT ---
                        if (Plugin.Instance.Config.ShowCustomInfo && !string.IsNullOrEmpty(p.CustomInfo))
                        {
                            string roleColorHex = "#" + UnityEngine.ColorUtility.ToHtmlStringRGB(p.Role.Color);
                            string cinfoText = $"<align=center><size={Plugin.Instance.Config.CinfoTextSize}%><color={roleColorHex}>{p.CustomInfo.Replace("\n", " ")}</color></size></align>";
                            try {
                                if (pd.TryGetHint("RpHudCinfo", out var absCinfo) && absCinfo is HintServiceMeow.Core.Models.Hints.Hint cinfoHint)
                                {
                                    cinfoHint.Text = cinfoText;
                                    cinfoHint.YCoordinate = Plugin.Instance.Config.HudY - Plugin.Instance.Config.CinfoSpacingY;
                                    cinfoHint.XCoordinate = Plugin.Instance.Config.HudPositionX;
                                    cinfoHint.Alignment = HintServiceMeow.Core.Enum.HintAlignment.Center;
                                }
                                else
                                {
                                    pd.AddHint(new HintServiceMeow.Core.Models.Hints.Hint()
                                    {
                                        Id = "RpHudCinfo",
                                        YCoordinate = Plugin.Instance.Config.HudY - Plugin.Instance.Config.CinfoSpacingY,
                                        XCoordinate = Plugin.Instance.Config.HudPositionX,
                                        Alignment = HintServiceMeow.Core.Enum.HintAlignment.Center,
                                        Text = cinfoText
                                    });
                                }
                            } catch { }
                        }
                        else
                        {
                            try { pd.RemoveHint("RpHudCinfo"); } catch { }
                        }
                        
                        // --- BASE HUD HINT ---
                        string baseHud = Plugin.Instance.Config.HudFormat
                            .Replace("{tpsColor}", cachedTpsColor)
                            .Replace("{tps}", cachedTps.ToString("0"))
                            .Replace("{playerCount}", playerCount.ToString())
                            .Replace("{maxPlayers}", maxPlayers.ToString())
                            .Replace("{playerName}", dispName)
                            .Replace("{playerId}", p.Id.ToString());

                        try {
                            if (pd.TryGetHint("RpPluginHud", out var absHud) && absHud is HintServiceMeow.Core.Models.Hints.Hint hudHint)
                            {
                                hudHint.Text = $"<size={Plugin.Instance.Config.HudFontSize}%>{baseHud}</size>";
                                hudHint.YCoordinate = Plugin.Instance.Config.HudY;
                                hudHint.XCoordinate = Plugin.Instance.Config.HudPositionX;
                                hudHint.Alignment = HintServiceMeow.Core.Enum.HintAlignment.Center;
                            }
                            else
                            {
                                pd.AddHint(new HintServiceMeow.Core.Models.Hints.Hint()
                                {
                                    Id = "RpPluginHud",
                                    YCoordinate = Plugin.Instance.Config.HudY,
                                    XCoordinate = Plugin.Instance.Config.HudPositionX,
                                    Alignment = HintServiceMeow.Core.Enum.HintAlignment.Center,
                                    Text = $"<size={Plugin.Instance.Config.HudFontSize}%>{baseHud}</size>"
                                });
                            }
                        } catch { }

                        // --- MESSAGES HINT ---
                        var visibleMsgs = ActiveMessages
                            .Where(m => m.Sender != null && m.Sender.IsConnected && Vector3.Distance(p.Position, m.Sender.Position) <= Plugin.Instance.Config.MessageRange)
                            .OrderBy(m => m.ExpiryTime)
                            .ToList();

                        int maxMsgs = Plugin.Instance.Config.MaxVisibleMessages;
                        if (visibleMsgs.Count > maxMsgs)
                        {
                            visibleMsgs = visibleMsgs.Skip(visibleMsgs.Count - maxMsgs).ToList();
                        }

                        string nameColor = Plugin.Instance.Config.PlayerNameColor;
                        string prefix = Plugin.Instance.Config.ServerNameFormat.Replace("{serverName}", Plugin.Instance.Config.ServerName);
                        
                        for (int i = 0; i < maxMsgs; i++)
                        {
                            if (i < visibleMsgs.Count)
                            {
                                var m = visibleMsgs[visibleMsgs.Count - 1 - i];
                                string nick = m.Sender.Nickname;
                                string typeFixed = m.Type == "ZAR" ? Plugin.Instance.Config.DicePrefix : m.Type;
                                
                                string msgLine = Plugin.Instance.Config.MessageFormat
                                    .Replace("{prefix}", prefix)
                                    .Replace("{nameColor}", nameColor)
                                    .Replace("{playerName}", nick)
                                    .Replace("{msgColor}", m.ColorHex)
                                    .Replace("{type}", typeFixed)
                                    .Replace("{text}", m.Text);
                                    
                                try {
                                    if (pd.TryGetHint($"RpMsg{i}", out var absMsg) && absMsg is HintServiceMeow.Core.Models.Hints.Hint msgHint)
                                    {
                                        msgHint.Text = $"<size={Plugin.Instance.Config.MessageTextSize}%>{msgLine}</size>";
                                        msgHint.YCoordinate = Plugin.Instance.Config.MessagesY - (i * Plugin.Instance.Config.MessageSpacingY);
                                    }
                                    else
                                    {
                                        pd.AddHint(new HintServiceMeow.Core.Models.Hints.Hint()
                                        {
                                            Id = $"RpMsg{i}",
                                            YCoordinate = Plugin.Instance.Config.MessagesY - (i * Plugin.Instance.Config.MessageSpacingY),
                                            XCoordinate = Plugin.Instance.Config.MessagesX,
                                            Alignment = HintServiceMeow.Core.Enum.HintAlignment.Left,
                                            Text = $"<size={Plugin.Instance.Config.MessageTextSize}%>{msgLine}</size>"
                                        });
                                    }
                                } catch { }
                            }
                            else
                            {
                                try { pd.RemoveHint($"RpMsg{i}"); } catch { }
                            }
                        }
                        
                        // We also need to remove the old combined RpPluginMessages hint just in case they upgrade without restarting
                        try { pd.RemoveHint("RpPluginMessages"); } catch { }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"HudRoutine Error: {ex}");
                }
            }
        }
    }
}
