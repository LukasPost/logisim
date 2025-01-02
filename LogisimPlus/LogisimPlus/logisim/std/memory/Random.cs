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

	using logisim.data;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceData = logisim.instance.InstanceData;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstanceLogger = logisim.instance.InstanceLogger;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using StringUtil = logisim.util.StringUtil;

	public class Random : InstanceFactory
	{
		private static readonly Attribute<int> ATTR_SEED = Attributes.forInteger("seed", Strings.getter("randomSeedAttr"));

		private const int OUT = 0;
		private const int CK = 1;
		private const int NXT = 2;
		private const int RST = 3;

		public Random() : base("Random", Strings.getter("randomComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.WIDTH, ATTR_SEED, StdAttr.EDGE_TRIGGER, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {BitWidth.create(8), Convert.ToInt32(0), StdAttr.TRIG_RISING, "", StdAttr.DEFAULT_LABEL_FONT});
			KeyConfigurator = new BitWidthConfigurator(StdAttr.WIDTH);

			OffsetBounds = Bounds.create(-30, -20, 30, 40);
			IconName = "random.gif";
			InstanceLogger = typeof(Logger);

			Port[] ps = new Port[4];
			ps[OUT] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			ps[CK] = new Port(-30, -10, Port.INPUT, 1);
			ps[NXT] = new Port(-30, 10, Port.INPUT, 1);
			ps[RST] = new Port(-20, 20, Port.INPUT, 1);
			ps[OUT].setToolTip(Strings.getter("randomQTip"));
			ps[CK].setToolTip(Strings.getter("randomClockTip"));
			ps[NXT].setToolTip(Strings.getter("randomNextTip"));
			ps[RST].setToolTip(Strings.getter("randomResetTip"));
			setPorts(ps);
		}

		protected internal override void configureNewInstance(Instance instance)
		{
			Bounds bds = instance.Bounds;
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.X + bds.Width / 2, bds.Y - 3, GraphicsUtil.H_CENTER, GraphicsUtil.V_BASELINE);
		}

		public override void propagate(InstanceState state)
		{
			StateData data = (StateData) state.Data;
			if (data == null)
			{
				data = new StateData(state.getAttributeValue(ATTR_SEED));
				state.Data = data;
			}

			BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
			object triggerType = state.getAttributeValue(StdAttr.EDGE_TRIGGER);
			bool triggered = data.updateClock(state.getPort(CK), triggerType);

			if (state.getPort(RST) == Value.TRUE)
			{
				data.reset(state.getAttributeValue(ATTR_SEED));
			}
			else if (triggered && state.getPort(NXT) != Value.FALSE)
			{
				data.step();
			}

			state.setPort(OUT, Value.createKnown(dataWidth, data.value), 4);
		}

		public override void paintInstance(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			Bounds bds = painter.Bounds;
			StateData state = (StateData) painter.Data;
			BitWidth widthVal = painter.getAttributeValue(StdAttr.WIDTH);
			int width = widthVal == null ? 8 : widthVal.Width;

			// draw boundary, label
			painter.drawBounds();
			painter.drawLabel();

			// draw input and output ports
			painter.drawPort(OUT, "Q", Direction.West);
			painter.drawPort(RST);
			painter.drawPort(NXT);
			painter.drawClock(CK, Direction.East);

			// draw contents
			if (painter.ShowState)
			{
				int val = state == null ? 0 : state.value;
				string str = StringUtil.toHexString(width, val);
				if (str.Length <= 4)
				{
					GraphicsUtil.drawText(g, str, bds.X + 15, bds.Y + 4, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
				}
				else
				{
					int split = str.Length - 4;
					GraphicsUtil.drawText(g, str.Substring(0, split), bds.X + 15, bds.Y + 3, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
					GraphicsUtil.drawText(g, str.Substring(split), bds.X + 15, bds.Y + 15, GraphicsUtil.H_CENTER, GraphicsUtil.V_TOP);
				}
			}
		}

		private class StateData : ClockState, InstanceData
		{
			internal const long multiplier = 0x5DEECE66DL;
			internal const long addend = 0xBL;
			internal static readonly long mask = (1L << 48) - 1;

			internal long initSeed;
			internal long curSeed;
			internal int value;

			public StateData(object seed)
			{
				reset(seed);
			}

			internal virtual void reset(object seed)
			{
				long start = seed is int? ? ((int?) seed).Value : 0;
				if (start == 0)
				{
					// Prior to 2.7.0, this would reset to the seed at the time of
					// the StateData's creation. It seems more likely that what
					// would be intended was starting a new sequence entirely...
					start = (DateTimeHelper.CurrentUnixTimeMillis() ^ multiplier) & mask;
					if (start == initSeed)
					{
						start = (start + multiplier) & mask;
					}
				}
				this.initSeed = start;
				this.curSeed = start;
				this.value = (int) start;
			}

			internal virtual void step()
			{
				long v = curSeed;
				v = (v * multiplier + addend) & mask;
				curSeed = v;
				value = (int)(v >> 12);
			}
		}

		public class Logger : InstanceLogger
		{
			public override string getLogName(InstanceState state, object option)
			{
				string ret = state.getAttributeValue(StdAttr.LABEL);
				return !string.ReferenceEquals(ret, null) && !ret.Equals("") ? ret : null;
			}

			public override Value getLogValue(InstanceState state, object option)
			{
				BitWidth dataWidth = state.getAttributeValue(StdAttr.WIDTH);
				if (dataWidth == null)
				{
					dataWidth = BitWidth.create(0);
				}
				StateData data = (StateData) state.Data;
				if (data == null)
				{
					return Value.createKnown(dataWidth, 0);
				}
				return Value.createKnown(dataWidth, data.value);
			}
		}
	}
}
