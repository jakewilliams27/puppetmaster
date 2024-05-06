# Toewrfall - Puppetmaster Modloader & Toolkit

## Team Members
List of team members involved in the project.
Just me

## Time Taken
How long it took you to complete the project.
14 hours

## Overview
TowerFall is a local co-op game where 2-4 archers are pitted against each other on a series of rounds. There are different game modes and variants that alter how the game is played. My goal was to create a modloader so that in the future new arrows, variants, etc can be created. The goal for this project was to actually inject and create some kind of modding toolkit. In this case, the toolkit currently only provides the ability to register commands which is qute limiting, but it demonstrates the ability to go further and commands are a good building block for creating commands that can potentially change the strength or properties of certain arrows. 

## Objectives
- Inject modloader into TowerFall.exe
- Create modding tools ( ResterCommand being the most straightforward)
- Add runtime texture patching

## Terminology
Modloader - an injected binary that will load modifications to base game
Runtime Texture Patching - Textures are hotswapped at runtime rather than the files being modified. This reduces the chance of corrupting your game files.
Injection - making my DLL run inside another process

## Key Technologies/Tools Used
Harmony2 - For patching functions
Shed & ManagedInjector - For injecting the binary in the game
Powershell - Created a script that injected and launched the game during testing
C# - Programmed the modloader with this.
Notepad - Looking at XML files
Reflection - for finding the Assemblies and Invoking methods

## Challenges
I had to use injection for almost everything. But, that does show the usefulness of this toolkit. Since I did it once, the toolkit provides a no-reflection necessary way to modify the game, making it much more straightforward and simple to mod. Figuring out how to call things via reflection was hard. I also tried building my own launcher with ManagedInjector but it simply just didn't work even though it said success so I went back to Shed.

## Results/Findings
- Discovered that textures are loaded from Atlases and so modifying the get_Atlas function could swap the atlas at runtime. 
- Found that award names/constraints are programmed in and can be added to the modloader in the future (Adding an award for 0 kills or something for example, where kills is the constraint and I can define the name as "Loser" or something)
- There is a built in way to add cheats, but there is only one cheat programmed in. It is a button combo that unlocks all of the playable characters
- Character sprites are not effected by the atlases. They are stored in a .XNB file that is a XNA binary file format. I will need to figure out how this works to modify character sprites.


## Conclusion
It should be relatively simple for me to expand the toolkit, with the main constriant being time. For this project, there isn't much "fun" stuff I did with the loader, but I dmeonstrated the usefulness and ability to modify parts of the gaim. I plan on looking for ways to modify the character sprites soon so that new characters can be added. I suppose I can probably do something similar at runtime with them that I did with the texture patching but we will see.

## Code Snippets
(If applicable) List of code snippets used in the project.
Built in cheat listener I found.
```csharp
this.cheats = new CheatListener();
this.cheats.AddInput('l', () => MenuInput.Left);
this.cheats.AddInput('r', () => MenuInput.Right);
this.cheats.AddInput('u', () => MenuInput.Up);
this.cheats.AddInput('d', () => MenuInput.Down);
this.cheats.AddInput('L', () => MenuInput.Alt2);
this.cheats.AddInput('R', () => MenuInput.Alt);
this.cheats.AddInput('A', () => MenuInput.Confirm);
this.cheats.AddCheat("lrLRuudlRA", delegate
{
	this.SaveOnTransition = true;
	SaveData.Instance.Unlocks.UnlockAll();
	SaveData.Instance.Quest.RevealAll();
	if (GameData.DarkWorldDLC)
	{
		SaveData.Instance.DarkWorld.RevealAll();
	}
	this.Background.AscensionTransition();
	MainMenu.PlayMenuMusic(false, true);
	base.Add<ScreenFlash>(new ScreenFlash(Color.White, 1f, -1));
	Sounds.sfx_trainingSuccess.Play(160f, 1f);
	});
	if (!SaveData.Instance.Unlocks.GunnStyle)
{
	this.cheats.AddCheat("lruudA", delegate
	{
		this.SaveOnTransition = true;
		SaveData.Instance.Unlocks.GunnStyle = true;
		SaveData.Instance.Unlocks.HandleVariantsAndAchievements(UnlockData.Unlocks.GunnStyle);
		base.Add<ScreenFlash>(new ScreenFlash(Color.White, 1f, -1));
		Sounds.sfx_trainingSuccess.Play(160f, 1f);
	});
}
base.Add<CheatListener>(this.cheats);
```
Snippet 2.
```java
static int subtract(int a, int b) {
    return a - b;
}
```

## References
https://github.com/enkomio/ManagedInjector
https://github.com/enkomio/Shed
https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/reflection