﻿// ====================================================================================================
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
	using Value = logisim.data.Value;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;

	/// <summary>
	/// This component takes a multibit input and outputs the value that follows it in Gray Code. For instance, given input
	/// 0100 the output is 1100.
	/// </summary>
	internal class GrayIncrementer : InstanceFactory
	{
		/*
		 * Note that there are no instance variables. There is only one instance of this class created, which manages all
		 * instances of the component. Any information associated with individual instances should be handled through
		 * attributes. For GrayIncrementer, each instance has a "bit width" that it works with, and so we'll have an
		 * attribute.
		 */

		/// <summary>
		/// The constructor configures the factory. </summary>
		internal GrayIncrementer() : base("Gray Code Incrementer")
		{

			/*
			 * This is how we can set up the attributes for GrayIncrementers. In this case, there is just one attribute -
			 * the width - whose default is 4. The StdAttr class defines several commonly occurring attributes, including
			 * one for "bit width." It's best to use those StdAttr attributes when appropriate: A user can then select
			 * several components (even from differing factories) with the same attribute and modify them all at once.
			 */
			setAttributes(new Attribute[] {StdAttr.Width}, new object[] {BitWidth.create(4)});

			/*
			 * The "offset bounds" is the location of the bounding rectangle relative to the mouse location. Here, we're
			 * choosing the component to be 30x30, and we're anchoring it relative to its primary output (as is typical for
			 * Logisim), which happens to be in the center of the east edge. Thus, the top left corner of the bounding box
			 * is 30 pixels west and 15 pixels north of the mouse location.
			 */
			OffsetBounds = Bounds.create(-30, -15, 30, 30);

			/*
			 * The ports are locations where wires can be connected to this component. Each port object says where to find
			 * the port relative to the component's anchor location, then whether the port is an input/output/both, and
			 * finally the expected bit width for the port. The bit width can be a constant (like 1) or an attribute (as
			 * here).
			 */
			setPorts(new Port[]
			{
				new Port(-30, 0, Port.INPUT, StdAttr.Width),
				new Port(0, 0, Port.OUTPUT, StdAttr.Width)
			});
		}

		/// <summary>
		/// Computes the current output for this component. This method is invoked any time any of the inputs change their
		/// values; it may also be invoked in other circumstances, even if there is no reason to expect it to change
		/// anything.
		/// </summary>
		public override void propagate(InstanceState state)
		{
			// First we retrieve the value being fed into the input. Note that in
			// the setPorts invocation above, the component's input was included at
			// index 0 in the parameter array, so we use 0 as the parameter below.
			Value @in = state.getPort(0);

			// Now compute the output. We've farmed this out to a helper method,
			// since the same logic is needed for the library's other components.
			Value @out = nextGray(@in);

			// Finally we propagate the output into the circuit. The first parameter
			// is 1 because in our list of ports (configured by invocation of
			// setPorts above) the output is at index 1. The second parameter is the
			// value we want to send on that port. And the last parameter is its
			// "delay" - the number of steps it will take for the output to update
			// after its input.
			state.setPort(1, @out, @out.Width + 1);
		}

		/// <summary>
		/// Says how an individual instance should appear on the canvas. </summary>
		public override void paintInstance(InstancePainter painter)
		{
			// As it happens, InstancePainter contains several convenience methods
			// for drawing, and we'll use those here. Frequently, you'd want to
			// retrieve its JGraphics object (painter.getJGraphics) so you can draw
			// directly onto the canvas.
			painter.drawRectangle(painter.Bounds, "G+1");
			painter.drawPorts();
		}

		/// <summary>
		/// Computes the next gray value in the sequence after prev. This static method just does some bit twiddling; it
		/// doesn't have much to do with Logisim except that it manipulates Value and BitWidth objects.
		/// </summary>
		internal static Value nextGray(Value prev)
		{
			BitWidth bits = prev.BitWidth;
			if (!prev.FullyDefined)
			{
				return Value.createError(bits);
			}
			int x = prev.toIntValue();
			int ct = (x >> 16) ^ x; // compute parity of x
			ct = (ct >> 8) ^ ct;
			ct = (ct >> 4) ^ ct;
			ct = (ct >> 2) ^ ct;
			ct = (ct >> 1) ^ ct;
			if ((ct & 1) == 0)
			{ // if parity is even, flip 1's bit
				x = x ^ 1;
			}
			else
			{ // else flip bit just above last 1
				int y = x ^ (x & (x - 1)); // first compute the last 1
				y = (y << 1) & bits.Mask;
				x = (y == 0 ? 0 : x ^ y);
			}
			return Value.createKnown(bits, x);
		}
	}

}
