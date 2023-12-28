# lcbhop
[Thunderstore page](https://thunderstore.io/c/lethal-company/p/Zyntex/Bhop_Mod) | [Github page](https://github.com/Zyntex1/lcbhop)

Bhop mod featuring *real* **Bhop** movement in Lethal Company.

Chat commands:
- /autobhop - Toggles auto bhop mode.
- /speedo - Toggles Speedometer HUD.

Requires [ItemQuickSwitch](https://thunderstore.io/c/lethal-company/p/vasanex/ItemQuickSwitch/) if not using auto bhop mode!

Movement code based on [quake3-movement-unity3d](https://github.com/WiggleWizard/quake3-movement-unity3d/tree/master), modified to match [halflife](https://github.com/ValveSoftware/halflife/blob/master/pm_shared/pm_shared.c)

## Changelog

### 1.3.1
- Fix bhop speed cap limiting vertical velocity.

### 1.3.0
- **DELETE YOUR PREVIOUS CONFIG IF YOU WANT THESE TO UPDATE!**
- Adjusted default movevars (based on **CS 1.6**), this should make bhopping more fair and not completely game breaking.
- Added in bhop speed cap (this can be disabled in the config).
- Fixed speed loss on every hop even when using autobhop.

### 1.2.1
- Fixed incorrent maths in the code which resulted in friction and other stuff not being handled correctly.
- Updated default movevars, make sure to delete your previous config if you want this to update!
- Hopefully fixed instances of taking damage randomly.

### 1.2.0
- Add movement variables to the config:
  - gravity
  - friction
  - maxspeed
  - movespeed
  - accelerate
  - airaccelerate
  - stopspeed
- Jump height and gravity should be more accurate now (a little higher).
- Speedometer shows units similar to **Source**.

### 1.1.1
- Removed ItemQuickSwitch as a concrete dependency.

### 1.1.0
- Toggle option for auto bhop mode.
- Toggleable Speedometer HUD.
- Fix duck on first jump.

### 1.0.2
- Fix not being able to duck when jumping.

### 1.0.1
- Change movement to be more **Source** like.
- Disabled fall damage.

### 1.0.0
- Initial release.
