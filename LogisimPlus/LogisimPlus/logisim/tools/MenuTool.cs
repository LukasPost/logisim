// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{

	using Circuit = logisim.circuit.Circuit;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using Component = logisim.comp.Component;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using Location = logisim.data.Location;
	using Canvas = logisim.gui.main.Canvas;
	using Selection = logisim.gui.main.Selection;
	using SelectionActions = logisim.gui.main.SelectionActions;
	using Project = logisim.proj.Project;

	public class MenuTool : Tool
	{
		private class MenuComponent : JPopupMenu, ActionListener
		{
			private readonly MenuTool outerInstance;

			internal Project proj;
			internal Circuit circ;
			internal Component comp;
			internal JMenuItem del = new JMenuItem(Strings.get("compDeleteItem"));
			internal JMenuItem attrs = new JMenuItem(Strings.get("compShowAttrItem"));

			internal MenuComponent(MenuTool outerInstance, Project proj, Circuit circ, Component comp)
			{
				this.outerInstance = outerInstance;
				this.proj = proj;
				this.circ = circ;
				this.comp = comp;
				bool canChange = proj.LogisimFile.contains(circ);

				add(del);
				del.addActionListener(this);
				del.setEnabled(canChange);
				add(attrs);
				attrs.addActionListener(this);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				if (src == del)
				{
					Circuit circ = proj.CurrentCircuit;
					CircuitMutation xn = new CircuitMutation(circ);
					xn.remove(comp);
					proj.doAction(xn.toAction(Strings.getter("removeComponentAction", comp.Factory.DisplayGetter)));
				}
				else if (src == attrs)
				{
					proj.Frame.viewComponentAttributes(circ, comp);
				}
			}
		}

		private class MenuSelection : JPopupMenu, ActionListener
		{
			private readonly MenuTool outerInstance;

			internal Project proj;
			internal JMenuItem del = new JMenuItem(Strings.get("selDeleteItem"));
			internal JMenuItem cut = new JMenuItem(Strings.get("selCutItem"));
			internal JMenuItem copy = new JMenuItem(Strings.get("selCopyItem"));

			internal MenuSelection(MenuTool outerInstance, Project proj)
			{
				this.outerInstance = outerInstance;
				this.proj = proj;
				bool canChange = proj.LogisimFile.contains(proj.CurrentCircuit);
				add(del);
				del.addActionListener(this);
				del.setEnabled(canChange);
				add(cut);
				cut.addActionListener(this);
				cut.setEnabled(canChange);
				add(copy);
				copy.addActionListener(this);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				Selection sel = proj.Selection;
				if (src == del)
				{
					proj.doAction(SelectionActions.clear(sel));
				}
				else if (src == cut)
				{
					proj.doAction(SelectionActions.cut(sel));
				}
				else if (src == copy)
				{
					proj.doAction(SelectionActions.copy(sel));
				}
			}
		}

		public MenuTool()
		{
		}

		public override bool Equals(object other)
		{
			return other is MenuTool;
		}

		public override int GetHashCode()
		{
			return typeof(MenuTool).GetHashCode();
		}

		public override string Name
		{
			get
			{
				return "Menu Tool";
			}
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("menuTool");
			}
		}

		public override string Description
		{
			get
			{
				return Strings.get("menuToolDesc");
			}
		}

		public override void mousePressed(Canvas canvas, JGraphics g, MouseEvent e)
		{
			int x = e.getX();
			int y = e.getY();
			Location pt = new Location(x, y);

			JPopupMenu menu;
			Project proj = canvas.Project;
			Selection sel = proj.Selection;
			ICollection<Component> in_sel = sel.getComponentsContaining(pt, g);
			if (in_sel.Count > 0)
			{
				Component comp = in_sel.GetEnumerator().next();
				if (sel.Components.Count > 1)
				{
					menu = new MenuSelection(this, proj);
				}
				else
				{
					menu = new MenuComponent(this, proj, canvas.Circuit, comp);
					MenuExtender extender = (MenuExtender) comp.getFeature(typeof(MenuExtender));
					if (extender != null)
					{
						extender.configureMenu(menu, proj);
					}
				}
			}
			else
			{
				ICollection<Component> cl = canvas.Circuit.getAllContaining(pt, g);
				if (cl.Count > 0)
				{
					Component comp = cl.GetEnumerator().next();
					menu = new MenuComponent(this, proj, canvas.Circuit, comp);
					MenuExtender extender = (MenuExtender) comp.getFeature(typeof(MenuExtender));
					if (extender != null)
					{
						extender.configureMenu(menu, proj);
					}
				}
				else
				{
					menu = null;
				}
			}

			if (menu != null)
			{
				canvas.showPopupMenu(menu, x, y);
			}
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y)
		{
			JGraphics g = c.Graphics;
			g.fillRect(x + 2, y + 1, 9, 2);
			g.drawRect(x + 2, y + 3, 15, 12);
			g.setColor(Color.LightGray);
			g.drawLine(x + 4, y + 2, x + 8, y + 2);
			for (int y_offs = y + 6; y_offs < y + 15; y_offs += 3)
			{
				g.drawLine(x + 4, y_offs, x + 14, y_offs);
			}
		}
	}

}
