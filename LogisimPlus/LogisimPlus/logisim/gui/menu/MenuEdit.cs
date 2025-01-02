// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using ProjectEvent = logisim.proj.ProjectEvent;
	using ProjectListener = logisim.proj.ProjectListener;
	using StringUtil = logisim.util.StringUtil;

	internal class MenuEdit : Menu
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			cut = new MenuItemImpl(this, LogisimMenuBar.CUT);
			copy = new MenuItemImpl(this, LogisimMenuBar.COPY);
			paste = new MenuItemImpl(this, LogisimMenuBar.PASTE);
			delete = new MenuItemImpl(this, LogisimMenuBar.DELETE);
			dup = new MenuItemImpl(this, LogisimMenuBar.DUPLICATE);
			selall = new MenuItemImpl(this, LogisimMenuBar.SELECT_ALL);
			raise = new MenuItemImpl(this, LogisimMenuBar.RAISE);
			lower = new MenuItemImpl(this, LogisimMenuBar.LOWER);
			raiseTop = new MenuItemImpl(this, LogisimMenuBar.RAISE_TOP);
			lowerBottom = new MenuItemImpl(this, LogisimMenuBar.LOWER_BOTTOM);
			addCtrl = new MenuItemImpl(this, LogisimMenuBar.ADD_CONTROL);
			remCtrl = new MenuItemImpl(this, LogisimMenuBar.REMOVE_CONTROL);
			myListener = new MyListener(this);
		}

		private class MyListener : ProjectListener, ActionListener
		{
			private readonly MenuEdit outerInstance;

			public MyListener(MenuEdit outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void projectChanged(ProjectEvent e)
			{
				Project proj = outerInstance.menubar.Project;
				Action last = proj == null ? null : proj.LastAction;
				if (last == null)
				{
					outerInstance.undo.setText(Strings.get("editCantUndoItem"));
					outerInstance.undo.setEnabled(false);
				}
				else
				{
					outerInstance.undo.setText(StringUtil.format(Strings.get("editUndoItem"), last.Name));
					outerInstance.undo.setEnabled(true);
				}
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				Project proj = outerInstance.menubar.Project;
				if (src == outerInstance.undo)
				{
					if (proj != null)
					{
						proj.undoAction();
					}
				}
			}
		}

		private LogisimMenuBar menubar;
		private JMenuItem undo = new JMenuItem();
		private MenuItemImpl cut;
		private MenuItemImpl copy;
		private MenuItemImpl paste;
		private MenuItemImpl delete;
		private MenuItemImpl dup;
		private MenuItemImpl selall;
		private MenuItemImpl raise;
		private MenuItemImpl lower;
		private MenuItemImpl raiseTop;
		private MenuItemImpl lowerBottom;
		private MenuItemImpl addCtrl;
		private MenuItemImpl remCtrl;
		private MyListener myListener;

		public MenuEdit(LogisimMenuBar menubar)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.menubar = menubar;

			int menuMask = getToolkit().getMenuShortcutKeyMaskEx();
			undo.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_Z, menuMask));
			cut.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_X, menuMask));
			copy.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_C, menuMask));
			paste.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_V, menuMask));
			delete.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_DELETE, 0));
			dup.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_D, menuMask));
			selall.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_A, menuMask));
			raise.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_UP, menuMask));
			lower.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_DOWN, menuMask));
			raiseTop.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_UP, menuMask | KeyEvent.SHIFT_DOWN_MASK));
			lowerBottom.setAccelerator(KeyStroke.getKeyStroke(KeyEvent.VK_DOWN, menuMask | KeyEvent.SHIFT_DOWN_MASK));

			add(undo);
			addSeparator();
			add(cut);
			add(copy);
			add(paste);
			addSeparator();
			add(delete);
			add(dup);
			add(selall);
			addSeparator();
			add(raise);
			add(lower);
			add(raiseTop);
			add(lowerBottom);
			addSeparator();
			add(addCtrl);
			add(remCtrl);

			Project proj = menubar.Project;
			if (proj != null)
			{
				proj.addProjectListener(myListener);
				undo.addActionListener(myListener);
			}

			undo.setEnabled(false);
			menubar.registerItem(LogisimMenuBar.CUT, cut);
			menubar.registerItem(LogisimMenuBar.COPY, copy);
			menubar.registerItem(LogisimMenuBar.PASTE, paste);
			menubar.registerItem(LogisimMenuBar.DELETE, delete);
			menubar.registerItem(LogisimMenuBar.DUPLICATE, dup);
			menubar.registerItem(LogisimMenuBar.SELECT_ALL, selall);
			menubar.registerItem(LogisimMenuBar.RAISE, raise);
			menubar.registerItem(LogisimMenuBar.LOWER, lower);
			menubar.registerItem(LogisimMenuBar.RAISE_TOP, raiseTop);
			menubar.registerItem(LogisimMenuBar.LOWER_BOTTOM, lowerBottom);
			menubar.registerItem(LogisimMenuBar.ADD_CONTROL, addCtrl);
			menubar.registerItem(LogisimMenuBar.REMOVE_CONTROL, remCtrl);
			computeEnabled();
		}

		public virtual void localeChanged()
		{
			this.setText(Strings.get("editMenu"));
			myListener.projectChanged(null);
			cut.setText(Strings.get("editCutItem"));
			copy.setText(Strings.get("editCopyItem"));
			paste.setText(Strings.get("editPasteItem"));
			delete.setText(Strings.get("editClearItem"));
			dup.setText(Strings.get("editDuplicateItem"));
			selall.setText(Strings.get("editSelectAllItem"));
			raise.setText(Strings.get("editRaiseItem"));
			lower.setText(Strings.get("editLowerItem"));
			raiseTop.setText(Strings.get("editRaiseTopItem"));
			lowerBottom.setText(Strings.get("editLowerBottomItem"));
			addCtrl.setText(Strings.get("editAddControlItem"));
			remCtrl.setText(Strings.get("editRemoveControlItem"));
		}

		internal override void computeEnabled()
		{
			setEnabled(menubar.Project != null || cut.hasListeners() || copy.hasListeners() || paste.hasListeners() || delete.hasListeners() || dup.hasListeners() || selall.hasListeners() || raise.hasListeners() || lower.hasListeners() || raiseTop.hasListeners() || lowerBottom.hasListeners() || addCtrl.hasListeners() || remCtrl.hasListeners());
		}
	}

}
