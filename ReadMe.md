#CADtest by @CADbloke
####CADtest runs NUnitLite version 3 inside AutoCAD and/or the AutoCAD Core Console

**Open the Visual Studio 2013 Demo solution to see how this works **.
You will need to fix the AutoCAD Project references - yours will be different to mine. It is currently set to build AutoCAD 2015 and .NET 4.5

#####To add the NUnitLite v3 Console runner to a Visual Studio project...
1. There are folders for C# and VB projects. This was originally written in C# and converted to VB at http://converter.telerik.com/ so the VB code may look a bit too much like C#, it also may be missing updates that the C# code has because getting old absent minded.
1. Clone this repo to your local Library folder
1. Add a new class library project to your Solution
1. In `Nuget`, add `NUnitLite 3` and the `NUnit 3` Framework to your new class library - congratulations, it is now a test project.
1. If you want to modify any CADtest code files you should copy them into your project which you can do by "Add Existing Item" and *not* linking it. Me, I generally link to them from a local library Repo by hand-editing the `CSPROJ` file but I like breaking things. How do you tihnk I discovered this?
1. To link them from your local library, hand-edit your `.CSPROJ | .VBPROJ` file (see below) 
  1. link or copy `App.cs | app.vb`, it has the code to display the console
  1. link or copy `CADtestRunner.cs | CADtestRunner.vb` and tweak it as you wish, it launches NUnitLite and runs the tests. You can set all sorts of options there. 
1. If you edit an original file linked from your library beware that this will affect everything else that links to it.
1. If you don't link to an original then you may need to edit a lot of projects if this gets updated and you want to use the updates.
1. You can set the Debug startup to run a .SCR file to netload the DLL and run the command. the .SCR file would look a lot like...
```
netload "C:\Path\To\Your\TestDLL.dll"
RunNUnitTests
```
The startup parameters in Visual Studio's Debug settings would look like...
```
/b C:\Path\To\Your\TestNetload.scr
```
for AutoCAD and like...
```
/s C:\Path\To\Your\TestNetload.scr
```
for the Core Console because why should they be the same that would make sense.
1. Ctrl-F5 runs the startup project without debugging so if you set your CADtest project as the startup that will run your tests by netloading the DLL and running the command.
1. This is really fast in the Core Console runner. If you are using the Core Console then Define a Build Constant "`CoreConsole`" and don't reference `AcMgd.dll`

Add the Resharper Templates file to one of the local solution layers in Resharper Options manager. The file is `ResharperCADtestTemplates.DotSettings` - it only has C# templates in it, sorry VB'ers.

####Linking the C# code files
In the `CSPROJ` file, if you want to copy and modify the Command class...

```xml
<Compile Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-CS\**\*.cs" Exclude = "\CADtestRunner.cs>
```
...
and then add `CADtestRunner.cs` to the project as an existing item but don't link to it.

or 
if you want to keep it as-is...

```xml
<Compile Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-CS\**\*.cs">
```

...and this is the rest of the copy-paste into the `CSPROJ`...
```xml
  <Link>CADtest\%(RecursiveDir)%(Filename)%(Extension)</Link>
</Compile>
<None Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-CS\**\*.txt">
  <Link>CADtest\%(RecursiveDir)%(Filename)%(Extension)</Link>
</None>
```

In the Test Classes, use the Attribute `[TestFixture, Apartment(ApartmentState.STA)]`
...especially for Acad 2015 because it throws exceptions otherwise. The Resharper class Template has this.

####Linking the VB code files
In the `VBPROJ` file, if you want to copy and modify the Command class...

```xml
<Compile Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-VB\**\*.vb" Exclude = "\CADtestRunner.vb>
```
...
and then add `CADtestRunner.vb` to the project as an existing item but don't link to it.

or 
if you want to keep it as-is...

```xml
<Compile Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-VB\**\*.vb">
```

...and this is the rest of the copy-paste into the `vbPROJ`...
```xml
  <Link>CADtest\%(RecursiveDir)%(Filename)%(Extension)</Link>
</Compile>
<None Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-VB\**\*.txt">
  <Link>CADtest\%(RecursiveDir)%(Filename)%(Extension)</Link>
</None>
```

In the Test Classes, use the Attribute `<TestFixture, Apartment(ApartmentState.STA)>`
...especially for Acad 2015 because it throws exceptions otherwise. 