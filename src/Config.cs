using BepInEx.Configuration;

namespace lcbhop {
    public class Config {
        private readonly ConfigFile config;

        public bool autobhop { get; set; }
        public bool speedometer { get; set; }
        public float maxspeed { get; set; }
        public float sprintspeed { get; set; }
        public float jumpheight { get; set; }
        public bool disablefalldamage { get; set; }
        public bool infinitestamina { get; set; }

        public Config( ConfigFile cfg ) {
            config = cfg;
        }

        public void Init( ) {
            autobhop = config.Bind( "General", "Auto Bhop", true, "When enabled, holding Space will autojump. Disabling rebinds jump to scroll, needs ItemQuickSwitch mod!" ).Value;
            speedometer = config.Bind( "General", "Speedometer", false, "Enables speedometer HUD." ).Value;
            maxspeed = config.Bind( "General", "Walking Speed", 4.6f, "Sets the walking speed of the player." ).Value;
            jumpheight = config.Bind("General", "Sprint Speed", 3.0f, "Sets how much faster sprinting is." ).Value;
            jumpheight = config.Bind("General", "Jump Height", 8.0f, "Sets the height of the jump." ).Value;
            disablefalldamage = config.Bind("General", "Disable Fall Damage", true, "Disables fall damage." ).Value;
            infinitestamina = config.Bind("General", "Infinite Stamina", true, "Enables infinite stamina." ).Value;
        }
    }
}