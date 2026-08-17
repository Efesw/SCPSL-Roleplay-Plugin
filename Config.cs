using Exiled.API.Interfaces;
using System.ComponentModel;

namespace RpPluginByEfes
{
    public enum PluginLanguage
    {
        English,
        Turkish
    }

    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;
        
        [Description("Is debug mode enabled?")]
        public bool Debug { get; set; } = false;

        [Description("Language mode of the plugin. Affects command names (e.g., dice vs zarat). Values: English, Turkish")]
        public PluginLanguage Language { get; set; } = PluginLanguage.English;

        [Description("The server name displayed before the player's name in RP holograms.")]
        public string ServerName { get; set; } = "MyServer";

        [Description("Format for the server name prefix in RP holograms. You can use Unity rich text here.")]
        public string ServerNameFormat { get; set; } = "<size=60%><color=#ffffff44>{serverName} / </color></size>";

        [Description("The format for the RP messages on the screen.")]
        public string MessageFormat { get; set; } = "<b>{prefix}<color={nameColor}>{playerName}</color> | <color={msgColor}>{type} : {text}</color></b>";

        [Description("The duration (in seconds) that RP messages stay on the screen.")]
        public float MessageDuration { get; set; } = 5f;
        
        [Description("The maximum radius (in meters) within which players can see RP messages.")]
        public float MessageRange { get; set; } = 15f;

        [Description("The hex color code for the player's name in DO/ME/OOC messages.")]
        public string PlayerNameColor { get; set; } = "#cccccc";

        [Description("The hex color code for DO command messages.")]
        public string DoColor { get; set; } = "#ff5555";

        [Description("The hex color code for ME command messages.")]
        public string MeColor { get; set; } = "#aa55ff";

        [Description("The hex color code for OOC command messages.")]
        public string OocColor { get; set; } = "#55ffff";

        [Description("The hex color code for DICE command messages.")]
        public string DiceColor { get; set; } = "#ffff55";

        [Description("Show the player's CustomInfo (cinfo) one line above the HUD?")]
        public bool ShowCustomInfo { get; set; } = true;

        [Description("The format of the text displayed in the HUD. Available variables: {tpsColor}, {tps}, {playerCount}, {maxPlayers}, {playerName}, {playerId}")]
        public string HudFormat { get; set; } = "<b><color=#ffaa00>RpPluginByEfes</color></b> | <color=#cccccc>TPS: </color><color={tpsColor}>{tps}</color> | <color=#cccccc>Players:</color> {playerCount}/{maxPlayers} | <color=#00ffff>{playerName} ({playerId})</color>";

        [Description("The font size percentage for the HUD text. Default: 70")]
        public int HudFontSize { get; set; } = 70;

        [Description("How often (in seconds) the TPS indicator should update. Default is 5.0s to save performance.")]
        public float TpsUpdateInterval { get; set; } = 5.0f;

        [Description("The font size percentage for Cinfo text. Default: 60")]
        public int CinfoTextSize { get; set; } = 60;

        [Description("The font size percentage for DO/ME/OOC/DICE messages. Default: 60")]
        public int MessageTextSize { get; set; } = 60;

        [Description("The maximum character limit for DO/ME/OOC messages.")]
        public int MaxMessageLength { get; set; } = 32;

        [Description("The maximum number of RP messages displayed on the screen simultaneously.")]
        public int MaxVisibleMessages { get; set; } = 6;

        [Description("The Y (vertical) coordinate for the RpPlugin HUD text. (1050 is near the very bottom)")]
        public float HudY { get; set; } = 1050f;

        [Description("The X (horizontal) coordinate for the RpPlugin HUD text. (0 = perfectly centered)")]
        public float HudPositionX { get; set; } = 0f;

        [Description("The vertical spacing (in pixels) between the HUD and the Cinfo text above it.")]
        public float CinfoSpacingY { get; set; } = 30f;

        [Description("The Y (vertical) coordinate where DO/ME/OOC messages begin to stack.")]
        public float MessagesY { get; set; } = 850f;

        [Description("The X (horizontal) coordinate where DO/ME/OOC messages begin to stack.")]
        public float MessagesX { get; set; } = 150f;

        [Description("The vertical spacing (in pixels) between each stacked RP message.")]
        public float MessageSpacingY { get; set; } = 30f;

        [Description("The cooldown (in seconds) between using RP commands.")]
        public float CommandCooldownSeconds { get; set; } = 3f;

        [Description("Cooldown message for RP commands. {0} is the remaining time in seconds.")]
        public string CommandCooldown { get; set; } = "Please wait {0} seconds before using this command again.";

        [Description("The prefix shown for the Dice command (e.g., DICE or ZAR)")]
        public string DicePrefix { get; set; } = "DICE";

        [Description("Credits / Developer Information")]
        public string MadeBy { get; set; } = "Made by Efes";
    }
}
