// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using ComponentEvent = logisim.comp.ComponentEvent;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using ComponentUserEvent = logisim.comp.ComponentUserEvent;
	using EndData = logisim.comp.EndData;
	using ManagedComponent = logisim.comp.ManagedComponent;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using StdAttr = logisim.instance.StdAttr;
	using Project = logisim.proj.Project;
	using MenuExtender = logisim.tools.MenuExtender;
	using ToolTipMaker = logisim.tools.ToolTipMaker;
	using WireRepair = logisim.tools.WireRepair;
	using WireRepairData = logisim.tools.WireRepairData;
	using StringUtil = logisim.util.StringUtil;

	public class Splitter : ManagedComponent, WireRepair, ToolTipMaker, MenuExtender, AttributeListener
	{
		// basic data
		internal sbyte[] bit_thread; // how each bit maps to thread within end

		// derived data
		internal CircuitWires.SplitterData wire_data;

		public Splitter(Location loc, AttributeSet attrs) : base(loc, attrs, 3)
		{
			configureComponent();
			attrs.addAttributeListener(this);
		}

		//
		// abstract ManagedComponent methods
		//
		public override ComponentFactory Factory
		{
			get
			{
				return SplitterFactory.instance;
			}
		}

		public override void propagate(CircuitState state)
		{
			; // handled by CircuitWires, nothing to do
		}

		public override bool contains(Location loc)
		{
			if (base.contains(loc))
			{
				Location myLoc = Location;
				Direction facing = AttributeSet.getValue(StdAttr.FACING);
				if (facing == Direction.East || facing == Direction.West)
				{
					return Math.Abs(loc.X - myLoc.X) > 5 || loc.manhattanDistanceTo(myLoc) <= 5;
				}
				else
				{
					return Math.Abs(loc.Y - myLoc.Y) > 5 || loc.manhattanDistanceTo(myLoc) <= 5;
				}
			}
			else
			{
				return false;
			}
		}

		private void configureComponent()
		{
			lock (this)
			{
				SplitterAttributes attrs = (SplitterAttributes) AttributeSet;
				SplitterParameters parms = attrs.Parameters;
				int fanout = attrs.fanout;
				sbyte[] bit_end = attrs.bit_end;
        
				// compute width of each end
				bit_thread = new sbyte[bit_end.Length];
				sbyte[] end_width = new sbyte[fanout + 1];
				end_width[0] = (sbyte) bit_end.Length;
				for (int i = 0; i < bit_end.Length; i++)
				{
					sbyte thr = bit_end[i];
					if (thr > 0)
					{
						bit_thread[i] = end_width[thr];
						end_width[thr]++;
					}
					else
					{
						bit_thread[i] = -1;
					}
				}
        
				// compute end positions
				Location origin = Location;
				int x = origin.X + parms.End0X;
				int y = origin.Y + parms.End0Y;
				int dx = parms.EndToEndDeltaX;
				int dy = parms.EndToEndDeltaY;
        
				EndData[] ends = new EndData[fanout + 1];
				ends[0] = new EndData(origin, BitWidth.create(bit_end.Length), EndData.INPUT_OUTPUT);
				for (int i = 0; i < fanout; i++)
				{
					ends[i + 1] = new EndData(new Location(x, y), BitWidth.create(end_width[i + 1]), EndData.INPUT_OUTPUT);
					x += dx;
					y += dy;
				}
				wire_data = new CircuitWires.SplitterData(fanout);
				setEnds(ends);
				recomputeBounds();
				fireComponentInvalidated(new ComponentEvent(this));
			}
		}

		//
		// user interface methods
		//
		public override void draw(ComponentDrawContext context)
		{
			SplitterAttributes attrs = (SplitterAttributes) AttributeSet;
			if (attrs.appear == SplitterAttributes.APPEAR_LEGACY)
			{
				SplitterPainter.drawLegacy(context, attrs, Location);
			}
			else
			{
				Location loc = Location;
				SplitterPainter.drawLines(context, attrs, loc);
				SplitterPainter.drawLabels(context, attrs, loc);
				context.drawPins(this);
			}
		}

		public override object getFeature(object key)
		{
			if (key == typeof(WireRepair))
			{
				return this;
			}
			if (key == typeof(ToolTipMaker))
			{
				return this;
			}
			if (key == typeof(MenuExtender))
			{
				return this;
			}
			else
			{
				return base.getFeature(key);
			}
		}

		public virtual bool shouldRepairWire(WireRepairData data)
		{
			return true;
		}

		public virtual string getToolTip(ComponentUserEvent e)
		{
			int end = -1;
			for (int i = getEnds().Count - 1; i >= 0; i--)
			{
				if (getEndLocation(i).manhattanDistanceTo(e.X, e.Y) < 10)
				{
					end = i;
					break;
				}
			}

			if (end == 0)
			{
				return Strings.get("splitterCombinedTip");
			}
			else if (end > 0)
			{
				int bits = 0;
				StringBuilder buf = new StringBuilder();
				SplitterAttributes attrs = (SplitterAttributes) AttributeSet;
				sbyte[] bit_end = attrs.bit_end;
				bool inString = false;
				int beginString = 0;
				for (int i = 0; i < bit_end.Length; i++)
				{
					if (bit_end[i] == end)
					{
						bits++;
						if (!inString)
						{
							inString = true;
							beginString = i;
						}
					}
					else
					{
						if (inString)
						{
							appendBuf(buf, beginString, i - 1);
							inString = false;
						}
					}
				}
				if (inString)
				{
					appendBuf(buf, beginString, bit_end.Length - 1);
				}
				string @base;
				switch (bits)
				{
				case 0:
					@base = Strings.get("splitterSplit0Tip");
					break;
				case 1:
					@base = Strings.get("splitterSplit1Tip");
					break;
				default:
					@base = Strings.get("splitterSplitManyTip");
					break;
				}
				return StringUtil.format(@base, buf.ToString());
			}
			else
			{
				return null;
			}
		}

		private static void appendBuf(StringBuilder buf, int start, int end)
		{
			if (buf.Length > 0)
			{
				buf.Append(",");
			}
			if (start == end)
			{
				buf.Append(start);
			}
			else
			{
				buf.Append(start + "-" + end);
			}
		}

		public virtual void configureMenu(JPopupMenu menu, Project proj)
		{
			menu.addSeparator();
			menu.add(new SplitterDistributeItem(proj, this, 1));
			menu.add(new SplitterDistributeItem(proj, this, -1));
		}

		//
		// AttributeListener methods
		//
		public virtual void attributeListChanged(AttributeEvent e)
		{
		}

		public virtual void attributeValueChanged(AttributeEvent e)
		{
			configureComponent();
		}
	}
}
