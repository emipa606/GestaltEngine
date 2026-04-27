# GitHub Copilot Instructions for Gestalt Engine (Continued)

## Mod Overview and Purpose

**Gestalt Engine (Continued)** is a standalone version of the "Gestalt Engine" originally found in "Reinforced Mechanoids 2". The mod is designed for enhanced control over mechanoids without requiring a mechanitor. It adds a new mechanoid type, the Matriarch, capable of gestating new mechanoids. This mod aims to enrich the RimWorld experience by integrating seamlessly with the game's art style and mechanics, while offering customization options for balancing.

## Key Features and Systems

- **Standalone Mechanitor-Free Control**: Allows players to manage mechanoids independently of a mechanitor.
- **New Mechanoid - Matriarch**: Introduces the Matriarch mechanoid that can reproduce other mechanoids.
- **Comprehensive Mod Options**: Extensive settings for users to tweak game balance to their preference.
- **Seamless Integration**: Ensures that the mod fits into RimWorld's existing aesthetic and mechanics.
  
## Coding Patterns and Conventions

- **Organized Static Classes**: Static classes are used widely, providing clear separation of utility functions and modular components.
- **Clear Method Names**: Methods are named for their purpose and functionality, aiding readability and maintenance.
- **Robust Use of Targeting Parameters**: Private methods are utilized for complex targeting logic, enhancing code encapsulation and reusability.

## XML Integration

The mod primarily interacts with XML through RimWorld's Def system which may not be outlined fully in the source code description. Ensure XML definitions are placed correctly within the mod's `Defs` directory to provide necessary configurations for new mechanoids and properties.

## Harmony Patching

While the mod currently does not necessitate any Harmony patches, future updates or compatibility enhancements might require patching. If needed, Harmony is recommended for non-intrusive modifications of RimWorld's core functionality.

## Suggestions for GitHub Copilot

- Leverage Copilot to generate boilerplate code for new classes or methods based on existing patterns.
- Use Copilot to draft XML Defs templates, complementing existing configurations.
- Employ Copilot's natural language processing to generate complex targeting methods more intuitively.
- Seek Copilot's assistance for implementing Harmony patches as the mod evolves with changing dependencies.

Remember, while Copilot can expedite the coding process, always review and test generated code for logical accuracy and integration stability within the broader mod framework.

## Additional Resources

For more detailed documentation and community support, please refer to the [Changelog tab] and use the mod's dedicated Discord channel for error reporting and discussion.

Happy modding!


This file serves as a comprehensive guide for developers and contributors engaged in modding the "Gestalt Engine (Continued)" for RimWorld. By following these guidelines, developers can maintain consistency, scalability, and compatibility within the mod and the wider RimWorld modding ecosystem.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
