Many thanks to Marco Amendola for contributing this sample!  It demonstrates a very ingenuitive approach 
to building a framework for transparently handling Asynchronous action progress and cancellation.  He
accomplishes this by plugging in a custom filter implementation which he decorates Async actions with.  Along
with this filter he provides a custom presenter and view.  The accompanying presenter/view allow a user to
see all background tasks occurring at any given time, along with their progress and a cancellation options.