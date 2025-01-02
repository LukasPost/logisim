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
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Value = logisim.data.Value;
	using HexFile = logisim.gui.hex.HexFile;
	using HexFrame = logisim.gui.hex.HexFrame;
	using Frame = logisim.gui.main.Frame;
	using Instance = logisim.instance.Instance;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using Project = logisim.proj.Project;

	public class Rom : Mem
	{
		public static Attribute<MemContents> CONTENTS_ATTR = new ContentsAttribute();

		// The following is so that instance's MemListeners aren't freed by the
		// garbage collector until the instance itself is ready to be freed.
		private WeakHashMap<Instance, MemListener> memListeners;

		public Rom() : base("ROM", Strings.getter("romComponent"), 0)
		{
			IconName = "rom.gif";
			memListeners = new WeakHashMap<Instance, MemListener>();
		}

		internal override void configurePorts(Instance instance)
		{
			Port[] ps = new Port[MEM_INPUTS];
			configureStandardPorts(instance, ps);
			instance.setPorts(ps);
		}

		public override AttributeSet createAttributeSet()
		{
			return new RomAttributes();
		}

		internal override MemState getState(Instance instance, CircuitState state)
		{
			MemState ret = (MemState) instance.getData(state);
			if (ret == null)
			{
				MemContents contents = getMemContents(instance);
				ret = new MemState(contents);
				instance.setData(state, ret);
			}
			return ret;
		}

		internal override MemState getState(InstanceState state)
		{
			MemState ret = (MemState) state.Data;
			if (ret == null)
			{
				MemContents contents = getMemContents(state.Instance);
				ret = new MemState(contents);
				state.Data = ret;
			}
			return ret;
		}

		internal override HexFrame getHexFrame(Project proj, Instance instance, CircuitState state)
		{
			return RomAttributes.getHexFrame(getMemContents(instance), proj);
		}

		// TODO - maybe delete this method?
		internal virtual MemContents getMemContents(Instance instance)
		{
			return instance.getAttributeValue(CONTENTS_ATTR);
		}

		public override void propagate(InstanceState state)
		{
			MemState myState = getState(state);
			BitWidth dataBits = state.getAttributeValue(DATA_ATTR);

			Value addrValue = state.getPort(ADDR);
			bool chipSelect = state.getPort(CS) != Value.FALSE;

			if (!chipSelect)
			{
				myState.Current = -1;
				state.setPort(DATA, Value.createUnknown(dataBits), DELAY);
				return;
			}

			int addr = addrValue.toIntValue();
			if (!addrValue.FullyDefined || addr < 0)
			{
				return;
			}
			if (addr != myState.Current)
			{
				myState.Current = addr;
				myState.scrollToShow(addr);
			}

			int val = myState.Contents.get(addr);
			state.setPort(DATA, Value.createKnown(dataBits, val), DELAY);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			base.configureNewInstance(instance);
			MemContents contents = getMemContents(instance);
			MemListener listener = new MemListener(instance);
			memListeners.put(instance, listener);
			contents.addHexModelListener(listener);
		}

		private class ContentsAttribute : Attribute<MemContents>
		{
			public ContentsAttribute() : base("contents", Strings.getter("romContentsAttr"))
			{
			}

			public override java.awt.Component getCellEditor(Window source, MemContents value)
			{
				if (source is Frame)
				{
					Project proj = ((Frame) source).Project;
					RomAttributes.register(value, proj);
				}
				ContentsCell ret = new ContentsCell(source, value);
				ret.mouseClicked(null);
				return ret;
			}

			public override string toDisplayString(MemContents value)
			{
				return Strings.get("romContentsValue");
			}

			public override string toStandardString(MemContents state)
			{
				int addr = state.LogLength;
				int data = state.Width;
				StringWriter ret = new StringWriter();
				ret.write("addr/data: " + addr + " " + data + "\n");
				try
				{
					HexFile.save(ret, state);
				}
				catch (IOException)
				{
				}
				return ret.ToString();
			}

			public override MemContents parse(string value)
			{
				int lineBreak = value.IndexOf('\n');
				string first = lineBreak < 0 ? value : value.Substring(0, lineBreak);
				string rest = lineBreak < 0 ? "" : value.Substring(lineBreak + 1);
				StringTokenizer toks = new StringTokenizer(first);
				try
				{
					string header = toks.nextToken();
					if (!header.Equals("addr/data:"))
					{
						return null;
					}
					int addr = int.Parse(toks.nextToken());
					int data = int.Parse(toks.nextToken());
					MemContents ret = MemContents.create(addr, data);
					HexFile.open(ret, new StringReader(rest));
					return ret;
				}
				catch (IOException)
				{
					return null;
				}
				catch (System.FormatException)
				{
					return null;
				}
				catch (NoSuchElementException)
				{
					return null;
				}
			}
		}

		private class ContentsCell : JLabel, MouseListener
		{
			internal Window source;
			internal MemContents contents;

			internal ContentsCell(Window source, MemContents contents) : base(Strings.get("romContentsValue"))
			{
				this.source = source;
				this.contents = contents;
				addMouseListener(this);
			}

			public virtual void mouseClicked(MouseEvent e)
			{
				if (contents == null)
				{
					return;
				}
				Project proj = source is Frame ? ((Frame) source).Project : null;
				HexFrame frame = RomAttributes.getHexFrame(contents, proj);
				frame.Visible = true;
				frame.toFront();
			}

			public virtual void mousePressed(MouseEvent e)
			{
			}

			public virtual void mouseReleased(MouseEvent e)
			{
			}

			public virtual void mouseEntered(MouseEvent e)
			{
			}

			public virtual void mouseExited(MouseEvent e)
			{
			}
		}
	}

}
