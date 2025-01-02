// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.hex
{


	using HexEditor = global::hex.HexEditor;
	using HexModel = global::hex.HexModel;
	using LFrame = logisim.gui.generic.LFrame;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using Project = logisim.proj.Project;
	using JFileChoosers = logisim.util.JFileChoosers;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;
	using WindowMenuItemManager = logisim.util.WindowMenuItemManager;

	public class HexFrame : LFrame
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			windowManager = new WindowMenuManager(this);
			editListener = new EditListener(this);
			myListener = new MyListener(this);
		}

		private class WindowMenuManager : WindowMenuItemManager, LocaleListener
		{
			private readonly HexFrame outerInstance;

			internal WindowMenuManager(HexFrame outerInstance) : base(Strings.get("hexFrameMenuItem"), false)
			{
				this.outerInstance = outerInstance;
				LocaleManager.addLocaleListener(this);
			}

			public override JFrame getJFrame(bool create)
			{
				return outerInstance;
			}

			public virtual void localeChanged()
			{
				Text = Strings.get("hexFrameMenuItem");
			}
		}

		private class MyListener : ActionListener, LocaleListener
		{
			private readonly HexFrame outerInstance;

			public MyListener(HexFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal File lastFile = null;

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.open)
				{
					JFileChooser chooser = JFileChoosers.createSelected(lastFile);
					chooser.setDialogTitle(Strings.get("openButton"));
					int choice = chooser.showOpenDialog(outerInstance);
					if (choice == JFileChooser.APPROVE_OPTION)
					{
						File f = chooser.getSelectedFile();
						try
						{
							HexFile.open(outerInstance.model, f);
							lastFile = f;
						}
						catch (IOException e)
						{
							JOptionPane.showMessageDialog(outerInstance, e.Message, Strings.get("hexOpenErrorTitle"), JOptionPane.ERROR_MESSAGE);
						}
					}
				}
				else if (src == outerInstance.save)
				{
					JFileChooser chooser = JFileChoosers.createSelected(lastFile);
					chooser.setDialogTitle(Strings.get("saveButton"));
					int choice = chooser.showSaveDialog(outerInstance);
					if (choice == JFileChooser.APPROVE_OPTION)
					{
						File f = chooser.getSelectedFile();
						try
						{
							HexFile.save(f, outerInstance.model);
							lastFile = f;
						}
						catch (IOException e)
						{
							JOptionPane.showMessageDialog(outerInstance, e.Message, Strings.get("hexSaveErrorTitle"), JOptionPane.ERROR_MESSAGE);
						}
					}
				}
				else if (src == outerInstance.close)
				{
					WindowEvent e = new WindowEvent(outerInstance, WindowEvent.WINDOW_CLOSING);
					outerInstance.processWindowEvent(e);
				}
			}

			public virtual void localeChanged()
			{
				setTitle(Strings.get("hexFrameTitle"));
				outerInstance.open.setText(Strings.get("openButton"));
				outerInstance.save.setText(Strings.get("saveButton"));
				outerInstance.close.setText(Strings.get("closeButton"));
			}
		}

		private class EditListener : ActionListener, ChangeListener
		{
			private readonly HexFrame outerInstance;

			public EditListener(HexFrame outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal Clip clip = null;

			internal virtual Clip Clip
			{
				get
				{
					if (clip == null)
					{
						clip = new Clip(outerInstance.editor);
					}
					return clip;
				}
			}

			internal virtual void register(LogisimMenuBar menubar)
			{
				menubar.addActionListener(LogisimMenuBar.CUT, this);
				menubar.addActionListener(LogisimMenuBar.COPY, this);
				menubar.addActionListener(LogisimMenuBar.PASTE, this);
				menubar.addActionListener(LogisimMenuBar.DELETE, this);
				menubar.addActionListener(LogisimMenuBar.SELECT_ALL, this);
				enableItems(menubar);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == LogisimMenuBar.CUT)
				{
					Clip.copy();
					outerInstance.editor.delete();
				}
				else if (src == LogisimMenuBar.COPY)
				{
					Clip.copy();
				}
				else if (src == LogisimMenuBar.PASTE)
				{
					Clip.paste();
				}
				else if (src == LogisimMenuBar.DELETE)
				{
					outerInstance.editor.delete();
				}
				else if (src == LogisimMenuBar.SELECT_ALL)
				{
					outerInstance.editor.selectAll();
				}
			}

			internal virtual void enableItems(LogisimMenuBar menubar)
			{
				bool sel = outerInstance.editor.selectionExists();
				bool clip = true; // TODO editor.clipboardExists();
				menubar.setEnabled(LogisimMenuBar.CUT, sel);
				menubar.setEnabled(LogisimMenuBar.COPY, sel);
				menubar.setEnabled(LogisimMenuBar.PASTE, clip);
				menubar.setEnabled(LogisimMenuBar.DELETE, sel);
				menubar.setEnabled(LogisimMenuBar.SELECT_ALL, true);
			}

			public virtual void stateChanged(ChangeEvent e)
			{
				enableItems((LogisimMenuBar) getJMenuBar());
			}
		}

		private WindowMenuManager windowManager;
		private EditListener editListener;
		private MyListener myListener;
		private HexModel model;
		private HexEditor editor;
		private JButton open = new JButton();
		private JButton save = new JButton();
		private JButton close = new JButton();

		public HexFrame(Project proj, HexModel model)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			setDefaultCloseOperation(HIDE_ON_CLOSE);

			LogisimMenuBar menubar = new LogisimMenuBar(this, proj);
			setJMenuBar(menubar);

			this.model = model;
			this.editor = new HexEditor(model);

			JPanel buttonPanel = new JPanel();
			buttonPanel.add(open);
			buttonPanel.add(save);
			buttonPanel.add(close);
			open.addActionListener(myListener);
			save.addActionListener(myListener);
			close.addActionListener(myListener);

			Size pref = editor.getPreferredSize();
			JScrollPane scroll = new JScrollPane(editor, JScrollPane.VERTICAL_SCROLLBAR_ALWAYS, JScrollPane.HORIZONTAL_SCROLLBAR_NEVER);
			pref.height = Math.Min(pref.height, pref.width * 3 / 2);
			scroll.setPreferredSize(pref);
			scroll.getViewport().setBackground(editor.getBackground());

			Container contents = getContentPane();
			contents.add(scroll, BorderLayout.CENTER);
			contents.add(buttonPanel, BorderLayout.SOUTH);

			LocaleManager.addLocaleListener(myListener);
			myListener.localeChanged();
			pack();

			Size size = getSize();
			Size screen = getToolkit().getScreenSize();
			if (size.width > screen.width || size.height > screen.height)
			{
				size.width = Math.Min(size.width, screen.width);
				size.height = Math.Min(size.height, screen.height);
				setSize(size);
			}

			editor.Caret.addChangeListener(editListener);
			editor.Caret.setDot(0, false);
			editListener.register(menubar);
		}

		public override bool Visible
		{
			set
			{
				if (value && !isVisible())
				{
					windowManager.frameOpened(this);
				}
				base.setVisible(value);
			}
		}
	}

}
