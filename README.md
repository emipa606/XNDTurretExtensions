# TurretExtensions
Turret Extensions for RimWorld. A utility for modders to be able to create more interesting turret mods, and a Quality of Life mod!

# About this Fork
This fork's main goal is to optimize Turret Extensions so it no longer requires up to 20ms for each time it runs the WorkGiver (worst case scenarion).

At various places the code has been cleaned up and condensed using, hopefully, faster methods. The biggest saving is done by changing the way the WorkGiver runs.
We patch RimWorld's ThingListGroupHelper.Includes() function to include Turrets within the ThingRequestGroup.Undefined, and set the WorkGiver to run on those.
Combined with a check at the beginning of the WorkGiver's function to exit if something isn't a Turret, we reduce the amount of scans required massively (before this patch it ran on all buildings marked as BuildingArtificial).

This solution was made in part possible thanks to Dninemfive's help!

# Testing Required
This fork still needs some testing before I am confident enough to make a PR to XeoNova to include it in an official update.
If you want to help testing, please download this repo (Clone or Download -> Download as Zip, unpack that file into your local mods folder) and report issues you might come across.

# LICENSE
I'm still waiting for a response to confirm I can release this under the MPL2.0 license.
Until otherwise stated please consider this repo as being under a proprietary license with a usage license only.
