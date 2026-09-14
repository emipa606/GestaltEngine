# GitHub Copilot Instructions for Gestalt Engine (Continued)

## Mod Overview and Purpose
Gestalt Engine (Continued) is an updated version of Helixien's mod, designed to provide a standalone experience of the mechanics initially found in "Reinforced Mechanoids 2". This mod empowers players to control mechanoids without requiring a mechanitor and introduces a new mechanoid, the Matriarch, capable of gestating new mechanoids. The mod aims to integrate seamlessly into RimWorld, maintaining the game's art style and mechanics while offering extensive mod options for balance customization.

## Key Features and Systems
- **Standalone Mechanoid Control**: Operate mechanoids independently of a mechanitor, enhancing gameplay flexibility.
- **New Mechanoid - Matriarch**: Adds a unique mechanoid capable of gestating new mechanoids.
- **Customization**: Extensive options to adjust mod features based on the player's preferences.
- **Compatibility**: Supports RimWorld version 1.4, with a likelihood of compatibility with Combat Extended.

## Coding Patterns and Conventions
When contributing to the project, adhere to the following coding conventions to ensure consistency and maintainability:
- **Naming Conventions**: Use `PascalCase` for class names and methods, and `camelCase` for local variables and method parameters.
- **File Organization**: Structure files logically in folders, e.g., `Source` for C#, `Defs` for XML definitions.
- **Comments and Documentation**: Use XML documentation comments for public methods and classes. For internal logic explanations, use single-line or block comments as appropriate.

## XML Integration
- **Defining Mod Options**: Use `ModOptionCategoryDef` in XML files (`ModOptionCategories.xml`) to define configurable settings that allow users to customize their experience.
- **Recipes and Things**: Utilize `RecipeDef` and `ThingDef` to define crafting recipes and mechanoid specifics in XML (`Recipes_MechGestator_Medium.xml` for recipes).
- **Research Projects**: Implement `ResearchProjectDef` in XML (`ResearchProjects_4_MultiAnalyzer.xml`) for new technologies related to Gestalt Engine.
- **Building and Race Definitions**: Specify building and mechanoid characteristics using `statDef` and `ThingDef` (`Buildings_Gestalt.xml` and `Races_Mechanoid.xml`).

## Harmony Patching
Harmony patches modify existing game classes to introduce new behaviors:
- Use the `Prefix` attribute to execute code before the original method, and `Postfix` for code execution after the original method.
- Example files include `CameraJumper_TryJumpAndSelect.cs`, which uses a `Prefix`, and `CompPowerTrader_CompInspectStringExtra.cs`, which includes both `Prefix` and `Postfix`.

## Suggestions for Copilot
To make the best out of GitHub Copilot while developing for Gestalt Engine:
- **Boilerplate Code**: Use Copilot to generate repetitive code structures, such as `HarmonyPatch` templates or `Def` XML boilerplates.
- **Refactoring and Suggestions**: Utilize Copilot suggestions for efficient refactoring opportunities and alternative coding approaches in C# files.
- **XML Autocompletion**: Leverage Copilot for XML autocompletion to quickly draft new definitions or modify existing ones.
- **Documentation Drafts**: Use Copilot to propose drafts for method documentation or comments for clarity on complex logic. 

Include this file as a reference for contributors to streamline the development process and ensure the mod remains consistent, efficient, and expandable.

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

