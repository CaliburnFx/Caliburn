# IResult and Coroutines

Previously, I mentioned that there was one more compelling feature of the Actions concept called Coroutines. If you haven’t heard that term before, here’s what [wikipedia](http://en.wikipedia.org/wiki/Coroutine)[1] has to say:

_In computer science, coroutines are program components that generalize subroutines to allow multiple entry points for suspending and resuming execution at certain locations. Coroutines are well-suited for implementing more familiar program components such as cooperative tasks, iterators,infinite lists and pipes._

Here’s one way you can thing about it: Imagine being able to execute a method, then pause it’s execution on some statement, go do something else, then come back and resume execution where you left off. This technique is extremely powerful in task-based programming, especially when those tasks need to run asynchronously. For example, let’s say we have a ViewModel that needs to call a web service asynchronously, then it needs to take the results of that, do some work on it and call another web service asynchronously. Finally, it must then display the result in a modal dialog and respond to the user’s dialog selection with another asynchronous task. Accomplishing this with the standard event-driven async model is not a pleasant experience. However, this is a simple task to accomplish by using coroutines. The problem…C# doesn’t implement coroutines natively. Fortunately, we can (sort of) build them on top of iterators.

There are two things necessary to take advantage of this feature in Caliburn: First, implement the IResult interface on some class, representing the task you wish to execute; Second, yield instances of IResult from an Action [2]. Let’s make this more concrete. Say we had a Silverlight application where we wanted to dynamically download and show screens not part of the main package. First we would probably want to show a “Loading” indicator, then asynchronously download the external package, next hide the “Loading” indicator and finally navigate to a particular screen inside the dynamic module. Here’s what the code would look like if your first screen wanted to use coroutines to navigate to a dynamically loaded second screen:

```
namespace Caliburn.Coroutines
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using PresentationFramework.RoutedMessaging;

    [Export(typeof(ScreenOneViewModel))]
    public class ScreenOneViewModel
    {
        public IEnumerable<IResult> GoForward()
        {
            yield return Loader.Show("Downloading...");
            yield return new LoadCatalog("Caliburn.Coroutines.External.xap");
            yield return Loader.Hide();
            yield return new ShowScreen("ExternalScreen");
        }
    }
}

```

First, notice that the Action “GoForward” has a return type of IEnumerable<IResult>. This is critical for using coroutines. The body of the method has four yield statements. Each of these yields is returning an instance of IResult. The first is a result to show the “Downloading” indicator, the second to download the xap asynchronously, the third to hide the “Downloading” message and the fourth to show a new screen from the downloaded xap. After each yield statement, the compiler will “pause” the execution of this method until that particular task completes. The first, third and fourth tasks are synchronous, while the second is asynchronous. But the yield syntax allows you to write all the code in a sequential fashion, preserving the original workflow as a much more readable and declarative structure. To understand a bit more how this works, have a look at the IResult interface:

```
public interface IResult
{
    void Execute(ResultExecutionContext context);
    event EventHandler<ResultCompletionEventArgs> Completed;
}

```

It’s a fairly simple interface to implement. Simply write your code in the “Execute” method and be sure to raise the “Completed” event when you are done, whether it be a synchronous or an asynchronous task. Because coroutines occur inside of an Action, we provide you with an ResultExecutionContext useful in building UI-related IResult implementations. This allows the ViewModel a way to declaratively state its intentions in controlling the view without having any reference to a View or the need for interaction-based unit testing. Here’s what the ResultExecutionContext looks like:

```
public class ResultExecutionContext
{
    public IRoutedMessageWithOutcome Message { get; }
    public IInteractionNode HandlingNode { get; }
    public IServiceLocator ServiceLocator { get; }
}

```

And here’s an explanation of what all these properties mean:

*   Message – The original message, usually an ActionMessage, that caused the invocation of this IResult.
*   HandlingNode– The node in the UI hierarchy that handled the message.
*   ServiceLocator– The IoC container.

Bearing that in mind, I wrote a naive Loader IResult that searches the VisualTree looking for the first instance of a BusyIndicator to use to display a loading message. Here’s the implementation:

```
using System;
using System.Windows;
using System.Windows.Controls;
using PresentationFramework.RoutedMessaging;

public class Loader : IResult
{
    readonly string message;
    readonly bool hide;

    public Loader(string message)
    {
        this.message = message;
    }

    public Loader(bool hide)
    {
        this.hide = hide;
    }

    public void Execute(ResultExecutionContext context)
    {
        var view = context.HandlingNode.UIElement as FrameworkElement;
        while(view != null)
        {
            var busyIndicator = view as BusyIndicator;
            if(busyIndicator != null)
            {
                if(!string.IsNullOrEmpty(message))
                    busyIndicator.BusyContent = message;
                busyIndicator.IsBusy = !hide;
                break;
            }

            view = view.Parent as FrameworkElement;
        }

        Completed(this, new ResultCompletionEventArgs());
    }

    public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

    public static IResult Show(string message = null)
    {
        return new Loader(message);
    }

    public static IResult Hide()
    {
        return new Loader(true);
    }
}

```

See how I took advantage of context.HandlingNode to get the view? This opens up a lot of possibilities while maintaining separation between the view and the view model. Just to list a few interesting things you could do with IResult implementations: show a message box, show a VM-based modal dialog, show a VM-based Popup at the user’s mouse position, play an animation, show File Save/Load dialogs, place focus on a particular UI element based on VM properties rather than controls, etc. In fact, all of these things are already implemented in the ShellFramework and ready for your use out-of-the-box. Of coarse, one of the biggest opportunities is calling web services. Let’s look at how you might do that, but by using a slightly different scenario, dynamically downloading a xap:

```
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using Core;
using PresentationFramework.RoutedMessaging;

public class LoadCatalog : IResult
{
    static readonly Dictionary<string, DeploymentCatalog> Catalogs = new Dictionary<string, DeploymentCatalog>();
    readonly string uri;

    [Import]
    public AggregateCatalog Catalog { get; set; }

    [Import]
    public IAssemblySource AssemblySource { get; set; }

    public LoadCatalog(string relativeUri)
    {
        uri = relativeUri;
    }

    public void Execute(ResultExecutionContext context)
    {
        DeploymentCatalog catalog;

        if(Catalogs.TryGetValue(uri, out catalog))
            Completed(this, new ResultCompletionEventArgs());
        else
        {
            catalog = new DeploymentCatalog(new Uri("/ClientBin/" + uri, UriKind.RelativeOrAbsolute));
            catalog.DownloadCompleted += (s, e) => {
                if(e.Error == null) {
                    Catalogs[uri] = catalog;
                    Catalog.Catalogs.Add(catalog);
                    catalog.Parts
                        .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                        .Where(assembly => !AssemblySource.Contains(assembly))
                        .Apply(x => AssemblySource.Add(x));
                }
                else Loader.Hide().Execute(context);

                Completed(this, new ResultCompletionEventArgs {
                    Error = e.Error,
                    WasCancelled = false
                });
            };

            catalog.DownloadAsync();
        }
    }

    public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
}

```

In case it wasn’t clear, this sample is using MEF. Furthermore, we are taking advantage of the DeploymentCatalog created for Silverlight 4\. You don’t really need to know a lot about MEF or DeploymentCatalog to get the takeaway. Just take note of the fact that we wire for the DownloadCompleted event and make sure to fire the IResult.Completed event in its handler. This is what enables the async pattern to work. We also make sure to check the error and pass that along in the ResultCompletionEventArgs. Speaking of that, here’s what that class looks like:

```
public class ResultCompletionEventArgs : EventArgs
{
    public Exception Error { get; set; }
    public bool WasCancelled { get; set; }
}

```

Caliburn’s enumerator checks these properties after it get’s called back from each IResult. If there is either an error or WasCancelled is set to true, we stop execution. You can use this to your advantage. Let’s say you create an IResult for the OpenFileDialog. You could check the result of that dialog, and if the user canceled it, set WasCancelled on the event args. By doing this, you can write an action that assumes that if the code following the Dialog.Show executes, the user must have selected a file. This sort of technique can simplify the logic in such situations. Obviously, you could use the same technique for the SaveFileDialog or any confirmation style message box if you so desired. My favorite part of the LoadCatalog implementation shown above, is that the original implementation was written by a CM user! Thanks janoveh for this awesome submission!

Another thing you can do is create a series of IResult implementations built around your application’s shell (again, this is one of the things the ShellFramework does, so you might want to check it out). That is what the ShowScreen result used above does. Here is its implementation:

```
using System;
using System.ComponentModel.Composition;
using Core.InversionOfControl;
using PresentationFramework.RoutedMessaging;

public class ShowScreen : IResult {
    readonly string name;
    readonly Type screenType;

    public ShowScreen(string name) {
        this.name = name;
    }

    public ShowScreen(Type screenType) {
        this.screenType = screenType;
    }

    [Import]
    public IShell Shell { get; set; }

    public void Execute(ResultExecutionContext context) {
        var screen = !string.IsNullOrEmpty(name)
            ? context.ServiceLocator.GetInstance<object>(name)
            : context.ServiceLocator.GetInstance(screenType, null);

        Shell.ActivateItem(screen);
        Completed(this, new ResultCompletionEventArgs());
    }

    public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

    public static ShowScreen Of<T>() {
        return new ShowScreen(typeof(T));
    }
}

```

This brings up another important feature of IResult. Before Caliburn executes a result, it passes it through the your IoC container's BuildUp method allowing your container the opportunity to push dependencies in through the properties. This allows you to create them normally within your view models, while still allowing them to take dependencies on application services. In this case, we depend on IShell.

I hope this gives some explanation and creative ideas for what can be accomplished with IResult.

## Referenced Samples

*   [Caliburn.Coroutines](https://github.com/CaliburnFx/Caliburn/tree/master/samples/Caliburn.Coroutines)

_Note: Please be sure to run the sample as an out-of-browser application._

## Footnotes

1.  When I went to look up the “official” definition on wikipedia I was interested to see what it had to say about implementations in various languages. Scrolling down to the section on C#…Caliburn was listed! Fun stuff.
2.  You can also return a single instance of IResult without using IEnuermable<IResult> if you just want a simple way to execute a single task.