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

	using LogisimVersion = logisim.LogisimVersion;
	using Expression = logisim.analyze.model.Expression;
	using Expressions = logisim.analyze.model.Expressions;
	using ExpressionComputer = logisim.circuit.ExpressionComputer;
	using TextField = logisim.comp.TextField;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
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
	using AppPreferences = logisim.prefs.AppPreferences;
	using WireRepair = logisim.tools.WireRepair;
	using WireRepairData = logisim.tools.WireRepairData;
	using BitWidthConfigurator = logisim.tools.key.BitWidthConfigurator;
	using IntegerConfigurator = logisim.tools.key.IntegerConfigurator;
	using JoinedConfigurator = logisim.tools.key.JoinedConfigurator;
	using GraphicsUtil = logisim.util.GraphicsUtil;
	using Icons = logisim.util.Icons;
	using StringGetter = logisim.util.StringGetter;

	internal abstract class AbstractGate : InstanceFactory
	{
		private string[] iconNames = new string[3];
		private Icon[] icons = new Icon[3];
		private int bonusWidth = 0;
		private bool negateOutput = false;
		private bool isXor = false;
		private string rectLabel = "";
		private bool paintInputLines;

		protected internal AbstractGate(string name, StringGetter desc) : this(name, desc, false)
		{
		}

		protected internal AbstractGate(string name, StringGetter desc, bool isXor) : base(name, desc)
		{
			this.isXor = isXor;
			FacingAttribute = StdAttr.FACING;
			KeyConfigurator = JoinedConfigurator.create(new IntegerConfigurator(GateAttributes.ATTR_INPUTS, 2, GateAttributes.MAX_INPUTS, 0), new BitWidthConfigurator(StdAttr.WIDTH));
		}

		public override AttributeSet createAttributeSet()
		{
			return new GateAttributes(isXor);
		}

		public virtual object getDefaultAttributeValue<T1>(Attribute<T1> attr, LogisimVersion ver)
		{
			if (attr is NegateAttribute)
			{
				return false;
			}
			else
			{
				return base.getDefaultAttributeValue(attr, ver);
			}
		}

		public override Bounds getOffsetBounds(AttributeSet attrsBase)
		{
			GateAttributes attrs = (GateAttributes) attrsBase;
			Direction facing = attrs.facing;
			int size = ((int?) attrs.size.Value).Value;
			int inputs = attrs.inputs;
			if (inputs % 2 == 0)
			{
				inputs++;
			}
			int negated = attrs.negated;

			int width = size + bonusWidth + (negateOutput ? 10 : 0);
			if (negated != 0)
			{
				width += 10;
			}
			int height = Math.Max(10 * inputs, size);
			if (facing == Direction.South)
			{
				return Bounds.create(-height / 2, -width, height, width);
			}
			else if (facing == Direction.North)
			{
				return Bounds.create(-height / 2, 0, height, width);
			}
			else if (facing == Direction.West)
			{
				return Bounds.create(0, -height / 2, width, height);
			}
			else
			{
				return Bounds.create(-width, -height / 2, width, height);
			}
		}

		public override bool contains(Location loc, AttributeSet attrsBase)
		{
			GateAttributes attrs = (GateAttributes) attrsBase;
			if (base.contains(loc, attrs))
			{
				if (attrs.negated == 0)
				{
					return true;
				}
				else
				{
					Direction facing = attrs.facing;
					Bounds bds = getOffsetBounds(attrsBase);
					int delt;
					if (facing == Direction.North)
					{
						delt = loc.Y - (bds.Y + bds.Height);
					}
					else if (facing == Direction.South)
					{
						delt = loc.Y - bds.Y;
					}
					else if (facing == Direction.West)
					{
						delt = loc.X - (bds.X + bds.Height);
					}
					else
					{
						delt = loc.X - bds.X;
					}
					if (Math.Abs(delt) > 5)
					{
						return true;
					}
					else
					{
						int inputs = attrs.inputs;
						for (int i = 1; i <= inputs; i++)
						{
							Location offs = getInputOffset(attrs, i);
							if (loc.manhattanDistanceTo(offs) <= 5)
							{
								return true;
							}
						}
						return false;
					}
				}
			}
			else
			{
				return false;
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
			paintBase(painter);
			if (!painter.PrintView || painter.GateShape == AppPreferences.SHAPE_RECTANGULAR)
			{
				painter.drawPorts();
			}
		}

		private void paintBase(InstancePainter painter)
		{
			GateAttributes attrs = (GateAttributes) painter.AttributeSet;
			Direction facing = attrs.facing;
			int inputs = attrs.inputs;
			int negated = attrs.negated;

			object shape = painter.GateShape;
			Location loc = painter.Location;
			Bounds bds = painter.OffsetBounds;
			int width = bds.Width;
			int height = bds.Height;
			if (facing == Direction.North || facing == Direction.South)
			{
				int t = width;
				width = height;
				height = t;
			}
			if (negated != 0)
			{
				width -= 10;
			}

			Graphics g = painter.Graphics;
			Color baseColor = g.getColor();
			if (shape == AppPreferences.SHAPE_SHAPED && paintInputLines)
			{
				PainterShaped.paintInputLines(painter, this);
			}
			else if (negated != 0)
			{
				for (int i = 0; i < inputs; i++)
				{
					int negatedBit = (negated >> i) & 1;
					if (negatedBit == 1)
					{
						Location @in = getInputOffset(attrs, i);
						Location cen = @in.translate(facing, 5);
						painter.drawDongle(loc.X + cen.X, loc.Y + cen.Y);
					}
				}
			}

			g.setColor(baseColor);
			g.translate(loc.X, loc.Y);
			double rotate = 0.0;
			if (facing != Direction.East && g is Graphics2D)
			{
				rotate = -facing.toRadians();
				Graphics2D g2 = (Graphics2D) g;
				g2.rotate(rotate);
			}

			if (shape == AppPreferences.SHAPE_RECTANGULAR)
			{
				paintRectangular(painter, width, height);
			}
			else if (shape == AppPreferences.SHAPE_DIN40700)
			{
				paintDinShape(painter, width, height, inputs);
			}
			else
			{ // SHAPE_SHAPED
				if (negateOutput)
				{
					g.translate(-10, 0);
					paintShape(painter, width - 10, height);
					painter.drawDongle(5, 0);
					g.translate(10, 0);
				}
				else
				{
					paintShape(painter, width, height);
				}
			}

			if (rotate != 0.0)
			{
				((Graphics2D) g).rotate(-rotate);
			}
			g.translate(-loc.X, -loc.Y);

			painter.drawLabel();
		}

		protected internal virtual void setIconNames(string all)
		{
			setIconNames(all, all, all);
		}

		protected internal virtual void setIconNames(string shaped, string rect, string din)
		{
			iconNames[0] = shaped;
			iconNames[1] = rect;
			iconNames[2] = din;
		}

		private Icon getIcon(int type)
		{
			Icon ret = icons[type];
			if (ret != null)
			{
				return ret;
			}
			else
			{
				string iconName = iconNames[type];
				if (string.ReferenceEquals(iconName, null))
				{
					return null;
				}
				else
				{
					ret = Icons.getIcon(iconName);
					if (ret == null)
					{
						iconNames[type] = null;
					}
					else
					{
						icons[type] = ret;
					}
					return ret;
				}
			}
		}

		private Icon IconShaped
		{
			get
			{
				return getIcon(0);
			}
		}

		private Icon IconRectangular
		{
			get
			{
				return getIcon(1);
			}
		}

		private Icon IconDin40700
		{
			get
			{
				return getIcon(2);
			}
		}

		protected internal virtual bool PaintInputLines
		{
			set
			{
				paintInputLines = value;
			}
		}

		protected internal abstract void paintIconShaped(InstancePainter painter);

		protected internal virtual void paintIconRectangular(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			g.drawRect(1, 2, 16, 16);
			if (negateOutput)
			{
				g.drawOval(16, 8, 4, 4);
			}
			string label = getRectangularLabel(painter.AttributeSet);
			GraphicsUtil.drawCenteredText(g, label, 9, 8);
		}

		public override sealed void paintIcon(InstancePainter painter)
		{
			Graphics g = painter.Graphics;
			g.setColor(Color.black);
			if (painter.GateShape == AppPreferences.SHAPE_RECTANGULAR)
			{
				Icon iconRect = IconRectangular;
				if (iconRect != null)
				{
					iconRect.paintIcon(painter.Destination, g, 2, 2);
				}
				else
				{
					paintIconRectangular(painter);
				}
			}
			else if (painter.GateShape == AppPreferences.SHAPE_DIN40700)
			{
				Icon iconDin = IconDin40700;
				if (iconDin != null)
				{
					iconDin.paintIcon(painter.Destination, g, 2, 2);
				}
				else
				{
					paintIconRectangular(painter);
				}
			}
			else
			{
				Icon iconShaped = IconShaped;
				if (iconShaped != null)
				{
					iconShaped.paintIcon(painter.Destination, g, 2, 2);
				}
				else
				{
					paintIconShaped(painter);
				}
			}
		}

		protected internal virtual int AdditionalWidth
		{
			set
			{
				bonusWidth = value;
			}
		}

		protected internal virtual bool NegateOutput
		{
			set
			{
				negateOutput = value;
			}
		}

		protected internal virtual string RectangularLabel
		{
			set
			{
				rectLabel = value;
			}
		}

		protected internal virtual string getRectangularLabel(AttributeSet attrs)
		{
			return rectLabel;
		}

		//
		// protected methods intended to be overridden
		//
		protected internal abstract Value Identity {get;}

		protected internal abstract void paintShape(InstancePainter painter, int width, int height);

		protected internal virtual void paintRectangular(InstancePainter painter, int width, int height)
		{
			int don = negateOutput ? 10 : 0;
			AttributeSet attrs = painter.AttributeSet;
			painter.drawRectangle(-width, -height / 2, width - don, height, getRectangularLabel(attrs));
			if (negateOutput)
			{
				painter.drawDongle(-5, 0);
			}
		}

		protected internal abstract void paintDinShape(InstancePainter painter, int width, int height, int inputs);

		protected internal abstract Value computeOutput(Value[] inputs, int numInputs, InstanceState state);

		protected internal abstract Expression computeExpression(Expression[] inputs, int numInputs);

		protected internal virtual bool shouldRepairWire(Instance instance, WireRepairData data)
		{
			return false;
		}

		//
		// methods for instances
		//
		protected internal override void configureNewInstance(Instance instance)
		{
			instance.addAttributeListener();
			computePorts(instance);
			computeLabel(instance);
		}

		protected internal override void instanceAttributeChanged<T1>(Instance instance, Attribute<T1> attr)
		{
			if (attr == GateAttributes.ATTR_SIZE || attr == StdAttr.FACING)
			{
				instance.recomputeBounds();
				computePorts(instance);
				computeLabel(instance);
			}
			else if (attr == GateAttributes.ATTR_INPUTS || attr is NegateAttribute)
			{
				instance.recomputeBounds();
				computePorts(instance);
			}
			else if (attr == GateAttributes.ATTR_XOR)
			{
				instance.fireInvalidated();
			}
		}

		private void computeLabel(Instance instance)
		{
			GateAttributes attrs = (GateAttributes) instance.AttributeSet;
			Direction facing = attrs.facing;
			int baseWidth = ((int?) attrs.size.Value).Value;

			int axis = baseWidth / 2 + (negateOutput ? 10 : 0);
			int perp = 0;
			if (AppPreferences.GATE_SHAPE.get().Equals(AppPreferences.SHAPE_RECTANGULAR))
			{
				perp += 6;
			}
			Location loc = instance.Location;
			int cx;
			int cy;
			if (facing == Direction.North)
			{
				cx = loc.X + perp;
				cy = loc.Y + axis;
			}
			else if (facing == Direction.South)
			{
				cx = loc.X - perp;
				cy = loc.Y - axis;
			}
			else if (facing == Direction.West)
			{
				cx = loc.X + axis;
				cy = loc.Y - perp;
			}
			else
			{
				cx = loc.X - axis;
				cy = loc.Y + perp;
			}
			instance.setTextField(StdAttr.LABEL, StdAttr.LABEL_FONT, cx, cy, TextField.H_CENTER, TextField.V_CENTER);
		}

		internal virtual void computePorts(Instance instance)
		{
			GateAttributes attrs = (GateAttributes) instance.AttributeSet;
			int inputs = attrs.inputs;

			Port[] ports = new Port[inputs + 1];
			ports[0] = new Port(0, 0, Port.OUTPUT, StdAttr.WIDTH);
			for (int i = 0; i < inputs; i++)
			{
				Location offs = getInputOffset(attrs, i);
				ports[i + 1] = new Port(offs.X, offs.Y, Port.INPUT, StdAttr.WIDTH);
			}
			instance.setPorts(ports);
		}

		public override void propagate(InstanceState state)
		{
			GateAttributes attrs = (GateAttributes) state.AttributeSet;
			int inputCount = attrs.inputs;
			int negated = attrs.negated;
			AttributeSet opts = state.Project.Options.AttributeSet;
			bool errorIfUndefined = opts.getValue(Options.ATTR_GATE_UNDEFINED).Equals(Options.GATE_UNDEFINED_ERROR);

			Value[] inputs = new Value[inputCount];
			int numInputs = 0;
			bool error = false;
			for (int i = 1; i <= inputCount; i++)
			{
				if (state.isPortConnected(i))
				{
					int negatedBit = (negated >> (i - 1)) & 1;
					if (negatedBit == 1)
					{
						inputs[numInputs] = state.getPort(i).not();
					}
					else
					{
						inputs[numInputs] = state.getPort(i);
					}
					numInputs++;
				}
				else
				{
					if (errorIfUndefined)
					{
						error = true;
					}
				}
			}
			Value @out = null;
			if (numInputs == 0 || error)
			{
				@out = Value.createError(attrs.width);
			}
			else
			{
				@out = computeOutput(inputs, numInputs, state);
				@out = pullOutput(@out, attrs.@out);
			}
			state.setPort(0, @out, GateAttributes.DELAY);
		}

		internal static Value pullOutput(Value value, object outType)
		{
			if (outType == GateAttributes.OUTPUT_01)
			{
				return value;
			}
			else
			{
				Value[] v = value.All;
				if (outType == GateAttributes.OUTPUT_0Z)
				{
					for (int i = 0; i < v.Length; i++)
					{
						if (v[i] == Value.TRUE)
						{
							v[i] = Value.UNKNOWN;
						}
					}
				}
				else if (outType == GateAttributes.OUTPUT_Z1)
				{
					for (int i = 0; i < v.Length; i++)
					{
						if (v[i] == Value.FALSE)
						{
							v[i] = Value.UNKNOWN;
						}
					}
				}
				return Value.create(v);
			}
		}

		protected internal override object getInstanceFeature(in Instance instance, object key)
		{
			if (key == typeof(WireRepair))
			{
				return new WireRepairAnonymousInnerClass(this, instance);
			}
			if (key == typeof(ExpressionComputer))
			{
				return new ExpressionComputerAnonymousInnerClass(this, instance);
			}
			return base.getInstanceFeature(instance, key);
		}

		private class WireRepairAnonymousInnerClass : WireRepair
		{
			private readonly AbstractGate outerInstance;

			private Instance instance;

			public WireRepairAnonymousInnerClass(AbstractGate outerInstance, Instance instance)
			{
				this.outerInstance = outerInstance;
				this.instance = instance;
			}

			public bool shouldRepairWire(WireRepairData data)
			{
				return outerInstance.shouldRepairWire(instance, data);
			}
		}

		private class ExpressionComputerAnonymousInnerClass : ExpressionComputer
		{
			private readonly AbstractGate outerInstance;

			private Instance instance;

			public ExpressionComputerAnonymousInnerClass(AbstractGate outerInstance, Instance instance)
			{
				this.outerInstance = outerInstance;
				this.instance = instance;
			}

			public void computeExpression(IDictionary<Location, Expression> expressionMap)
			{
				GateAttributes attrs = (GateAttributes) instance.AttributeSet;
				int inputCount = attrs.inputs;
				int negated = attrs.negated;

				Expression[] inputs = new Expression[inputCount];
				int numInputs = 0;
				for (int i = 1; i <= inputCount; i++)
				{
					Expression e = expressionMap[instance.getPortLocation(i)];
					if (e != null)
					{
						int negatedBit = (negated >> (i - 1)) & 1;
						if (negatedBit == 1)
						{
							e = Expressions.not(e);
						}
						inputs[numInputs] = e;
						++numInputs;
					}
				}
				if (numInputs > 0)
				{
					Expression @out = outerInstance.computeExpression(inputs, numInputs);
					expressionMap[instance.getPortLocation(0)] = @out;
				}
			}
		}

		internal virtual Location getInputOffset(GateAttributes attrs, int index)
		{
			int inputs = attrs.inputs;
			Direction facing = attrs.facing;
			int size = ((int?) attrs.size.Value).Value;
			int axisLength = size + bonusWidth + (negateOutput ? 10 : 0);
			int negated = attrs.negated;

			int skipStart;
			int skipDist;
			int skipLowerEven = 10;
			if (inputs <= 3)
			{
				if (size < 40)
				{
					skipStart = -5;
					skipDist = 10;
					skipLowerEven = 10;
				}
				else if (size < 60 || inputs <= 2)
				{
					skipStart = -10;
					skipDist = 20;
					skipLowerEven = 20;
				}
				else
				{
					skipStart = -15;
					skipDist = 30;
					skipLowerEven = 30;
				}
			}
			else if (inputs == 4 && size >= 60)
			{
				skipStart = -5;
				skipDist = 20;
				skipLowerEven = 0;
			}
			else
			{
				skipStart = -5;
				skipDist = 10;
				skipLowerEven = 10;
			}

			int dy;
			if ((inputs & 1) == 1)
			{
				dy = skipStart * (inputs - 1) + skipDist * index;
			}
			else
			{
				dy = skipStart * inputs + skipDist * index;
				if (index >= inputs / 2)
				{
					dy += skipLowerEven;
				}
			}

			int dx = axisLength;
			int negatedBit = (negated >> index) & 1;
			if (negatedBit == 1)
			{
				dx += 10;
			}

			if (facing == Direction.North)
			{
				return new Location(dy, dx);
			}
			else if (facing == Direction.South)
			{
				return new Location(dy, -dx);
			}
			else if (facing == Direction.West)
			{
				return new Location(dx, dy);
			}
			else
			{
				return new Location(-dx, dy);
			}
		}
	}

}
