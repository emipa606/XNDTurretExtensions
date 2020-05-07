# TurretExtensions
Turret Extensions for RimWorld. A utility for modders to be able to create more interesting turret mods, and a Quality of Life mod!

# About this Fork
This fork's main goal is to optimize Turret Extensions so it no longer requires up to 20ms for each time it runs the WorkGiver (worst case scenarion).

At various places the code has been cleaned up and condensed using, hopefully, faster methods. The biggest saving is done by changing the way the WorkGiver runs.

We now add all turrets that are eligable to be upgraded (as in make use of the CompUpgradable component) to a static list, which we then pass into the WorkGivers. This keeps the amount of cycles to a minimum as we only go through the things we need to. No more scanning everything that is considered a building.

Thanks to Dubwise for this idea, and Syrchalis for a good example implementation.

# Testing Required
This fork still needs some testing before I am confident enough to make a PR to XeoNova to include it in an official update.
If you want to help testing, please download this repo (Clone or Download -> Download as Zip, unpack that file into your local mods folder) and report issues you might come across.

# LICENSE
This fork is licensed under MPL2.0, the original project (not this one) is licensed under MIT. See the image for proof:
![NetrveQuestionLicense](https://i.imgur.com/GuT0PKy.png)
![XeoNovaDanQuote](https://i.imgur.com/ZUbieUI.png)
