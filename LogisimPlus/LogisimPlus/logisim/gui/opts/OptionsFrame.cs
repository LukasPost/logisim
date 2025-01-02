// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{


	using LibraryEvent = logisim.file.LibraryEvent;
	using LibraryListener = logisim.file.LibraryListener;
	using LogisimFile = logisim.file.LogisimFile;
	using LogisimFileActions = logisim.file.LogisimFileActions;
	using Options = logisim.file.Options;
	using LFrame = logisim.gui.generic.LFrame;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using Project = logisim.proj.Project;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using StringUtil = logisim.util.StringUtil;
	using WindowMenuItemManager = logisim.util.WindowMenuItemManager;

	public class OptionsFrame : LFrame
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
			windowManager = new WindowMenuManager(this);
		}

		private class WindowMenuManager : WindowMenuItemManager, LocaleListener
		{
			private readonly OptionsFrame outerInstance;

			internal WindowMenuManager(OptionsFrame outerInstance) : base(Strings.get("optionsFrameMenuItem"), false)
			{
				this.outerInstance = outerInstance;
			}

			public override JFrame getJFrame(bool create)
			{
				return outerInstance;
			}

			public virtual void localeChanged()
			{
				string title = outerInstance.project.LogisimFile.DisplayName;
				Text = StringUtil.format(Strings.get("optionsFrameMenuItem"), title);
			}
		}

		private class MyListener : ActionListener, LibraryListener, LocaleListener
		{
			private readonly OptionsFrame outerInstance;

			public MyListener(OptionsFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.revert)
				{
					outerInstance.Project.doAction(LogisimFileActions.revertDefaults());
				}
				else if (src == outerInstance.close)
				{
					WindowEvent e = new WindowEvent(outerInstance, WindowEvent.WINDOW_CLOSING);
					outerInstance.processWindowEvent(e);
				}
			}

			public virtual void libraryChanged(LibraryEvent @event)
			{
				if (@event.Action == LibraryEvent.SET_NAME)
				{
					setTitle(computeTitle(outerInstance.file));
					outerInstance.windowManager.localeChanged();
				}
			}

			public virtual void localeChanged()
			{
				setTitle(computeTitle(outerInstance.file));
				for (int i = 0; i < outerInstance.panels.Length; i++)
				{
					outerInstance.tabbedPane.setTitleAt(i, outerInstance.panels[i].Title);
					outerInstance.tabbedPane.setToolTipTextAt(i, outerInstance.panels[i].getToolTipText());
					outerInstance.panels[i].localeChanged();
				}
				outerInstance.revert.setText(Strings.get("revertButton"));
				outerInstance.close.setText(Strings.get("closeButton"));
				outerInstance.windowManager.localeChanged();
			}
		}

		private Project project;
		private LogisimFile file;
		private MyListener myListener;
		private WindowMenuManager windowManager;

		private OptionsPanel[] panels;
		private JTabbedPane tabbedPane;
		private JButton revert = new JButton();
		private JButton close = new JButton();

		public OptionsFrame(Project project)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.project = project;
			this.file = project.LogisimFile;
			file.addLibraryListener(myListener);
			setDefaultCloseOperation(HIDE_ON_CLOSE);
			setJMenuBar(new LogisimMenuBar(this, project));

			panels = new OptionsPanel[]
			{
				new SimulateOptions(this),
				new ToolbarOptions(this),
				new MouseOptions(this)
			};
			tabbedPane = new JTabbedPane();
			for (int index = 0; index < panels.Length; index++)
			{
				OptionsPanel panel = panels[index];
				tabbedPane.addTab(panel.Title, null, panel, panel.getToolTipText());
			}

			JPanel buttonPanel = new JPanel();
			buttonPanel.add(revert);
			buttonPanel.add(close);
			revert.addActionListener(myListener);
			close.addActionListener(myListener);

			Container contents = getContentPane();
			tabbedPane.setPreferredSize(new Dimension(450, 300));
			contents.add(tabbedPane, BorderLayout.CENTER);
			contents.add(buttonPanel, BorderLayout.SOUTH);

			LocaleManager.addLocaleListener(myListener);
			myListener.localeChanged();
			pack();
		}

		public virtual Project Project
		{
			get
			{
				return project;
			}
		}

		public virtual LogisimFile LogisimFile
		{
			get
			{
				return file;
			}
		}

		public virtual Options Options
		{
			get
			{
				return file.Options;
			}
		}

		public override bool Visible
		{
			set
			{
				if (value)
				{
					windowManager.frameOpened(this);
				}
				base.setVisible(value);
			}
		}

		internal virtual OptionsPanel[] PrefPanels
		{
			get
			{
				return panels;
			}
		}

		private static string computeTitle(LogisimFile file)
		{
			string name = file == null ? "???" : file.Name;
			return StringUtil.format(Strings.get("optionsFrameTitle"), name);
		}
	}

}
