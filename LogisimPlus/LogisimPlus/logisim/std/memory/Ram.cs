// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.memory
{

	using CircuitState = logisim.circuit.CircuitState;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using AttributeSets = logisim.data.AttributeSets;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using HexFrame = logisim.gui.hex.HexFrame;
	using Instance = logisim.instance.Instance;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using Project = logisim.proj.Project;

	public class Ram : Mem
	{
		internal static readonly AttributeOption BUS_COMBINED = new AttributeOption("combined", Strings.getter("ramBusSynchCombined"));
		internal static readonly AttributeOption BUS_ASYNCH = new AttributeOption("asynch", Strings.getter("ramBusAsynchCombined"));
		internal static readonly AttributeOption BUS_SEPARATE = new AttributeOption("separate", Strings.getter("ramBusSeparate"));

		internal static readonly Attribute<AttributeOption> ATTR_BUS = Attributes.forOption("bus", Strings.getter("ramBusAttr"), new AttributeOption[] {BUS_COMBINED, BUS_ASYNCH, BUS_SEPARATE});

// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private static logisim.data.Attribute<?>[] ATTRIBUTES = { Mem.ADDR_ATTR, Mem.DATA_ATTR, ATTR_BUS };
		private static Attribute<object>[] ATTRIBUTES = new Attribute<object>[] {Mem.ADDR_ATTR, Mem.DATA_ATTR, ATTR_BUS};
		private static object[] DEFAULTS = new object[] {BitWidth.create(8), BitWidth.create(8), BUS_COMBINED};

		private static readonly int OE = MEM_INPUTS + 0;
		private static readonly int CLR = MEM_INPUTS + 1;
		private static readonly int CLK = MEM_INPUTS + 2;
		private static readonly int WE = MEM_INPUTS + 3;
		private static readonly int DIN = MEM_INPUTS + 4;

		private static object[][] logOptions = new object[9][];

		public Ram() : base("RAM", Strings.getter("ramComponent"), 3)
		{
			IconName = "ram.gif";
			InstanceLogger = typeof(Logger);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			base.configureNewInstance(instance);
			instance.addAttributeListener();
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			base.instanceAttributeChanged(instance, attr);
			configurePorts(instance);
		}

		internal override void configurePorts(Instance instance)
		{
			object bus = instance.getAttributeValue(ATTR_BUS);
			if (bus == null)
			{
				bus = BUS_COMBINED;
			}
			bool asynch = bus == null ? false : bus.Equals(BUS_ASYNCH);
			bool separate = bus == null ? false : bus.Equals(BUS_SEPARATE);

			int portCount = MEM_INPUTS;
			if (asynch)
			{
				portCount += 2;
			}
			else if (separate)
			{
				portCount += 5;
			}
			else
			{
				portCount += 3;
			}
			Port[] ps = new Port[portCount];

			configureStandardPorts(instance, ps);
			ps[OE] = new Port(-50, 40, Port.INPUT, 1);
			ps[OE].setToolTip(Strings.getter("ramOETip"));
			ps[CLR] = new Port(-30, 40, Port.INPUT, 1);
			ps[CLR].setToolTip(Strings.getter("ramClrTip"));
			if (!asynch)
			{
				ps[CLK] = new Port(-70, 40, Port.INPUT, 1);
				ps[CLK].setToolTip(Strings.getter("ramClkTip"));
			}
			if (separate)
			{
				ps[WE] = new Port(-110, 40, Port.INPUT, 1);
				ps[WE].setToolTip(Strings.getter("ramWETip"));
				ps[DIN] = new Port(-140, 20, Port.INPUT, DATA_ATTR);
				ps[DIN].setToolTip(Strings.getter("ramInTip"));
			}
			else
			{
				ps[DATA].setToolTip(Strings.getter("ramBusTip"));
			}
			instance.setPorts(ps);
		}

		public override AttributeSet createAttributeSet()
		{
			return AttributeSets.fixedSet(ATTRIBUTES, DEFAULTS);
		}

		internal override MemState getState(InstanceState state)
		{
			BitWidth addrBits = state.getAttributeValue(ADDR_ATTR);
			BitWidth dataBits = state.getAttributeValue(DATA_ATTR);

			RamState myState = (RamState) state.Data;
			if (myState == null)
			{
				MemContents contents = MemContents.create(addrBits.Width, dataBits.Width);
				Instance instance = state.Instance;
				myState = new RamState(instance, contents, new MemListener(instance));
				state.Data = myState;
			}
			else
			{
				myState.Ram = state.Instance;
			}
			return myState;
		}

		internal override MemState getState(Instance instance, CircuitState state)
		{
			BitWidth addrBits = instance.getAttributeValue(ADDR_ATTR);
			BitWidth dataBits = instance.getAttributeValue(DATA_ATTR);

			RamState myState = (RamState) instance.getData(state);
			if (myState == null)
			{
				MemContents contents = MemContents.create(addrBits.Width, dataBits.Width);
				myState = new RamState(instance, contents, new MemListener(instance));
				instance.setData(state, myState);
			}
			else
			{
				myState.Ram = instance;
			}
			return myState;
		}

		internal override HexFrame getHexFrame(Project proj, Instance instance, CircuitState circState)
		{
			RamState state = (RamState) getState(instance, circState);
			return state.getHexFrame(proj);
		}

		public override void propagate(InstanceState state)
		{
			RamState myState = (RamState) getState(state);
			BitWidth dataBits = state.getAttributeValue(DATA_ATTR);
			object busVal = state.getAttributeValue(ATTR_BUS);
			bool asynch = busVal == null ? false : busVal.Equals(BUS_ASYNCH);
			bool separate = busVal == null ? false : busVal.Equals(BUS_SEPARATE);

			Value addrValue = state.getPort(ADDR);
			bool chipSelect = state.getPort(CS) != Value.FALSE;
			bool triggered = asynch || myState.setClock(state.getPort(CLK), StdAttr.TRIG_RISING);
			bool outputEnabled = state.getPort(OE) != Value.FALSE;
			bool shouldClear = state.getPort(CLR) == Value.TRUE;

			if (shouldClear)
			{
				myState.Contents.clear();
			}

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

			if (!shouldClear && triggered)
			{
				bool shouldStore;
				if (separate)
				{
					shouldStore = state.getPort(WE) != Value.FALSE;
				}
				else
				{
					shouldStore = !outputEnabled;
				}
				if (shouldStore)
				{
					Value dataValue = state.getPort(separate ? DIN : DATA);
					myState.Contents.set(addr, dataValue.toIntValue());
				}
			}

			if (outputEnabled)
			{
				int val = myState.Contents.get(addr);
				state.setPort(DATA, Value.createKnown(dataBits, val), DELAY);
			}
			else
			{
				state.setPort(DATA, Value.createUnknown(dataBits), DELAY);
			}
		}

		public override void paintInstance(InstancePainter painter)
		{
			base.paintInstance(painter);
			object busVal = painter.getAttributeValue(ATTR_BUS);
			bool asynch = busVal == null ? false : busVal.Equals(BUS_ASYNCH);
			bool separate = busVal == null ? false : busVal.Equals(BUS_SEPARATE);

			if (!asynch)
			{
				painter.drawClock(CLK, Direction.North);
			}
			painter.drawPort(OE, Strings.get("ramOELabel"), Direction.South);
			painter.drawPort(CLR, Strings.get("ramClrLabel"), Direction.South);

			if (separate)
			{
				painter.drawPort(WE, Strings.get("ramWELabel"), Direction.South);
				painter.Graphics.setColor(Color.BLACK);
				painter.drawPort(DIN, Strings.get("ramDataLabel"), Direction.East);
			}
		}

		private class RamState : MemState, AttributeListener
		{
			internal Instance parent;
			internal MemListener listener;
			internal HexFrame hexFrame = null;
			internal ClockState clockState;

			internal RamState(Instance parent, MemContents contents, MemListener listener) : base(contents)
			{
				this.parent = parent;
				this.listener = listener;
				this.clockState = new ClockState();
				if (parent != null)
				{
					parent.AttributeSet.addAttributeListener(this);
				}
				contents.addHexModelListener(listener);
			}

			internal virtual Instance Ram
			{
				set
				{
					if (parent == value)
					{
						return;
					}
					if (parent != null)
					{
						parent.AttributeSet.removeAttributeListener(this);
					}
					parent = value;
					if (value != null)
					{
						value.AttributeSet.addAttributeListener(this);
					}
				}
			}

			public override RamState clone()
			{
				RamState ret = (RamState) base.clone();
				ret.parent = null;
				ret.clockState = this.clockState.clone();
				ret.Contents.addHexModelListener(listener);
				return ret;
			}

			// Retrieves a HexFrame for editing within a separate window
			public virtual HexFrame getHexFrame(Project proj)
			{
				if (hexFrame == null)
				{
					hexFrame = new HexFrame(proj, Contents);
					hexFrame.addWindowListener(new WindowAdapterAnonymousInnerClass(this));
				}
				return hexFrame;
			}

			private class WindowAdapterAnonymousInnerClass : WindowAdapter
			{
				private readonly RamState outerInstance;

				public WindowAdapterAnonymousInnerClass(RamState outerInstance)
				{
					this.outerInstance = outerInstance;
				}

				public override void windowClosed(WindowEvent e)
				{
					outerInstance.hexFrame = null;
				}
			}

			//
			// methods for accessing the write-enable data
			//
			public virtual bool setClock(Value newClock, object trigger)
			{
				return clockState.updateClock(newClock, trigger);
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				AttributeSet attrs = e.Source;
				BitWidth addrBits = attrs.getValue(Mem.ADDR_ATTR);
				BitWidth dataBits = attrs.getValue(Mem.DATA_ATTR);
				Contents.setSizes(addrBits.Width, dataBits.Width);
			}
		}

		public class Logger : InstanceLogger
		{
			public override object[] getLogOptions(InstanceState state)
			{
				int addrBits = state.getAttributeValue(ADDR_ATTR).getWidth();
				if (addrBits >= logOptions.Length)
				{
					addrBits = logOptions.Length - 1;
				}
				lock (logOptions)
				{
					object[] ret = logOptions[addrBits];
					if (ret == null)
					{
						ret = new object[1 << addrBits];
						logOptions[addrBits] = ret;
						for (int i = 0; i < ret.Length; i++)
						{
							ret[i] = Convert.ToInt32(i);
						}
					}
					return ret;
				}
			}

			public override string getLogName(InstanceState state, object option)
			{
				if (option is int?)
				{
					string disp = Strings.get("ramComponent");
					Location loc = state.Instance.Location;
					return disp + loc + "[" + option + "]";
				}
				else
				{
					return null;
				}
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				if (option is int?)
				{
					MemState s = (MemState) state.Data;
					int addr = ((int?) option).Value;
					return Value.createKnown(BitWidth.create(s.DataBits), s.Contents.get(addr));
				}
				else
				{
					return Value.NIL;
				}
			}
		}
	}

}
