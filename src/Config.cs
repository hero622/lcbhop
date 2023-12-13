using BepInEx.Configuration;

namespace lcbhop {
    public class Config {
        private readonly ConfigFile config;

        public bool autobhop { get; set; }
        public bool speedometer { get; set; }

        // MoveVars
        public float gravity { get; set; }
        public float friction { get; set; }
        public float maxspeed { get; set; }
        public float movespeed { get; set; }
        public float accelerate { get; set; }
        public float airaccelerate { get; set; }
        public float stopspeed { get; set; }
        public float sprintspeed { get; set; }
        public float jumpheight { get; set; }
        public bool disablefalldamage { get; set; }
        public bool infinitestamina { get; set; }
        public bool sprintingenabled { get; set; }

        public Config( ConfigFile cfg ) {
            config = cfg;
        }

        public void Init( ) {
            autobhop = config.Bind( "General", "Auto Bhop", true, "When enabled, holding Space will autojump. Disabling rebinds jump to scroll, needs ItemQuickSwitch mod!" ).Value;
            speedometer = config.Bind( "General", "Speedometer", false, "Enables speedometer HUD." ).Value;
            disablefalldamage = config.Bind("General", "Disable Fall Damage", true, "Disables fall damage." ).Value;
            infinitestamina = config.Bind( "General", "Infinite Stamina", true, "Enables infinite stamina." ).Value;
            sprintingenabled = config.Bind( "General", "Enable Sprinting", false, "Enables speed increase from sprinting." ).Value;

            gravity = config.Bind( "Move Vars", "Gravity", 600.0f, "Gravity." ).Value;
            friction = config.Bind( "Move Vars", "Friction", 5.0f, "Ground friction." ).Value;
            maxspeed = config.Bind( "Move Vars", "Max Speed", 320.0f, "Max speed per tick." ).Value;
            movespeed = config.Bind( "Move Vars", "Move Speed", 250.0f, "Ground speed (like cl_forwardspeed etc.). For normal walking speed, set to 120." ).Value;
            accelerate = config.Bind( "Move Vars", "Accelerate", 6.5f, "Ground acceleration." ).Value;
            airaccelerate = config.Bind( "Move Vars", "Air Accelerate", 100.0f, "Air acceleration." ).Value;
            stopspeed = config.Bind( "Move Vars", "Stop Speed", 80.0f, "Ground deceleration." ).Value;
            sprintspeed = config.Bind("Move Vars", "Sprint Speed", 2.5f, "Sets how much faster sprinting is (multiplicative). Only affects speed if sprinting is enabled." ).Value;
            jumpheight = config.Bind("Move Vars", "Jump Height", 45.0f, "Sets the height of the player's jump." ).Value;
        }
    }
}