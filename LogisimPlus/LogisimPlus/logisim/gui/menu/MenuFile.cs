// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using Frame = logisim.gui.main.Frame;
	using OptionsFrame = logisim.gui.opts.OptionsFrame;
	using PreferencesFrame = logisim.gui.prefs.PreferencesFrame;
	using Project = logisim.proj.Project;
	using ProjectActions = logisim.proj.ProjectActions;
	using MacCompatibility = logisim.util.MacCompatibility;
    using LogisimPlus.Java;

    internal class MenuFile : Menu, ActionListener
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			print = new MenuItemImpl(this, LogisimMenuBar.PRINT);
			exportImage = new MenuItemImpl(this, LogisimMenuBar.EXPORT_IMAGE);
		}

		private LogisimMenuBar menubar;
		private JMenuItem newi = new JMenuItem();
		private JMenuItem open = new JMenuItem();
		private OpenRecent openRecent;
		private JMenuItem close = new JMenuItem();
		private JMenuItem save = new JMenuItem();
		private JMenuItem saveAs = new JMenuItem();
		private MenuItemImpl print;
		private MenuItemImpl exportImage;
		private JMenuItem prefs = new JMenuItem();
		private JMenuItem quit = new JMenuItem();

		public MenuFile(LogisimMenuBar menubar)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.menubar = menubar;
			openRecent = new OpenRecent(menubar);

			int menuMask = getToolkit().getMenuShortcutKeyMaskEx();

			newi.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_N, menuMask));
			open.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_O, menuMask));
			close.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_W, menuMask | InputEvent.SHIFT_DOWN_MASK));
			save.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_S, menuMask));
			saveAs.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_S, menuMask | InputEvent.SHIFT_DOWN_MASK));
			print.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_P, menuMask));
			quit.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_Q, menuMask));

			add(newi);
			add(open);
			add(openRecent);
			addSeparator();
			add(close);
			add(save);
			add(saveAs);
			addSeparator();
			add(exportImage);
			add(print);
			if (!MacCompatibility.PreferencesAutomaticallyPresent)
			{
				addSeparator();
				add(prefs);
			}
			if (!MacCompatibility.QuitAutomaticallyPresent)
			{
				addSeparator();
				add(quit);
			}

			Project proj = menubar.Project;
			newi.addActionListener(this);
			open.addActionListener(this);
			if (proj == null)
			{
				close.setEnabled(false);
				save.setEnabled(false);
				saveAs.setEnabled(false);
			}
			else
			{
				close.addActionListener(this);
				save.addActionListener(this);
				saveAs.addActionListener(this);
			}
			menubar.registerItem(LogisimMenuBar.EXPORT_IMAGE, exportImage);
			menubar.registerItem(LogisimMenuBar.PRINT, print);
			prefs.addActionListener(this);
			quit.addActionListener(this);
		}

		public virtual void localeChanged()
		{
			this.setText(Strings.get("fileMenu"));
			newi.setText(Strings.get("fileNewItem"));
			open.setText(Strings.get("fileOpenItem"));
			openRecent.localeChanged();
			close.setText(Strings.get("fileCloseItem"));
			save.setText(Strings.get("fileSaveItem"));
			saveAs.setText(Strings.get("fileSaveAsItem"));
			exportImage.setText(Strings.get("fileExportImageItem"));
			print.setText(Strings.get("filePrintItem"));
			prefs.setText(Strings.get("filePreferencesItem"));
			quit.setText(Strings.get("fileQuitItem"));
		}

		internal override void computeEnabled()
		{
			setEnabled(true);
			menubar.fireEnableChanged();
		}

		public virtual void actionPerformed(ActionEvent e)
		{
			object src = e.getSource();
			Project proj = menubar.Project;
			if (src == newi)
			{
				ProjectActions.doNew(proj);
			}
			else if (src == open)
			{
				ProjectActions.doOpen(proj == null ? null : proj.Frame.getCanvas(), proj);
			}
			else if (src == close)
			{
				Frame frame = proj.Frame;
				if (frame.confirmClose())
				{
					frame.dispose();
					OptionsFrame f = proj.getOptionsFrame(false);
					if (f != null)
					{
						f.dispose();
					}
				}
			}
			else if (src == save)
			{
				ProjectActions.doSave(proj);
			}
			else if (src == saveAs)
			{
				ProjectActions.doSaveAs(proj);
			}
			else if (src == prefs)
			{
				PreferencesFrame.showPreferences();
			}
			else if (src == quit)
			{
				ProjectActions.doQuit();
			}
		}
	}

}
