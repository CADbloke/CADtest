#CADtest for [NUnit](http://www.nunit.org/)  by[@CADbloke](http://CADbloke.com)
####CADtest runs [NUnitLite](http://www.nunit.org/index.php?p=nunitlite&r=3.0) [version 3](https://www.nuget.org/packages/NUnitLite) inside [AutoCAD](http://www.autodesk.com/products/autocad/overview) and/or the [AutoCAD Core Console](http://through-the-interface.typepad.com/through_the_interface/2012/02/the-autocad-2013-core-console.html)
####This project is only possible because of all the hard work done over the years by [Charlie Poole](http://www.charliepoole.org/) and the [NUnit](http://www.nunit.org/) team, this is 99.999% [their work](https://github.com/nunit), all I did was plug it in to AutoCAD.

If you're a Revit user - check out https://github.com/DynamoDS/RevitTestFramework

[![Join the chat at https://gitter.im/CADbloke/CADtest](https://badges.gitter.im/CADbloke/CADtest.svg)](https://gitter.im/CADbloke/CADtest?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

My Thanks and Kudos also go the the ever-resourceful AutoCAD developer crowd at [The Swamp](http://www.theswamp.org/index.php) and the [Autodesk forums](http://forums.autodesk.com/t5/net/bd-p/152) and to [@phliberato](https://github.com/phliberato) for his contributions.

####Getting Started
This is meant to be a *how-to* example rather than a full implementation that you can just drop more tests into. Take a look at the code and how it works, take what you like and ignore what you don't.

You will, of course, need AutoCAD installed to use this. No, it won't work in AutoCAD LT,
Yes, it should work in NanoCAD, BricsCAD etc. with the right references, I am yet to try that, if you do, please let us know how you go.

**Open the Visual Studio 2013 Demo solution to see how this works**.
You may want to change the AutoCAD Project references - It is currently set to build AutoCAD 2016 and .NET 4.5, getting the references from Nuget. There are projects for other versions of AutoCAD in there too, they will need some tweakage to get working.

The first build should fetch the Nuget packages. If the second build fails, restart Visual Studio or close the Solution and re-open it, I don't think it loves mixed C# & VB Solutions.

#####To add the NUnitLite v3 Console runner to a Visual Studio project...
 - There are folders for C# and VB projects. This was originally written in C# and converted to VB at http://converter.telerik.com/ so the VB code may look a bit too much like C# for your liking, it also may occasionally be missing updates that the C# code has because getting old absent minded.
 - Clone this repo to your local Library folder
 - Add a new class library project to your Solution
 -  In `Nuget`, add [`NUnitLite 3`](https://www.nuget.org/packages/NUnitLite) and the [`NUnit 3`Framework](https://www.nuget.org/packages/NUnit) to your new class library - congratulations, it is now a test project. If it already was an `NUnit 2.*` Test project then replace it with NUnit 3. You may have to fix a few things. Such is life in an ever-changing world.
 - If you want to modify any CADtest code files you should copy them into your project which you can do by "Add Existing Item" and *not* linking it. Me, I generally link to them from a local library Repo by hand-editing the `CSPROJ` file but I like breaking things. How do you tihnk I discovered this?
 -  To link them from your local library, hand-edit your `.CSPROJ | .VBPROJ` file (see below) 
 -  link or copy `App.cs | app.vb`, it has the code to display the console
 - link or copy `CADtestRunner.cs | CADtestRunner.vb` and tweak it as you wish, it launches NUnitLite and runs the tests. You can set all sorts of options there. 
 - If you edit an original file linked from your library beware that this will affect everything else that links to it.
 -  If you don't link to an original then you may need to edit a lot of projects if this gets updated and you want to use the updates.
 -  You can set the Debug startup to run a .SCR file to netload the DLL and run the command. the .SCR file would look a lot like...
```
netload "C:\Path\To\Your\TestDLL.dll"
RunCADtests
```
The startup parameters in Visual Studio's Debug settings would look like for AutoCAD...
```
/b C:\Path\To\Your\TestNetload.scr
```
... and for the Core Console like...
```
/s C:\Path\To\Your\TestNetload.scr
```
 because why should they be the same that would be `/b/s`.
 
 -  Ctrl-F5 runs the startup project without debugging so if you set your CADtest project as the startup that will run your tests by netloading the DLL and running the command.
 - This is really fast in the Core Console runner. If you are using the Core Console then [Define a Build Constant](https://www.google.com.au/search?q=C%23+Define+a+Build+Constant) "`CoreConsole`" and don't reference `AcMgd.dll`
 - If you have Resharper and are using C# then you can add the Resharper Templates file to one of the local solution [layers in Resharper Options](https://www.jetbrains.com/resharper/help/Sharing_Configuration_Options.html) manager. The file is `ResharperCADtestTemplates.DotSettings` - it only has C# templates in it, sorry VB'ers.

####Linking the C# code files by editing `.CSPROJ`
In the `CSPROJ` file, if you want to copy and modify the Command class...

```xml
<Compile Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-CS\**\*.cs" Exclude = "\CADtestRunner.cs>
```
... and then add `CADtestRunner.cs` to the project as an existing item but don't link to it.

or if you want to keep it as-is...

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

####Linking the VB code files by editing `.VBPROJ`
In the `VBPROJ` file, if you want to copy and modify the Command class...

```xml
<Compile Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-VB\**\*.vb" Exclude = "\CADtestRunner.vb>
```
... and then add `CADtestRunner.vb` to the project as an existing item but don't link to it.

or  if you want to keep it as-is...

```xml
<Compile Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-VB\**\*.vb">
```

...and this is the rest of the copy-paste into the `VBPROJ`...
```xml
  <Link>CADtest\%(RecursiveDir)%(Filename)%(Extension)</Link>
</Compile>
<None Include="$(Codez)\your.Libraries\AutoCAD\CADtest\CADtest-VB\**\*.txt">
  <Link>CADtest\%(RecursiveDir)%(Filename)%(Extension)</Link>
</None>
```
####~~Pro~~AmTip~~s~~
In the Test Classes, use the Attribute `[TestFixture, Apartment(ApartmentState.STA)]`(C#) or `<TestFixture, Apartment(ApartmentState.STA)>`(VB) ...especially for AutoCAD 2015 because it throws exceptions otherwise. 

Suggestions, questions and contributions are most welcome, use the [Issues here](https://github.com/CADbloke/CADtest/issues) or feel free to submit a [pull request](https://help.github.com/articles/using-pull-requests/) or ten, it's open source so it's for all of us.

Cheers
Ewen
