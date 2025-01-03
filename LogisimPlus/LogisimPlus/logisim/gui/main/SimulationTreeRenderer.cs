// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2011, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.main
{
    using LogisimPlus.Java;
    using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentFactory = logisim.comp.ComponentFactory;

	public class SimulationTreeRenderer : DefaultTreeCellRenderer
	{
		private class RendererIcon : Icon
		{
			internal ComponentFactory factory;
			internal bool isCurrentView;

			internal RendererIcon(ComponentFactory factory, bool isCurrentView)
			{
				this.factory = factory;
				this.isCurrentView = isCurrentView;
			}

			public virtual int IconHeight
			{
				get
				{
					return 20;
				}
			}

			public virtual int IconWidth
			{
				get
				{
					return 20;
				}
			}

			public virtual void paintIcon(JComponent c, JGraphics g, int x, int y)
			{
				ComponentDrawContext context = new ComponentDrawContext(c, null, null, g, g);
				factory.paintIcon(context, x, y, factory.createAttributeSet());

				// draw magnifying glass if appropriate
				if (isCurrentView)
				{
					int tx = x + 13;
					int ty = y + 13;
					int[] xp = new int[] {tx - 1, x + 18, x + 20, tx + 1};
					int[] yp = new int[] {ty + 1, y + 20, y + 18, ty - 1};
					g.setColor(ProjectExplorer.MAGNIFYING_INTERIOR);
					g.fillOval(x + 5, y + 5, 10, 10);
					g.setColor(Color.Black);
					g.drawOval(x + 5, y + 5, 10, 10);
					g.fillPolygon(xp, yp, xp.Length);
				}
			}
		}

		public override JComponent getTreeCellRendererComponent(JTree tree, object value, bool selected, bool expanded, bool leaf, int row, bool hasFocus)
		{
			JComponent ret = base.getTreeCellRendererComponent(tree, value, selected, expanded, leaf, row, hasFocus);
			SimulationTreeModel model = (SimulationTreeModel) tree.getModel();
			if (ret is JLabel)
			{
				JLabel label = (JLabel) ret;
				if (value is SimulationTreeNode)
				{
					SimulationTreeNode node = (SimulationTreeNode) value;
					ComponentFactory factory = node.ComponentFactory;
					if (factory != null)
					{
						label.setIcon(new RendererIcon(factory, node.isCurrentView(model)));
					}
				}
			}
			return ret;
		}
	}

}
