// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.start
{

	internal class MacOsAdapter
	{ // MAC extends ApplicationAdapter {

		private class MyListener : ActionListener
		{
			public virtual void actionPerformed(ActionEvent @event)
			{
				/*
				 * ApplicationEvent event2 = (ApplicationEvent) event; int type = event2.getType(); switch (type) { case
				 * ApplicationEvent.ABOUT: About.showAboutDialog(null); break; case ApplicationEvent.QUIT_APPLICATION:
				 * ProjectActions.doQuit(); break; case ApplicationEvent.OPEN_DOCUMENT: Startup.doOpen(event2.getFile());
				 * break; case ApplicationEvent.PRINT_DOCUMENT: Startup.doPrint(event2.getFile()); break; case
				 * ApplicationEvent.PREFERENCES: PreferencesFrame.showPreferences(); break; }
				 */
			}
		}

		internal static void addListeners(bool added)
		{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unused") MyListener myListener = new MyListener();
			MyListener myListener = new MyListener();
			/*
			 * if (!added) MRJAdapter.addOpenDocumentListener(myListener); if (!added)
			 * MRJAdapter.addPrintDocumentListener(myListener); MRJAdapter.addPreferencesListener(myListener);
			 * MRJAdapter.addQuitApplicationListener(myListener); MRJAdapter.addAboutListener(myListener);
			 */
		}

		/*
		 * MAC public void handleOpenFile(com.apple.eawt.ApplicationEvent event) { Startup.doOpen(new
		 * File(event.getFilename())); }
		 * 
		 * public void handlePrintFile(com.apple.eawt.ApplicationEvent event) { Startup.doPrint(new
		 * File(event.getFilename())); }
		 * 
		 * public void handlePreferences(com.apple.eawt.ApplicationEvent event) { PreferencesFrame.showPreferences(); }
		 */

		public static void register()
		{
			// MAC Application.getApplication().addApplicationListener(new MacOsAdapter());
		}
	}
}
