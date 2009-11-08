Caliburn Sivlerlight LOB Sample - Contact Manager

Please make sure to execute Caliburn's build.cmd before trying this sample.  When running the sample application, please set the ContactManager.Web as your startup project.
The ContactManagerTestPage.aspx should be your startup page.  If you try to run the SL application independently of the web site, you will have some errors, due to missing javascript provided by the page.

Some features you can look for:
-Dependency injection is used throughtout.  (Including auto-registration with the Singleton and PerRequest attributes)
-Use of Actions throughout (this includes the use of Preview and Dependencies filters).
-The ContactDetailsPresenter.ValidateContact demonstrates the use of a custom IExecutableResult from an Action.
	-> When you mouse over the 'red dot' after changing a contact, a popup will appear.  That is the result of the IExecutableResult.
	-> IExecutableResult works in conjunction with a custom ValidationResult.
	-> If you have some invalid properties, you can click on notices in the popup to focus the control related to the error.
-The structure of the application forms a composite hierarchy with implementors of IPresenter.  This hierarchy exists at runtime.  Below is an example of what might occur:
	ShellPresenter (PresenterManager)
		-> ContactListPresenter (MultiPresenterManager)
			-> N of ContactDetailsPresenter (Presenter)
	During the execution of this sample, the ShellPresenter may also host the SchedulePresenter, SettingsPresenter and HomePresenters.
-Deep linking is accomplished with the DeepLinkStateManager.  See the ShellPresenter for its use.
-The Settings class builds on top of the IsolatedStorageStateManager for simple key/value pairs stored locally.
-The Model classes inherit from ModelBase and uses a special form of property declaration (similar to dependency properties).  
	By nature of this form of definition, these classes gain the following features:
		-> Transactional editing support (BeginEdit, EndEdit, CancelEdit)
		-> Property change notification
		-> Dirty tracking
		-> Validation (by adding IPropertyValidator implementations to the IPropertyDefinition metadata)
		-> N-Level Undo/Redo support (wire to the root ModelChanged event to handle it yourself or use the UndoRedoManager)
-Custom property validators (see Model.Interogators)  Notice how I have used extension methods to improve the syntax and make validators contextual.
-Advanced shutdown scenarios for presenters.
	-> Look at the implementation of ISupportCustomShutdown for ContactDetailsPresenter and SchedulePresenter
	-> Look at the overrides of ExecuteShutdownModel in ContactListPresenter and ShellPresenter

In addition to Caliburn's framework, this sample demonstrates an application specific framework built on top of Caliburn, to add more convention to the way this app is built.
	Examples of this include:
		-> Use of a custom set of attached properties (Bind...) to do some fancy wire up of data binding, validation, etc.
		-> Use of the HistoryKey/HistoryInfo to aid the DeepLinkStateManager.

Finally, there are a number of other useful SL tidbits built in as well:
	-> Visual transitions with the TransitionPresenter and implementors of ITransition.
	-> Modal dialogs.  See the ShellView/ShellPresenter.
	-> UI scaling (ala ViewBox).  See the code behind for ShellView (the only code behind used in the entire application).

I hope this helps in your overall understanding of Caliburn and serves as an example of UI architecture.  Feedback is welcome.  Enjoy!