# GitHub Copilot Instructions for [XND] Turret Extensions (Continued)

Welcome to the GitHub Copilot instructions for the [XND] Turret Extensions (Continued) mod for RimWorld. This document provides an overview of the mod, its key features, coding patterns and conventions, XML integration, Harmony patching, and suggestions for using GitHub Copilot effectively.

## Mod Overview and Purpose

[XND] Turret Extensions (Continued) is a multi-purpose mod that updates and enhances the turret system in RimWorld. The mod addresses several vanilla bugs and introduces new features that allow modders to easily extend turret functionality through XML, without requiring C# coding. The primary goals of the mod include:

- Fixing inaccurate cooldown/warmup stat readouts and other turret-related bugs.
- Allowing manned turrets to use the Shooting Accuracy and Aiming Time stats of the pawn using them.
- Enabling the viewing of damage amount and damage type of turret ammunition.
- Providing a comprehensive turret framework that allows modders to create turrets with advanced features such as automatic targeting, limited firing arcs, and upgradable parameters.

## Key Features and Systems

### Fixes and Improvements
- **Cooldown/Warmup Stat Readouts**: Accurate display of cooldown and warmup stats.
- **Pawn Orientation**: Ensures pawns facing the correct direction when manning turrets.
- **Siege Shell Types**: Corrects potential issues with siege shell types.

### Enhanced Turret Functionality
- **Operator Stats**: Manned turrets use the operator's Shooting Accuracy and Aiming Time stats.
- **Damage Information**: Displays damage amount and type for turret ammunition.

### Turret Framework Extensions
- **Smart Forced Targeting**: Allows turrets to be force-targeted by the player.
- **Auto-Cancel Targeting**: Targets automatically cancel when the target is downed.
- **Limited Firing Arcs**: Turrets can have firing arcs limited to specific angles.
- **Upgradable Turrets**: Turrets with adjustable parameters for customization.

## Coding Patterns and Conventions

### General Coding Practices
- **Modular Design**: The mod is structured into different components, each responsible for specific functionalities.
- **Documentation**: Comprehensive documentation is provided in the Links section for modders.
- **Error Handling**: Proper error handling is implemented to ensure stability and user-friendly feedback.

### XML Integration
- **XML Definitions**: The mod extensively uses XML files to define and configure various aspects of the turret system.
- **Def Files**: Definitions for JobDefs, DesignationDefs, RecordDefs, StatCategoryDefs, and WorkGiverDefs are included in the mod.
- **XML Framework**: The mod provides a flexible XML framework that allows modders to create complex turret behaviors without C# code.

### Harmony Patching
- **Harmony Library**: The mod uses the Harmony Patch Library (brrainz.harmony) for patching vanilla RimWorld methods.
- **Patch Implementation**: Patches are used to fix bugs, enhance features, and integrate new functionalities.

## Suggestions for Copilot

### Utilizing Copilot
- **XML File Generation**: Use Copilot to generate XML definitions and configurations for new turret features.
- **Mod Integration**: Assist in integrating new turret functionalities with the existing mod framework.
- **Error Handling**: Help with implementing error handling and logging mechanisms.
- **Code Refactoring**: Suggest improvements for existing C# code to enhance readability and performance.

### Best Practices
- **Consistent Naming Conventions**: Follow the mod's naming conventions for new files and classes.
- **Documentation**: Document new code and configurations to maintain the mod's comprehensive documentation.
- **Testing**: Ensure new features are thoroughly tested before integration to maintain mod stability.

## Getting Started

To get started with modding using [XND] Turret Extensions (Continued), follow these steps:

1. **Install Dependencies**: Ensure you have the Harmony Patch Library installed.
2. **Fork the Repository**: Fork the mod repository to your GitHub account.
3. **Clone the Repository**: Clone the forked repository to your local machine.
4. **Explore the Codebase**: Review the existing C# and XML files to understand the mod's structure.
5. **Start Modding**: Use GitHub Copilot to assist with generating and integrating new features.

## Links

- **Documentation for Modders**: [TE Turret Expansion](https://github.com/Netrve/TE_Turret_Expansion)
- **Log Uploader**: [RimWorld Log Uploader](https://rimworldlog.com/)
- **Discord Support**: [RimWorld Modding Discord](https://discord.gg/rimworld)

## Credits

- **Mehni, erdelf, Lanilor, ChJees, Spdskatr, Telefonmast, FuriouslyEloquent**: Helped with C# development.
- **Pardeike (AKA Brrainz)**: Created the Harmony Patch Library.
- **Diana Winters**: Proofread the documentation.
- **Marnador**: Provided a RimWorld-style font.

---

By following these instructions, you can effectively utilize GitHub Copilot to enhance and expand the functionality of the [XND] Turret Extensions (Continued) mod. Happy modding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

