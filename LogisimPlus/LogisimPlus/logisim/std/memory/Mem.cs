// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using HexModel = hex.HexModel;
	using HexModelListener = hex.HexModelListener;
	using CircuitState = logisim.circuit.CircuitState;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using HexFile = logisim.gui.hex.HexFile;
	using HexFrame = logisim.gui.hex.HexFrame;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using Project = logisim.proj.Project;
	using MenuExtender = logisim.tools.MenuExtender;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using StringGetter = logisim.util.StringGetter;
	using StringUtil = logisim.util.StringUtil;

	internal abstract class Mem : InstanceFactory
	{
		// Note: The code is meant to be able to handle up to 32-bit addresses, but it
		// hasn't been debugged thoroughly. There are two definite changes I would
		// make if I were to extend the address bits: First, there would need to be some
		// modification to the memory's graphical representation, because there isn't
		// room in the box to include such long memory addresses with the current font
		// size. And second, I'd alter the MemContents class's PAGE_SIZE_BITS constant
		// to 14 so that its "page table" isn't quite so big.
		public static readonly Attribute ADDR_ATTR = Attributes.forBitWidth("addrWidth", Strings.getter("ramAddrWidthAttr"), 2, 24);
		public static readonly Attribute DATA_ATTR = Attributes.forBitWidth("dataWidth", Strings.getter("ramDataWidthAttr"));

		// port-related constants
		internal const int DATA = 0;
		internal const int ADDR = 1;
		internal const int CS = 2;
		internal const int MEM_INPUTS = 3;

		// other constants
		internal const int DELAY = 10;

		private WeakHashMap<Instance, File> currentInstanceFiles;

		internal Mem(string name, StringGetter desc, int extraPorts) : base(name, desc)
		{
			currentInstanceFiles = new WeakHashMap<Instance, File>();
			InstancePoker = typeof(MemPoker);
			KeyConfigurator = JoinedConfigurator.create(new BitWidthConfigurator(ADDR_ATTR, 2, 24, 0), new BitWidthConfigurator(DATA_ATTR));

			OffsetBounds = Bounds.create(-140, -40, 140, 80);
		}

		internal abstract void configurePorts(Instance instance);

		public override abstract AttributeSet createAttributeSet();

		internal abstract MemState getState(InstanceState state);

		internal abstract MemState getState(Instance instance, CircuitState state);

		internal abstract HexFrame getHexFrame(Project proj, Instance instance, CircuitState state);

		public override abstract void propagate(InstanceState state);

		protected internal override void configureNewInstance(Instance instance)
		{
			configurePorts(instance);
		}

		internal virtual void configureStandardPorts(Instance instance, Port[] ps)
		{
			ps[DATA] = new Port(0, 0, Port.INOUT, DATA_ATTR);
			ps[ADDR] = new Port(-140, 0, Port.INPUT, ADDR_ATTR);
			ps[CS] = new Port(-90, 40, Port.INPUT, 1);
			ps[DATA].ToolTip = Strings.getter("memDataTip");
			ps[ADDR].ToolTip = Strings.getter("memAddrTip");
			ps[CS].ToolTip = Strings.getter("memCSTip");
		}

		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			Bounds bds = painter.Bounds;

			// draw boundary
			painter.drawBounds();

			// draw contents
			if (painter.ShowState)
			{
				MemState state = getState(painter);
				state.paint(painter.Graphics, bds.X, bds.Y);
			}
			else
			{
				BitWidth addr = painter.getAttributeValue(ADDR_ATTR);
				int addrBits = addr.Width;
				int bytes = 1 << addrBits;
				string label;
				if (this is Rom)
				{
					if (addrBits >= 30)
					{
						label = StringUtil.format(Strings.get("romGigabyteLabel"), "" + (bytes >>> 30));
					}
					else if (addrBits >= 20)
					{
						label = StringUtil.format(Strings.get("romMegabyteLabel"), "" + (bytes >> 20));
					}
					else if (addrBits >= 10)
					{
						label = StringUtil.format(Strings.get("romKilobyteLabel"), "" + (bytes >> 10));
					}
					else
					{
						label = StringUtil.format(Strings.get("romByteLabel"), "" + bytes);
					}
				}
				else
				{
					if (addrBits >= 30)
					{
						label = StringUtil.format(Strings.get("ramGigabyteLabel"), "" + (bytes >>> 30));
					}
					else if (addrBits >= 20)
					{
						label = StringUtil.format(Strings.get("ramMegabyteLabel"), "" + (bytes >> 20));
					}
					else if (addrBits >= 10)
					{
						label = StringUtil.format(Strings.get("ramKilobyteLabel"), "" + (bytes >> 10));
					}
					else
					{
						label = StringUtil.format(Strings.get("ramByteLabel"), "" + bytes);
					}
				}
				JGraphicsUtil.drawCenteredText(g, label, bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			}

			// draw input and output ports
			painter.drawPort(DATA, Strings.get("ramDataLabel"), Direction.West);
			painter.drawPort(ADDR, Strings.get("ramAddrLabel"), Direction.East);
			g.setColor(Color.Gray);
			painter.drawPort(CS, Strings.get("ramCSLabel"), Direction.South);
		}

		internal virtual File getCurrentImage(Instance instance)
		{
			return currentInstanceFiles.get(instance);
		}

		internal virtual void setCurrentImage(Instance instance, File value)
		{
			currentInstanceFiles.put(instance, value);
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public void loadImage(logisim.instance.InstanceState instanceState, java.io.File imageFile) throws java.io.IOException
		public virtual void loadImage(InstanceState instanceState, File imageFile)
		{
			MemState s = this.getState(instanceState);
			HexFile.open(s.Contents, imageFile);
			this.setCurrentImage(instanceState.Instance, imageFile);
		}

		protected internal override object getInstanceFeature(Instance instance, object key)
		{
			if (key == typeof(MenuExtender))
			{
				return new MemMenu(this, instance);
			}
			return base.getInstanceFeature(instance, key);
		}

		internal class MemListener : HexModelListener
		{
			internal Instance instance;

			internal MemListener(Instance instance)
			{
				this.instance = instance;
			}

			public virtual void metainfoChanged(HexModel source)
			{
			}

			public virtual void bytesChanged(HexModel source, long start, long numBytes, int[] values)
			{
				instance.fireInvalidated();
			}
		}
	}

}
