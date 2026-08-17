using System;
using Exiled.API.Features;

namespace RpPluginByEfes
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "RpPluginByEfes";
        public override string Author => "Efes";
        public override string Prefix => "rppluginbyefes";
        public override Version Version => new Version(1, 0, 0);

        public static Plugin Instance { get; private set; }
        public RpManager Manager { get; private set; }

        public override void OnEnabled()
        {
            Instance = this;
            Manager = new RpManager();
            Exiled.Events.Handlers.Server.RoundStarted += Manager.OnRoundStarted;
            Exiled.Events.Handlers.Server.RestartingRound += Manager.OnRestartingRound;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= Manager.OnRoundStarted;
            Exiled.Events.Handlers.Server.RestartingRound -= Manager.OnRestartingRound;
            Manager.Destroy();
            Manager = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}
