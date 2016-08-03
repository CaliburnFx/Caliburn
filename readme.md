![Caliburn](https://raw.githubusercontent.com/CaliburnFx/Caliburn/master/assets/Caliburn-cropped.png "Caliburn")
[![Build status](https://ci.appveyor.com/api/projects/status/x08nw5asybe82ry9?svg=true)](https://ci.appveyor.com/project/CoreyKaylor/caliburn)

**Building**

From the root of the project you can run `build.cmd` which will compile in release mode
 and place the nuget packages in the artifacts directory, or just open the solution and compile.

Caliburn now compiles mostly from the dotnet cli tooling with the exception of some things that
are needed for the test project. This for the most part has simplified maintaining the project.

**Where are my adapters for Container X**

We are no longer maintaining adapters for every IoC project. You are welcome
to create your own adapter and publish a nuget, or alternative just include the original
adapter source from codeplex in your own project.

Of course using a DI container is still not necessary, as Caliburn has a simple built-in container it uses by default.

Unit tests for Caliburn's features can be found in Tests.Caliburn.
You can run them either in VS Test runner, or from the test project directory by `dotnet test`

Please see the samples folder for examples of how to use the most prominent features of Caliburn.  
There are identical samples for both WPF and Silverlight.  You can also find some how to's and larger examples there.  
