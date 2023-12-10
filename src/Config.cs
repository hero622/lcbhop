using BepInEx.Configuration;

namespace lcbhop {
    public class Config {
        private readonly ConfigFile config;

        public bool autobhop { get; set; }
        public bool speedometer { get; set; }

        public Config( ConfigFile cfg ) {
            config = cfg;
        }

        public void Init( ) {
            autobhop = config.Bind( "General", "Auto Bhop", true, "Disabling rebinds jump to scroll, needs ItemQuickSwitch mod!" ).Value;
            speedometer = config.Bind( "General", "Speedometer", false, "Enables speedometer HUD." ).Value;
        }
    }
}
