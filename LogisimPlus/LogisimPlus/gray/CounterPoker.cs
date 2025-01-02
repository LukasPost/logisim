// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace gray
{

	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Value = logisim.data.Value;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstancePoker = logisim.instance.InstancePoker;
	using InstanceState = logisim.instance.InstanceState;
	using StdAttr = logisim.instance.StdAttr;

	/// <summary>
	/// When the user clicks a counter using the Poke Tool, a CounterPoker object is created, and that object will handle all
	/// user events. Note that CounterPoker is a class specific to GrayCounter, and that it must be a subclass of
	/// InstancePoker in the logisim.instance package.
	/// </summary>
	public class CounterPoker : InstancePoker
	{
		public CounterPoker()
		{
		}

		/// <summary>
		/// Determines whether the location the mouse was pressed should result in initiating a poke.
		/// </summary>
		public override bool init(InstanceState state, MouseEvent e)
		{
			return state.Instance.Bounds.contains(e.getX(), e.getY());
			// Anywhere in the main rectangle initiates the poke. The user might
			// have clicked within a label, but that will be outside the bounds.
		}

		/// <summary>
		/// Draws an indicator that the caret is being selected. Here, we'll draw a red rectangle around the value.
		/// </summary>
		public override void paint(InstancePainter painter)
		{
			Bounds bds = painter.Bounds;
			BitWidth width = painter.getAttributeValue(StdAttr.WIDTH);
			int len = (width.Width + 3) / 4;

			Graphics g = painter.Graphics;
			g.setColor(Color.RED);
			int wid = 7 * len + 2; // width of caret rectangle
			int ht = 16; // height of caret rectangle
			g.drawRect(bds.X + (bds.Width - wid) / 2, bds.Y + (bds.Height - ht) / 2, wid, ht);
			g.setColor(Color.BLACK);
		}

		/// <summary>
		/// Processes a key by just adding it onto the end of the current value. </summary>
		public override void keyTyped(InstanceState state, KeyEvent e)
		{
			// convert it to a hex digit; if it isn't a hex digit, abort.
			int val = Character.digit(e.getKeyChar(), 16);
			BitWidth width = state.getAttributeValue(StdAttr.WIDTH);
			if (val < 0 || (val & width.Mask) != val)
			{
				return;
			}

			// compute the next value
			CounterData cur = CounterData.get(state, width);
			int newVal = (cur.Value.toIntValue() * 16 + val) & width.Mask;
			Value newValue = Value.createKnown(width, newVal);
			cur.Value = newValue;
			state.fireInvalidated();

			// You might be tempted to propagate the value immediately here, using
			// state.setPort. However, the circuit may currently be propagating in
			// another thread, and invoking setPort directly could interfere with
			// that. Using fireInvalidated notifies the propagation thread to
			// invoke propagate on the counter at its next opportunity.
		}
	}

}
