// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using Expression = logisim.analyze.model.Expression;
	using Expressions = logisim.analyze.model.Expressions;
	using ExpressionComputer = logisim.circuit.ExpressionComputer;
	using TextField = logisim.comp.TextField;
	using logisim.data;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Attributes = logisim.data.Attributes;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using AppPreferences = logisim.prefs.AppPreferences;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using Icons = logisim.util.Icons;
    using LogisimPlus.Java;

    internal class NotGate : InstanceFactory
	{
		public static readonly AttributeOption SIZE_NARROW = new AttributeOption(Convert.ToInt32(20), Strings.getter("gateSizeNarrowOpt"));
		public static readonly AttributeOption SIZE_WIDE = new AttributeOption(Convert.ToInt32(30), Strings.getter("gateSizeWideOpt"));
		public static readonly Attribute ATTR_SIZE = Attributes.forOption("size", Strings.getter("gateSizeAttr"), new AttributeOption[] {SIZE_NARROW, SIZE_WIDE});

		private const string RECT_LABEL = "1";
		private static readonly Icon toolIcon = Icons.getIcon("notGate.gif");
		private static readonly Icon toolIconRect = Icons.getIcon("notGateRect.gif");
		private static readonly Icon toolIconDin = Icons.getIcon("dinNotGate.gif");

		public static InstanceFactory FACTORY = new NotGate();

		private NotGate() : base("NOT Gate", Strings.getter("notGateComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, StdAttr.Width, ATTR_SIZE, GateAttributes.ATTR_OUTPUT, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {Direction.East, BitWidth.ONE, SIZE_WIDE, GateAttributes.OUTPUT_01, "", StdAttr.DEFAULT_LABEL_FONT});
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			object value = attrs.getValue(ATTR_SIZE);
			if (value == SIZE_NARROW)
			{
				Direction facing = (Direction)attrs.getValue(StdAttr.FACING);
				if (facing == Direction.South)
				{
					return Bounds.create(-9, -20, 18, 20);
				}
				if (facing == Direction.North)
				{
					return Bounds.create(-9, 0, 18, 20);
				}
				if (facing == Direction.West)
				{
					return Bounds.create(0, -9, 20, 18);
				}
				return Bounds.create(-20, -9, 20, 18);
			}
			else
			{
				Direction facing = (Direction)attrs.getValue(StdAttr.FACING);
				if (facing == Direction.South)
				{
					return Bounds.create(-9, -30, 18, 30);
				}
				if (facing == Direction.North)
				{
					return Bounds.create(-9, 0, 18, 30);
				}
				if (facing == Direction.West)
				{
					return Bounds.create(0, -9, 30, 18);
				}
				return Bounds.create(-30, -9, 30, 18);
			}
		}

		public override void propagate(InstanceState state)
		{
			Value @in = state.getPort(1);
			Value @out = @in.not();
			@out = Buffer.repair(state, @out);
			state.setPort(0, @out, GateAttributes.DELAY);
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			configurePorts(instance);
			instance.addAttributeListener();
			string gateShape = AppPreferences.GATE_SHAPE.get();
			configureLabel(instance, gateShape.Equals(AppPreferences.SHAPE_RECTANGULAR), null);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == ATTR_SIZE || attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
				configurePorts(instance);
				string gateShape = AppPreferences.GATE_SHAPE.get();
				configureLabel(instance, gateShape.Equals(AppPreferences.SHAPE_RECTANGULAR), null);
			}
		}

		private void configurePorts(Instance instance)
		{
			object size = instance.getAttributeValue(ATTR_SIZE);
			Direction facing = (Direction)instance.getAttributeValue(StdAttr.FACING);
			int dx = size == SIZE_NARROW ? -20 : -30;

			Port[] ports = new Port[2];
			ports[0] = new Port(0, 0, Port.OUTPUT, StdAttr.Width);
			Location @out = (new Location(0, 0)).translate(facing, dx);
			ports[1] = new Port(@out.X, @out.Y, Port.INPUT, StdAttr.Width);
			instance.Ports = ports;
		}

		protected internal override object getInstanceFeature(in Instance instance, object key)
		{
			if (key == typeof(ExpressionComputer))
			{
				return new ExpressionComputerAnonymousInnerClass(this, instance);
			}
			return base.getInstanceFeature(instance, key);
		}

		private class ExpressionComputerAnonymousInnerClass : ExpressionComputer
		{
			private readonly NotGate outerInstance;

			private Instance instance;

			public ExpressionComputerAnonymousInnerClass(NotGate outerInstance, Instance instance)
			{
				this.outerInstance = outerInstance;
				this.instance = instance;
			}

			public void computeExpression(Dictionary<Location, Expression> expressionMap)
			{
				Expression e = expressionMap[instance.getPortLocation(1)];
				if (e != null)
				{
					expressionMap[instance.getPortLocation(0)] = Expressions.not(e);
				}
			}
		}

		//
		// painting methods
		//
		public override void paintIcon(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			g.setColor(Color.Black);
			if (painter.GateShape == AppPreferences.SHAPE_RECTANGULAR)
			{
				if (toolIconRect != null)
				{
					toolIconRect.paintIcon(painter.Destination, g, 2, 2);
				}
				else
				{
					g.drawRect(0, 2, 16, 16);
					JGraphicsUtil.drawCenteredText(g, RECT_LABEL, 8, 8);
					g.drawOval(16, 8, 4, 4);
				}
			}
			else if (painter.GateShape == AppPreferences.SHAPE_DIN40700)
			{
				if (toolIconDin != null)
				{
					toolIconDin.paintIcon(painter.Destination, g, 2, 2);
				}
				else
				{
					g.drawRect(0, 2, 16, 16);
					JGraphicsUtil.drawCenteredText(g, RECT_LABEL, 8, 8);
					g.drawOval(16, 8, 4, 4);
				}
			}
			else
			{
				if (toolIcon != null)
				{
					toolIcon.paintIcon(painter.Destination, g, 2, 2);
				}
				else
				{
					int[] xp = new int[4];
					int[] yp = new int[4];
					xp[0] = 15;
					yp[0] = 10;
					xp[1] = 1;
					yp[1] = 3;
					xp[2] = 1;
					yp[2] = 17;
					xp[3] = 15;
					yp[3] = 10;
					g.drawPolyline(xp, yp, 4);
					g.drawOval(15, 8, 4, 4);
				}
			}
		}

		public override void paintGhost(InstancePainter painter)
		{
			paintBase(painter);
		}

		public override void paintInstance(InstancePainter painter)
		{
			painter.Graphics.setColor(Color.Black);
			paintBase(painter);
			painter.drawPorts();
			painter.drawLabel();
		}

		private void paintBase(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			Direction facing = (Direction)painter.getAttributeValue(StdAttr.FACING);
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			g.translate(x, y);
			double rotate = 0.0;
			if (facing != null && facing != Direction.East)
			{
				rotate = -facing.toRadians();
				g.rotate(rotate);
			}

			object shape = painter.GateShape;
			if (shape == AppPreferences.SHAPE_RECTANGULAR)
			{
				paintRectangularBase(g, painter);
			}
			else if (shape == AppPreferences.SHAPE_DIN40700)
			{
				int width = painter.getAttributeValue(ATTR_SIZE) == SIZE_NARROW ? 20 : 30;
				PainterDin.paintAnd(painter, width, 18, true);
			}
			else
			{
				PainterShaped.paintNot(painter);
			}

			if (rotate != 0.0)
			{
				g.rotate(-rotate);
			}
			g.translate(-x, -y);
		}

		private void paintRectangularBase(JGraphics g, InstancePainter painter)
		{
			JGraphicsUtil.switchToWidth(g, 2);
			if (painter.getAttributeValue(ATTR_SIZE) == SIZE_NARROW)
			{
				g.drawRect(-20, -9, 14, 18);
				JGraphicsUtil.drawCenteredText(g, RECT_LABEL, -13, 0);
				g.drawOval(-6, -3, 6, 6);
			}
			else
			{
				g.drawRect(-30, -9, 20, 18);
				JGraphicsUtil.drawCenteredText(g, RECT_LABEL, -20, 0);
				g.drawOval(-10, -5, 9, 9);
			}
			JGraphicsUtil.switchToWidth(g, 1);
		}

		internal static void configureLabel(Instance instance, bool isRectangular, Location control)
		{
			object facing = instance.getAttributeValue(StdAttr.FACING);
			Bounds bds = instance.Bounds;
			int x;
			int y;
			int halign;
			if (facing == Direction.North || facing == Direction.South)
			{
				x = bds.X + bds.Width / 2 + 2;
				y = bds.Y - 2;
				halign = TextField.H_LEFT;
			}
			else
			{ // west or east
				y = isRectangular ? bds.Y - 2 : bds.Y;
				if (control != null && control.Y == bds.Y)
				{
					// the control line will get in the way
					x = control.X + 2;
					halign = TextField.H_LEFT;
				}
				else
				{
					x = bds.X + bds.Width / 2;
					halign = TextField.H_CENTER;
				}
			}
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, x, y, halign, TextField.V_BASELINE);
		}
	}

}
