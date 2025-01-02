// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using Bounds = logisim.data.Bounds;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using Project = logisim.proj.Project;

	public class MemPoker : InstancePoker
	{
		private MemPoker sub;

		public override bool init(InstanceState state, MouseEvent @event)
		{
			Bounds bds = state.Instance.Bounds;
			MemState data = (MemState) state.Data;
			long addr = data.getAddressAt(@event.getX() - bds.X, @event.getY() - bds.Y);

			// See if outside box
			if (addr < 0)
			{
				sub = new AddrPoker();
			}
			else
			{
				sub = new DataPoker(state, data, addr);
			}
			return true;
		}

		public override Bounds getBounds(InstancePainter state)
		{
			return sub.getBounds(state);
		}

		public override void paint(InstancePainter painter)
		{
			sub.paint(painter);
		}

		public override void keyTyped(InstanceState state, KeyEvent e)
		{
			sub.keyTyped(state, e);
		}

		private class DataPoker : MemPoker
		{
			internal int initValue;
			internal int curValue;

			internal DataPoker(InstanceState state, MemState data, long addr)
			{
				data.Cursor = addr;
				initValue = data.Contents.get(data.Cursor);
				curValue = initValue;

				object attrs = state.Instance.AttributeSet;
				if (attrs is RomAttributes)
				{
					Project proj = state.Project;
					if (proj != null)
					{
						((RomAttributes) attrs).Project = proj;
					}
				}
			}

			public override Bounds getBounds(InstancePainter painter)
			{
				MemState data = (MemState) painter.Data;
				Bounds inBounds = painter.getInstance().Bounds;
				return data.getBounds(data.Cursor, inBounds);
			}

			public override void paint(InstancePainter painter)
			{
				Bounds bds = getBounds(painter);
				Graphics g = painter.Graphics;
				g.setColor(Color.RED);
				g.drawRect(bds.X, bds.Y, bds.Width, bds.Height);
				g.setColor(Color.BLACK);
			}

			public override void stopEditing(InstanceState state)
			{
				MemState data = (MemState) state.Data;
				data.Cursor = -1;
			}

			public override void keyTyped(InstanceState state, KeyEvent e)
			{
				char c = e.getKeyChar();
				int val = Character.digit(e.getKeyChar(), 16);
				MemState data = (MemState) state.Data;
				if (val >= 0)
				{
					curValue = curValue * 16 + val;
					data.Contents.set(data.Cursor, curValue);
					state.fireInvalidated();
				}
				else if (c == ' ' || c == '\t')
				{
					moveTo(data, data.Cursor + 1);
				}
				else if (c == '\r' || c == '\n')
				{
					moveTo(data, data.Cursor + data.Columns);
				}
				else if (c == '\u0008' || c == '\u007f')
				{
					moveTo(data, data.Cursor - 1);
				}
			}

			internal virtual void moveTo(MemState data, long addr)
			{
				if (data.isValidAddr(addr))
				{
					data.Cursor = addr;
					data.scrollToShow(addr);
					initValue = data.Contents.get(addr);
					curValue = initValue;
				}
			}
		}

		private class AddrPoker : MemPoker
		{
			public override Bounds getBounds(InstancePainter painter)
			{
				MemState data = (MemState) painter.Data;
				return data.getBounds(-1, painter.Bounds);
			}

			public override void paint(InstancePainter painter)
			{
				Bounds bds = getBounds(painter);
				Graphics g = painter.Graphics;
				g.setColor(Color.RED);
				g.drawRect(bds.X, bds.Y, bds.Width, bds.Height);
				g.setColor(Color.BLACK);
			}

			public override void keyTyped(InstanceState state, KeyEvent e)
			{
				char c = e.getKeyChar();
				int val = Character.digit(e.getKeyChar(), 16);
				MemState data = (MemState) state.Data;
				if (val >= 0)
				{
					long newScroll = (data.Scroll * 16 + val) & (data.LastAddress);
					data.Scroll = newScroll;
				}
				else if (c == ' ')
				{
					data.Scroll = data.Scroll + (data.Rows - 1) * data.Columns;
				}
				else if (c == '\r' || c == '\n')
				{
					data.Scroll = data.Scroll + data.Columns;
				}
				else if (c == '\u0008' || c == '\u007f')
				{
					data.Scroll = data.Scroll - data.Columns;
				}
			}
		}
	}
}
