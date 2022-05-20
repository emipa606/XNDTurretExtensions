# XNDTurretExtensions

![Image](https://i.imgur.com/buuPQel.png)

Update of XeoNovaDans mod
https://steamcommunity.com/sharedfiles/filedetails/?id=1496122245
based on Netrve updated version
https://steamcommunity.com/sharedfiles/filedetails/?id=2201064941

![Image](https://i.imgur.com/pufA0kM.png)

	
![Image](https://i.imgur.com/Z4GOv8H.png)

# **RimWorld Version 1.2**

The 1.2 version of Turret Extensions, **Turret Extensions - Continuum**, is now being maintained by Netrve, please find the workshop page here - 

https://steamcommunity.com/sharedfiles/filedetails/?id=2201064941

Netrve's fork also has an optimised 1.1 version of this mod which doesn't suffer from the performance issues this release suffers from, and is even compatible with 1.0

[hr]

# **Translations**

Deutsch - Energistics

# **Overview**

Turret Extensions is a multi-purpose mod that does the following:


- Fixes several vanilla bugs such as inaccurate cooldown/warmup stat readouts, pawns always facing south when manning turrets, and siegers potentially using wrong shell types
- Makes manned turrets use the Shooting Accuracy and Aiming Time stats of the pawn using them
- Allows you to view the damage amount and damage type of turret ammuniton (e.g. shells)
- Adds various, well-documented turret framework extensions that allow modders to easily do what was previously impossible without C#, purely via XML



Turret Extensions' various framework extensions allows for turret mods that fill niches that could simply never be filled before without the extra capabilities that Turret Extensions brings to the table. Here's some of what Turret Extensions allows for as of v1.3.0:


- Manned turrets that actually factor in their operator's accuracy and/or aiming time stats
- Automatic turrets that can be force-targeted by the player, just like with pawns
- Turret force-targeting to auto-cancel when the target is downed, just like with pawns
- Turrets with limited firing arcs, as opposed to always being 360 degrees
- Upgradable turrets with a large variety of parameters that can be adjusted



An example of a mod that uses a significant chunk of this mod's framework is TE Turret Expansion, which you can find in the Links section.

All of the above can now be done *purely* in XML; not a single line of C# is required! There is comprehensive documentation in the Links section for how to use this framework in your mods.

If you want to spice up your mod's thumbnail a bit, you can download a 'Powered by Turret Extensions' overlay image from Dropbox https://www.dropbox.com/s/s57fw90729uf0iz/TurretExtensionsPreviewOverlays.zip?dl=0]**here** - with and without drop shadows.
Preview:

![Image](https://i.imgur.com/aG1rVWZ.png)


If you have any suggestions or other constructive feedback, feel free to let me know!

# **Compatibility**

**Mods:**
Turret Extensions naturally won't be compatible with Combat Extended. Mods that aren't patched with Turret Extensions will mostly behave as they would in vanilla.

**Saves:**
Should be safely addable to existing savegames. There shouldn't be any problems removing this from existing savegames if you're not running any mods that depend on this.

# **Links**

GitHub releases: https://github.com/RimWorld-CCL-Reborn/TurretExtensions/releases
Documentation for modders: https://github.com/RimWorld-CCL-Reborn/TurretExtensions/wiki
Source code: https://github.com/RimWorld-CCL-Reborn/TurretExtensions

TE Turret Expansion: https://steamcommunity.com/sharedfiles/filedetails/?id=1496140597

# **Credits**

Big thanks to **Mehni, erdelf, Lanilor, ChJees, Spdskatr, Telefonmast and FuriouslyEloquent** for helping me with the C#!
**Pardeike (AKA Brrainz) -** for making the Harmony Patch Library - this probably wouldn't have been possible without his work
**Diana Winters -** for proofreading the documentation
**Marnador -** for the RimWorld-style font

![Image](https://i.imgur.com/PwoNOj4.png)



-  See if the the error persists if you just have this mod and its requirements active.
-  If not, try adding your other mods until it happens again.
-  Post your error-log using https://steamcommunity.com/workshop/filedetails/?id=818773962]HugsLib and command Ctrl+F12
-  For best support, please use the Discord-channel for error-reporting.
-  Do not report errors by making a discussion-thread, I get no notification of that.
-  If you have the solution for a problem, please post it to the GitHub repository.


