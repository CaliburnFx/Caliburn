# Caliburn vs. Caliburn.Micro

In general, prefer to use Caliburn.Micro over Caliburn. It is a smaller, simpler framework. It can probably do everything you need and can easily be customized on a per-application basis. Below is a list (probably not-comprehensive) of some of the things that are different between the two frameworks:

*   While both frameworks use a Bootstrapper, Caliburn has many more configuration options.
*   IoC abstraction is much more involved in Caliburn than Micro. Also, Caliburn maintains adapters for major containers.
*   Caliburn services are pluggable via interface. Micro is partially pluggable via public static Fun<...> fields for major features.
*   Caliburn supports AOP and Micro does not.
*   Caliburn has a Validation abstraction. Micro does not.
*   Caliburn has a module framework. Micro does not.
*   Caliburn has an infrastructure for background processing and ExpressionTree-Based runtime delegate generation. Micro does not.
*   Caliburn has a much more advanced Inflector than Micro.
*   Caliburn has a Testability framework.
*   Caliburn has a ViewModelFactory. Micro does not.
*   Caliburn's Parameter implementation can handle "dotting" through properties. Micro cannot.
*   Caliburn supports filters. Micro only supports a Can* guard to Action execution.
*   Micro's Trigger/Action mechanism is built on System.Windows.Interactivity. Caliburn's is custom (it predates the MS implementation).
*   Caliburn has a full Dispatcher abstraction. Micro only abstract UI thread invocation.
*   Caliburn has screen subjects. Micro does not.
*   Micro does not support custom IViewStrategies or the ViewAttribute.
*   Caliburn caches metadata and attempts to optimize a lot more.
*   The conventions implementations are drastically different between the two, though the support the same conventions.
*   Caliburn has abstractions for DeepLinking storage and IsolatedStorage while Micro does not.
*   Caliburn has an InputManager. Micro does not.
*   Caliburn has a ShellFramework. Micro does not.