# Contributing to SocialValley

Thank you for your interest in contributing! This document covers everything you need to get started.

---

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) (compatible with Stardew Valley 1.6)
- [SMAPI](https://smapi.io/) 4.0.0 or higher
- Stardew Valley 1.6+
- Visual Studio 2022 or VS Code with the C# extension

---

## Getting started

1. Fork the repository
2. Clone your fork:
   ```
   git clone https://github.com/YOUR_USERNAME/SocialValley.git
   ```
3. Open `SocialValley.sln` in Visual Studio or VS Code
4. Add a reference to `StardewValley.dll` and `SMAPI.dll` from your game installation if needed
5. Build the project — the mod output will go to your `Mods` folder if configured, or to `bin/`

---

## Making changes

- Create a new branch for your feature or fix:
  ```
  git checkout -b feature/your-feature-name
  ```
- Keep changes focused — one feature or fix per PR
- Test your changes in-game before submitting
- Keep code style consistent with the existing codebase
- Comments and PR descriptions should be in **English**

---

## Submitting a Pull Request

1. Push your branch to your fork
2. Open a Pull Request against the `main` branch
3. Describe what your PR does and why
4. Include screenshots if your change affects the UI

---

## Important rule — do not change the Game Client ID

The following constant must remain unchanged in any fork or contribution:

```csharp
private const string GameClientId = "01988480-9aca-751f-8351-cc3c505058ad";
```

This ID is tied to the Player2 integration and changing it will break authentication for all users. PRs that modify this value will not be accepted.

---

## What to contribute

Some areas where contributions are especially welcome:

- New language translations (system prompts and UI strings in `LanguageManager.cs`)
- NPC personality improvements in `NPCPersonalityManager.cs`
- Bug fixes and stability improvements
- UI/UX improvements
- Support for additional AI providers

---

## Questions?

Open an [Issue](https://github.com/CarlosNahuelcoy/SocialValley/issues) or start a [Discussion](https://github.com/CarlosNahuelcoy/SocialValley/discussions) on GitHub.
