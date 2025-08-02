# GitHub Copilot Instructions for [XND] Turret Extensions (Continued)

## Mod Overview and Purpose
Turret Extensions is a multi-purpose mod focused on enhancing and expanding the functionality and customization of turrets in RimWorld. This update of XeoNovaDan's original mod—maintained by Netrve—continues to improve performance and add new features, aiming to fill in the gaps left by vanilla capabilities and streamline mod development.

## Key Features and Systems
- **Bug Fixes**: Address vanilla issues like inaccurate cooldown/warmup stats, incorrect pawn facing when manning turrets, and siegers using wrong shell types.
- **Customization Options**: Make manned turrets benefit from the Shooting Accuracy and Aiming Time of operators and view turret ammunition details such as damage type and amount.
- **Extended Framework**: Support new and advanced turret behaviors using XML, eliminating the need for C# coding.
- **Turret Enhancements**: Introduce limited firing arcs, upgradeable turrets, and the ability to force-target enemies with auto-cancellation upon enemy downing.

## Coding Patterns and Conventions
- **Class Structure**: Many classes are designed as static for utility purposes, organized by functionality—e.g., `HarmonyPatches`, `ManualPatch_CompRefuelable`, etc.
- **Method Naming**: Methods are named descriptively to indicate their functionality and the targets they impact (e.g., `SetUpPowerVars`, `TryStartShootSomething`).
- **Code Readability**: Emphasizes clear and consistent naming conventions, inline comments, and organized class structures to maintain readability and easier debugging.

## XML Integration
- Achieve complex turret behavior purely through XML definitions using well-documented attributes and settings in the framework, aiding modders without deep C# knowledge.
- Utilize the `TurretFrameworkExtension` class to extend turret capabilities directly through XML.

## Harmony Patching
- The mod extensively uses Harmony for patching the base game, enhancing or altering original game functions without modifying the RimWorld source code.
- Multiple classes like `Patch_Building_TurretGun_*` and methods focus on specific tweaks, maintaining separation of concerns and enabling easier management of patches.

## Suggestions for Copilot
- **Autocomplete Suggestions**: Offer completions based on existing class and method patterns for quicker scaffold generation.
- **Refactoring Assistance**: Identify opportunities to optimize or refactor redundant code snippets.
- **Pattern Recognition**: Detect and suggest patterns similar to existing utility methods for extension without inconsistency.

## Additional Resources
- **Documentation**: Modders can find detailed use guides and examples in the linked documentation section of the repository.
- **TE Turret Expansion**: An example mod utilizing this framework can serve as a reference point for possible implementations.
- **Community Credits**: Acknowledge contributors and libraries (such as Harmony by Brrainz) essential to the mod's development.

If you have further feedback or enhancements, contributions are welcome!

---
This README filled by Copilot is intended to provide an overview and guide to modding with [XND] Turret Extensions. For in-depth dive into changes, check the linked documentation or the mod's source code.


This detailed `.github/copilot-instructions.md` file provides a comprehensive guide on the mod's functionalities, coding practices, and collaboration tips, tailored specifically for potential contributors and users leveraging GitHub Copilot.
