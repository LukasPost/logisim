// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.std.gates
{

	using Expression = logisim.analyze.model.Expression;
	using ExpressionComputer = logisim.circuit.ExpressionComputer;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using BitWidth = logisim.data.BitWidth;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Options = logisim.file.Options;
	using Instance = logisim.instance.Instance;
	using InstanceFactory = logisim.instance.InstanceFactory;
	using InstancePainter = logisim.instance.InstancePainter;
	using InstanceState = logisim.instance.InstanceState;
	using Port = logisim.instance.Port;
	using StdAttr = logisim.instance.StdAttr;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using JGraphicsUtil = logisim.util.JGraphicsUtil;
	using Icons = logisim.util.Icons;
    using LogisimPlus.Java;

    internal class Buffer : InstanceFactory
	{
		public static InstanceFactory FACTORY = new Buffer();

		private Buffer() : base("Buffer", Strings.getter("bufferComponent"))
		{
			setAttributes(new Attribute[] {StdAttr.FACING, StdAttr.Width, GateAttributes.ATTR_OUTPUT, StdAttr.LABEL, StdAttr.LABEL_FONT}, new object[] {Direction.East, BitWidth.ONE, GateAttributes.OUTPUT_01, "", StdAttr.DEFAULT_LABEL_FONT});
			Icon = Icons.getIcon("bufferGate.gif");
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = new BitWidthConfigurator(StdAttr.Width);
			setPorts(new Port[]
			{
				new Port(0, 0, Port.OUTPUT, StdAttr.Width),
				new Port(0, -20, Port.INPUT, StdAttr.Width)
			});
		}

		public override Bounds getOffsetBounds(AttributeSet attrs)
		{
			Direction facing = attrs.getValue(StdAttr.FACING);
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

		public override void propagate(InstanceState state)
		{
			Value @in = state.getPort(1);
			@in = Buffer.repair(state, @in);
			state.setPort(0, @in, GateAttributes.DELAY);
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			configurePorts(instance);
			instance.addAttributeListener();
			NotGate.configureLabel(instance, false, null);
		}

		protected internal override void instanceAttributeChanged(Instance instance, Attribute attr)
		{
			if (attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
				configurePorts(instance);
				NotGate.configureLabel(instance, false, null);
			}
		}

		private void configurePorts(Instance instance)
		{
			Direction facing = instance.getAttributeValue(StdAttr.FACING);

			Port[] ports = new Port[2];
			ports[0] = new Port(0, 0, Port.OUTPUT, StdAttr.Width);
			Location @out = (new Location(0, 0)).translate(facing, -20);
			ports[1] = new Port(@out.X, @out.Y, Port.INPUT, StdAttr.Width);
			instance.Ports = ports;
		}

		public override object getInstanceFeature(in Instance instance, object key)
		{
			if (key == typeof(ExpressionComputer))
			{
				return new ExpressionComputerAnonymousInnerClass(this, instance);
			}
			return base.getInstanceFeature(instance, key);
		}

		private class ExpressionComputerAnonymousInnerClass : ExpressionComputer
		{
			private readonly Buffer outerInstance;

			private Instance instance;

			public ExpressionComputerAnonymousInnerClass(Buffer outerInstance, Instance instance)
			{
				this.outerInstance = outerInstance;
				this.instance = instance;
			}

			public void computeExpression(Dictionary<Location, Expression> expressionMap)
			{
				Expression e = expressionMap[instance.getPortLocation(1)];
				if (e != null)
				{
					expressionMap[instance.getPortLocation(0)] = e;
				}
			}
		}

		//
		// painting methods
		//
		public override void paintGhost(InstancePainter painter)
		{
			paintBase(painter);
		}

		public override void paintInstance(InstancePainter painter)
		{
			JGraphics g = painter.Graphics;
			g.setColor(Color.Black);
			paintBase(painter);
			painter.drawPorts();
			painter.drawLabel();
		}

		private void paintBase(InstancePainter painter)
		{
			Direction facing = (Direction)painter.getAttributeValue(StdAttr.FACING);
			Location loc = painter.Location;
			int x = loc.X;
			int y = loc.Y;
			JGraphics g = painter.Graphics;
			g.translate(x, y);
			double rotate = 0.0;
			if (facing != Direction.East)
			{
				rotate = -facing.toRadians();
				g.rotate(rotate);
			}

			JGraphicsUtil.switchToWidth(g, 2);
			int[] xp = new int[4];
			int[] yp = new int[4];
			xp[0] = 0;
			yp[0] = 0;
			xp[1] = -19;
			yp[1] = -7;
			xp[2] = -19;
			yp[2] = 7;
			xp[3] = 0;
			yp[3] = 0;
			g.drawPolyline(xp, yp, 4);

			if (rotate != 0.0)
			{
				g.rotate(-rotate);
			}
			g.translate(-x, -y);
		}

		//
		// static methods - shared with other classes
		//
		internal static Value repair(InstanceState state, Value v)
		{
			AttributeSet opts = state.Project.Options.AttributeSet;
			object onUndefined = opts.getValue(Options.ATTR_GATE_UNDEFINED);
			bool errorIfUndefined = onUndefined.Equals(Options.GATE_UNDEFINED_ERROR);
			Value repaired;
			if (errorIfUndefined)
			{
				int vw = v.Width;
				BitWidth w = state.getAttributeValue(StdAttr.Width);
				int ww = w.Width;
				if (vw == ww && v.FullyDefined)
				{
					return v;
				}
				Value[] vs = new Value[w.Width];
				for (int i = 0; i < vs.Length; i++)
				{
					Value ini = i < vw ? v.get(i) : Value.ERROR;
					vs[i] = ini.FullyDefined ? ini : Value.ERROR;
				}
				repaired = Value.create(vs);
			}
			else
			{
				repaired = v;
			}

			object outType = state.getAttributeValue(GateAttributes.ATTR_OUTPUT);
			return AbstractGate.pullOutput(repaired, outType);
		}
	}

}
