// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace gray
{

	using logisim.data;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using StringUtil = logisim.util.StringUtil;

	/// <summary>
	/// Manufactures a counter that iterates over Gray codes. This demonstrates several additional features beyond the
	/// SimpleGrayCounter class.
	/// </summary>
	internal class GrayCounter : InstanceFactory
	{
		public GrayCounter() : base("Gray Counter")
		{
			OffsetBounds = Bounds.create(-30, -15, 30, 30);
			setPorts(new Port[]
			{
				new Port(-30, 0, Port.INPUT, 1),
				new Port(0, 0, Port.OUTPUT, StdAttr.Width)
			});

			// We'll have width, label, and label font attributes. The latter two
			// attributes allow us to associate a label with the component (though
			// we'll also need configureNewInstance to configure the label's
			// location).
			setAttributes(new Attribute[] {StdAttr.Width, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {BitWidth.create(4), "", StdAttr.DEFAULT_LABEL_FONT});

			// The following method invocation sets things up so that the instance's
			// state can be manipulated using the Poke Tool.
			InstancePoker = typeof(CounterPoker);

			// These next two lines set it up so that the explorer window shows a
			// customized icon representing the component type. This should be a
			// 16x16 image.
			URL url = this.GetType().getClassLoader().getResource("com/cburch/gray/counter.gif");
			if (url != null)
			{
				Icon = new ImageIcon(url);
			}
		}

		/// <summary>
		/// The configureNewInstance method is invoked every time a new instance is created. In the superclass, the method
		/// doesn't do anything, since the new instance is pretty thoroughly configured already by default. But sometimes you
		/// need to do something particular to each instance, so you would override the method. In this case, we need to set
		/// up the location for its label.
		/// </summary>
		protected internal override void configureNewInstance(Instance instance)
		{
			Bounds bds = instance.Bounds;
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, bds.X + bds.Width / 2, bds.Y - 3, JGraphicsUtil.H_CENTER, JGraphicsUtil.V_BASELINE);
		}

		public override void propagate(InstanceState state)
		{
			// This is the same as with SimpleGrayCounter, except that we use the
			// StdAttr.Width attribute to determine the bit width to work with.
			BitWidth width = state.getAttributeValue(StdAttr.Width);
			CounterData cur = CounterData.get(state, width);
			bool trigger = cur.updateClock(state.getPort(0));
			if (trigger)
			{
				cur.Value = GrayIncrementer.nextGray(cur.Value);
			}
			state.setPort(1, cur.Value, 9);
		}

		public override void paintInstance(InstancePainter painter)
		{
			// This is essentially the same as with SimpleGrayCounter, except for
			// the invocation of painter.drawLabel to make the label be drawn.
			painter.drawBounds();
			painter.drawClock(0, Direction.East);
			painter.drawPort(1);
			painter.drawLabel();

			if (painter.ShowState)
			{
				BitWidth width = painter.getAttributeValue(StdAttr.Width);
				CounterData state = CounterData.get(painter, width);
				Bounds bds = painter.Bounds;
				JGraphicsUtil.drawCenteredText(painter.Graphics, StringUtil.toHexString(width.Width, state.Value.toIntValue()), bds.X + bds.Width / 2, bds.Y + bds.Height / 2);
			}
		}
	}

}
