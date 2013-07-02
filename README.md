ModEditor
=========

1. Building

 ModEditor uses StarDrive-mod.exe executable to load game data. I've provided compiled version to /dist folder. Main project file is ModEditor/ModEditor.csproj. Other projects are not necessary.

2. External references
 
 dist/StarDrive-mod.exe
 ModEditor/WeifenLuo.WinFormsUI.Docking.dll

 It is good to check if these files exist and Visual Studio project references them well.

3. Installation & Debug
  
 I configured debugger to place executable right to game directory. I recommend doing the same for anyone.
