// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit.appear
{

	using Document = org.w3c.dom.Document;
	using Element = org.w3c.dom.Element;

	using CanvasObject = draw.model.CanvasObject;
	using Handle = draw.model.Handle;
	using HandleGesture = draw.model.HandleGesture;
	using Bounds = logisim.data.Bounds;
	using Location = logisim.data.Location;
	using Instance = logisim.instance.Instance;
	using Pin = logisim.std.wiring.Pin;
	using logisim.util;

	public class AppearancePort : AppearanceElement
	{
		private const int INPUT_RADIUS = 4;
		private const int OUTPUT_RADIUS = 5;
		private const int MINOR_RADIUS = 2;
		public static readonly Color COLOR = Color.BLUE;

		private Instance pin;

		public AppearancePort(Location location, Instance pin) : base(location)
		{
			this.pin = pin;
		}

		public override bool matches(CanvasObject other)
		{
			if (other is AppearancePort)
			{
				AppearancePort that = (AppearancePort) other;
				return this.matches(that) && this.pin == that.pin;
			}
			else
			{
				return false;
			}
		}

		public override int matchesHashCode()
		{
			return base.matchesHashCode() + pin.GetHashCode();
		}

		public override string DisplayName
		{
			get
			{
				return Strings.get("circuitPort");
			}
		}

		public override Element toSvgElement(Document doc)
		{
			Location loc = Location;
			Location pinLoc = pin.Location;
			Element ret = doc.createElement("circ-port");
			int r = Input ? INPUT_RADIUS : OUTPUT_RADIUS;
			ret.setAttribute("x", "" + (loc.X - r));
			ret.setAttribute("y", "" + (loc.Y - r));
			ret.setAttribute("width", "" + 2 * r);
			ret.setAttribute("height", "" + 2 * r);
			ret.setAttribute("pin", "" + pinLoc.X + "," + pinLoc.Y);
			return ret;
		}

		public virtual Instance Pin
		{
			get
			{
				return pin;
			}
			set
			{
				pin = value;
			}
		}


		private bool Input
		{
			get
			{
				Instance p = pin;
				return p == null || Pin.FACTORY.isInputPin(p);
			}
		}

		public override Bounds Bounds
		{
			get
			{
				int r = Input ? INPUT_RADIUS : OUTPUT_RADIUS;
				return base.getBounds(r);
			}
		}

		public override bool contains(Location loc, bool assumeFilled)
		{
			if (Input)
			{
				return Bounds.contains(loc);
			}
			else
			{
				return base.isInCircle(loc, OUTPUT_RADIUS);
			}
		}

		public override IList<Handle> getHandles(HandleGesture gesture)
		{
			Location loc = Location;

			int r = Input ? INPUT_RADIUS : OUTPUT_RADIUS;
			return UnmodifiableList.create(new Handle[]
			{
				new Handle(this, loc.translate(-r, -r)),
				new Handle(this, loc.translate(r, -r)),
				new Handle(this, loc.translate(r, r)),
				new Handle(this, loc.translate(-r, r))
			});
		}

		public override void paint(Graphics g, HandleGesture gesture)
		{
			Location location = Location;
			int x = location.X;
			int y = location.Y;
			g.setColor(COLOR);
			if (Input)
			{
				int r = INPUT_RADIUS;
				g.drawRect(x - r, y - r, 2 * r, 2 * r);
			}
			else
			{
				int r = OUTPUT_RADIUS;
				g.drawOval(x - r, y - r, 2 * r, 2 * r);
			}
			g.fillOval(x - MINOR_RADIUS, y - MINOR_RADIUS, 2 * MINOR_RADIUS, 2 * MINOR_RADIUS);
		}
	}

}
