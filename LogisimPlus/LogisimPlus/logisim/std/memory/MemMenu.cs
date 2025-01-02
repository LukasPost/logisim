// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{


	using CircuitState = logisim.circuit.CircuitState;
	using HexFile = logisim.gui.hex.HexFile;
	using HexFrame = logisim.gui.hex.HexFrame;
	using Instance = logisim.instance.Instance;
	using Project = logisim.proj.Project;
	using MenuExtender = logisim.tools.MenuExtender;

	internal class MemMenu : ActionListener, MenuExtender
	{
		private Mem factory;
		private Instance instance;
		private Project proj;
		private Frame frame;
		private CircuitState circState;
		private JMenuItem edit;
		private JMenuItem clear;
		private JMenuItem load;
		private JMenuItem save;

		internal MemMenu(Mem factory, Instance instance)
		{
			this.factory = factory;
			this.instance = instance;
		}

		public virtual void configureMenu(JPopupMenu menu, Project proj)
		{
			this.proj = proj;
			this.frame = proj.Frame;
			this.circState = proj.CircuitState;

			object attrs = instance.AttributeSet;
			if (attrs is RomAttributes)
			{
				((RomAttributes) attrs).Project = proj;
			}

			bool enabled = circState != null;
			edit = createItem(enabled, Strings.get("ramEditMenuItem"));
			clear = createItem(enabled, Strings.get("ramClearMenuItem"));
			load = createItem(enabled, Strings.get("ramLoadMenuItem"));
			save = createItem(enabled, Strings.get("ramSaveMenuItem"));

			menu.addSeparator();
			menu.add(edit);
			menu.add(clear);
			menu.add(load);
			menu.add(save);
		}

		private JMenuItem createItem(bool enabled, string label)
		{
			JMenuItem ret = new JMenuItem(label);
			ret.setEnabled(enabled);
			ret.addActionListener(this);
			return ret;
		}

		public virtual void actionPerformed(ActionEvent evt)
		{
			object src = evt.getSource();
			if (src == edit)
			{
				doEdit();
			}
			else if (src == clear)
			{
				doClear();
			}
			else if (src == load)
			{
				doLoad();
			}
			else if (src == save)
			{
				doSave();
			}
		}

		private void doEdit()
		{
			MemState s = factory.getState(instance, circState);
			if (s == null)
			{
				return;
			}
			HexFrame frame = factory.getHexFrame(proj, instance, circState);
			frame.Visible = true;
			frame.toFront();
		}

		private void doClear()
		{
			MemState s = factory.getState(instance, circState);
			bool isAllZero = s.Contents.Clear;
			if (isAllZero)
			{
				return;
			}

			int choice = JOptionPane.showConfirmDialog(frame, Strings.get("ramConfirmClearMsg"), Strings.get("ramConfirmClearTitle"), JOptionPane.YES_NO_OPTION);
			if (choice == JOptionPane.YES_OPTION)
			{
				s.Contents.clear();
			}
		}

		private void doLoad()
		{
			JFileChooser chooser = proj.createChooser();
			File oldSelected = factory.getCurrentImage(instance);
			if (oldSelected != null)
			{
				chooser.setSelectedFile(oldSelected);
			}
			chooser.setDialogTitle(Strings.get("ramLoadDialogTitle"));
			int choice = chooser.showOpenDialog(frame);
			if (choice == JFileChooser.APPROVE_OPTION)
			{
				File f = chooser.getSelectedFile();
				try
				{
					factory.loadImage(circState.getInstanceState(instance), f);
				}
				catch (IOException e)
				{
					JOptionPane.showMessageDialog(frame, e.Message, Strings.get("ramLoadErrorTitle"), JOptionPane.ERROR_MESSAGE);
				}
			}
		}

		private void doSave()
		{
			MemState s = factory.getState(instance, circState);

			JFileChooser chooser = proj.createChooser();
			File oldSelected = factory.getCurrentImage(instance);
			if (oldSelected != null)
			{
				chooser.setSelectedFile(oldSelected);
			}
			chooser.setDialogTitle(Strings.get("ramSaveDialogTitle"));
			int choice = chooser.showSaveDialog(frame);
			if (choice == JFileChooser.APPROVE_OPTION)
			{
				File f = chooser.getSelectedFile();
				try
				{
					HexFile.save(f, s.Contents);
					factory.setCurrentImage(instance, f);
				}
				catch (IOException e)
				{
					JOptionPane.showMessageDialog(frame, e.Message, Strings.get("ramSaveErrorTitle"), JOptionPane.ERROR_MESSAGE);
				}
			}
		}
	}

}
