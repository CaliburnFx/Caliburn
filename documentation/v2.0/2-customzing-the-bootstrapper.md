# Customizing The Bootstrapper

[In the last part](1-basic-configuration-actions-and-conventions.md) we discussed the most basic configuration for Caliburn and demonstrated a couple of simple features related to Actions and Conventions. In this part, I would like to explore the Bootstrapper class a little more. Let’s begin by configuring our application to use an IoC container. We’ll use MEF for this example, but Caliburn will work well with any container. First, go ahead and grab the code from Part 1\. We are going to use that as our starting point. Add three additional references: System.ComponentModel.Composition, System.ComponentModel.Composition.Initialization and Caliburn.MEF. Those are the assemblies that contain MEF’s functionality.<sup>1</sup> Now, let’s create a new Bootstrapper called MefBootstrapper. Use the following code:

```
namespace Caliburn.HelloMef {
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Linq;
    using Core.InversionOfControl;
    using MEF;
    using PresentationFramework.ApplicationModel;

    public class MefBootstrapper : Bootstrapper<IShell> {
        protected override IServiceLocator CreateContainer() {
            var container = CompositionHost.Initialize(
                new AggregateCatalog(
                    SelectAssemblies().Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                    )
                );

            return new MEFAdapter(container);
        }
    }
}

```

That’s all the code to integrate MEF. First, we override the CreateContainer method of the Bootstrapper class. This gives us an opportunity to set up our IoC container. In this case, I’m taking advantage of Silverlight’s CompositionHost to setup the CompositionContainer. You can just instantiate the container directly if you are working with .NET. Then, I’m creating an AggregateCatalog and populating it with AssemblyCatalogs; one for each Assembly return from SelectAssemblies. Finally, we return an instance of Caliburn's MEFAdapter, configured with the CompositionContainer, so that Caliburn can work with the IoC functionality at will. You'll find that Caliburn maintains adapters for all the major IoC containers, so you can follow this pattern regardless of the container you are using. If you are using a custom container or one we don't have an adapter for out of the box, simply implement the IContainer interface and plug it in the same way.

So, what is SelectAssemblies? This is the method that Caliburn uses to configure the IAssemblySource which is uses to locate views. You can add assemblies to this at any time during your application to make them available to the framework, but the SelectAssemblies method is a special place to do it in the Bootstrapper. Simply override SelectAssemblies like this:

```
protected override IEnumerable<Assembly> SelectAssemblies()
{
    return new[] {
        Assembly.GetExecutingAssembly()
    };
}

```

All you have to do is return a list of searchable assemblies. By default, the base class returns the assembly that your Application exists in. So, if all your views are in the same assembly as your application, you don’t even need to worry about this. If you have multiple referenced assemblies that contain views, this is an extension point you need to remember. Also, if you are dynamically loading modules, you’ll need to make sure they get registered with your IoC container and the IAssemblySoure (which you can resolve from the container).

That's all there is to basic configuration, but you may want to customize the default services or add additional static modules. To do this, override the Configure method. You will be provided with an instance of IConfigurationBuilder which will enable you to customize any service or add modules. Here's an example of how you might change some settings, specify your own window manager and add the ShellFramework module.

```
protected override void Configure(IConfigurationBuilder builder) {
    base.Configure(builder);

    builder.With.PresentationFramework()
        .RegisterAllItemsWithSubjects()
        .Using(x => x.WindowManager<MyCustomWindowManager>());

    builder.With.ShellFramework();
}

```

**Word to the Wise**

_While Caliburn does provide ServiceLocator functionality through the IoC class, you should avoid using this directly in your application code. ServiceLocator is considered by many to be an anti-pattern. Pulling from a container tends to obscure the intent of the dependent code and can make testing more complicated. In future articles I will demonstrate at least one scenario where you may be tempted to access the ServiceLocator from a ViewModel. I’ll also demonstrate some solutions.<sup>2</sup>_

Besides what is shown above, there are some other notable methods on the Bootstrapper. You can override OnStartup and OnExit to execute code when the application starts or shuts down respectively and OnUnhandledException to cleanup after any exception that wasn’t specifically handled by your application code. The last override, DisplayRootView, is unique. Let’s look at how it is implemented in Bootstrapper<TRootModel>

```
protected override void DisplayRootView()
{
    var viewModel = Container.GetInstance<TRootModel>();
#if SILVERLIGHT
    var locator = Container.GetInstance<IViewLocator>();
    var view = locator.LocateForModel(viewModel, null, null);

    var binder = Container.GetInstance<IViewModelBinder>();
    binder.Bind(viewModel, view, null);

    var activator = viewModel as IActivate;
    if (activator != null)
        activator.Activate();

    Mouse.Initialize((UIElement)view);
    Application.RootVisual = (UIElement)view;
#else
    Container.GetInstance<IWindowManager>()
        .ShowWindow(viewModel);
#endif
}

```

The Silverlight version of this method resolves your root VM from the container, locates the view for it and binds the two together. It then makes sure to “activate” the VM if it implements the appropriate interface. The WPF version does the same thing by using the WindowManager class, more or less. DisplayRootView is basically a convenience implementation for model-first development. If you don’t like it, perhaps because you prefer view-first MVVM, then this is the method you want to override to change that behavior.

Now that you understand all about the Bootstrapper, let’s get our sample working. We need to add the IShell interface. In our case, it’s just a marker interface. But, in a real application, you would have some significant shell-related functionality baked into this contract. Here’s the code:

```
public interface IShell
{
}

```

Now, we need to implement the interface and decorate our ShellViewModel with the appropriate MEF attributes:

```
[Export(typeof(IShell))]
public class ShellViewModel : PropertyChangedBase, IShell
{
   ...implementation is same as before...
}

```

That’s it! Your up and running with MEF and you have a handle on some of the other key extension point of the Bootstrapper as well.

## Referenced Samples

*   [Caliburn.HelloMef](https://github.com/CaliburnFx/Caliburn/tree/master/samples/Caliburn.HelloMef)

## Footnotes

1.  If you are using .NET rather than Silverlight, you will only need to reference System.ComponentModel.Composition and Caliburn.MEF.
2.  I’m quite guilty of this myself, but I’m trying to be more conscious of it. I’m also excited to see that modern IoC containers as well as Caliburn provide some very nice ways to avoid this situation.