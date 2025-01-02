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

	using AnalyzerModel = logisim.analyze.model.AnalyzerModel;
	using Expression = logisim.analyze.model.Expression;
	using VariableList = logisim.analyze.model.VariableList;
	using Circuit = logisim.circuit.Circuit;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using Wire = logisim.circuit.Wire;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using StdAttr = logisim.instance.StdAttr;
	using Constant = logisim.std.wiring.Constant;
	using Pin = logisim.std.wiring.Pin;

	public class CircuitBuilder
	{
		private CircuitBuilder()
		{
		}

		public static CircuitMutation build(Circuit destCirc, AnalyzerModel model, bool twoInputs, bool useNands)
		{
			CircuitMutation result = new CircuitMutation(destCirc);
			result.clear();

			Layout[] layouts = new Layout[model.Outputs.size()];
			int maxWidth = 0;
			for (int i = 0; i < layouts.Length; i++)
			{
				string output = model.Outputs.get(i);
				Expression expr = model.OutputExpressions.getExpression(output);
				CircuitDetermination det = CircuitDetermination.create(expr);
				if (det != null)
				{
					if (twoInputs)
					{
						det.convertToTwoInputs();
					}
					if (useNands)
					{
						det.convertToNands();
					}
					det.repair();
					layouts[i] = layoutGates(det);
					maxWidth = Math.Max(maxWidth, layouts[i].width);
				}
				else
				{
					layouts[i] = null;
				}
			}

			InputData inputData = computeInputData(model);
			int x = inputData.StartX;
			int y = 10;
			int outputX = x + maxWidth + 20;
			for (int i = 0; i < layouts.Length; i++)
			{
				string outputName = model.Outputs.get(i);
				Layout layout = layouts[i];
				Location output;
				int height;
				if (layout == null)
				{
					output = new Location(outputX, y + 20);
					height = 40;
				}
				else
				{
					int dy = 0;
					if (layout.outputY < 20)
					{
						dy = 20 - layout.outputY;
					}
					height = Math.Max(dy + layout.height, 40);
					output = new Location(outputX, y + dy + layout.outputY);
					placeComponents(result, layouts[i], x, y + dy, inputData, output);
				}
				placeOutput(result, output, outputName);
				y += height + 10;
			}
			placeInputs(result, inputData);
			return result;
		}

		//
		// layoutGates
		//
		private static Layout layoutGates(CircuitDetermination det)
		{
			return layoutGatesSub(det);
		}

		private class Layout
		{
			// initialized by parent
			internal int y; // top edge relative to parent's top edge
			// (or edge corresponding to input)

			// initialized by self
			internal int width;
			internal int height;
			internal ComponentFactory factory;
			internal AttributeSet attrs;
			internal int outputY; // where output is relative to my top edge
			internal int subX; // where right edge of sublayouts should be relative to my left edge
			internal Layout[] subLayouts;
			internal string inputName; // for references directly to inputs

			internal Layout(int width, int height, int outputY, ComponentFactory factory, AttributeSet attrs, Layout[] subLayouts, int subX)
			{
				this.width = width;
				this.height = roundUp(height);
				this.outputY = outputY;
				this.factory = factory;
				this.attrs = attrs;
				this.subLayouts = subLayouts;
				this.subX = subX;
				this.inputName = null;
			}

			internal Layout(string inputName) : this(0, 0, 0, null, null, null, 0)
			{
				this.inputName = inputName;
			}
		}

		private static Layout layoutGatesSub(CircuitDetermination det)
		{
			if (det is CircuitDetermination.Input)
			{
				CircuitDetermination.Input input = (CircuitDetermination.Input) det;
				return new Layout(input.Name);
			}
			else if (det is CircuitDetermination.Value)
			{
				CircuitDetermination.Value value = (CircuitDetermination.Value) det;
				ComponentFactory factory = Constant.FACTORY;
				AttributeSet attrs = factory.createAttributeSet();
				attrs.setValue(Constant.ATTR_VALUE, Convert.ToInt32(value.Value));
				Bounds bds = factory.getOffsetBounds(attrs);
				return new Layout(bds.Width, bds.Height, -bds.Y, factory, attrs, new Layout[0], 0);
			}

			// We know det is a Gate. Determine sublayouts.
			CircuitDetermination.Gate gate = (CircuitDetermination.Gate) det;
			ComponentFactory factory = gate.Factory;
			List<CircuitDetermination> inputs = gate.Inputs;

			// Handle a NOT implemented with a NAND as a special case
			if (gate.NandNot)
			{
				CircuitDetermination subDet = inputs[0];
				if (!(subDet is CircuitDetermination.Input))
				{
					Layout[] sub = new Layout[1];
					sub[0] = layoutGatesSub(subDet);
					sub[0].y = 0;

					AttributeSet attrs = factory.createAttributeSet();
					attrs.setValue(GateAttributes.ATTR_SIZE, GateAttributes.SIZE_NARROW);
					attrs.setValue(GateAttributes.ATTR_INPUTS, Convert.ToInt32(2));

					// determine layout's width
					Bounds bds = factory.getOffsetBounds(attrs);
					int betweenWidth = 40;
					if (sub[0].width == 0)
					{
						betweenWidth = 0;
					}
					int width = sub[0].width + betweenWidth + bds.Width;

					// determine outputY and layout's height.
					int outputY = sub[0].y + sub[0].outputY;
					int height = sub[0].height;
					int minOutputY = roundUp(-bds.Y);
					if (minOutputY > outputY)
					{
						// we have to shift everything down because otherwise
						// the component will peek over the rectangle's top.
						int dy = minOutputY - outputY;
						sub[0].y += dy;
						height += dy;
						outputY += dy;
					}
					int minHeight = outputY + bds.Y + bds.Height;
					if (minHeight > height)
					{
						height = minHeight;
					}

					// ok; create and return the layout.
					return new Layout(width, height, outputY, factory, attrs, sub, sub[0].width);
				}
			}

			Layout[] sub = new Layout[inputs.Count];
			int subWidth = 0; // maximum width of sublayouts
			int subHeight = 0; // total height of sublayouts
			for (int i = 0; i < sub.Length; i++)
			{
				sub[i] = layoutGatesSub(inputs[i]);
				if (sub.Length % 2 == 0 && i == (sub.Length + 1) / 2 && sub[i - 1].height + sub[i].height == 0)
				{
					// if there are an even number of inputs, then there is a
					// 20-tall gap between the middle two inputs. Ensure the two
					// middle inputs are at least 20 pixels apart.
					subHeight += 10;
				}
				sub[i].y = subHeight;
				subWidth = Math.Max(subWidth, sub[i].width);
				subHeight += sub[i].height + 10;
			}
			subHeight -= 10;

			AttributeSet attrs = factory.createAttributeSet();
			if (factory == NotGate.FACTORY)
			{
				attrs.setValue(NotGate.ATTR_SIZE, NotGate.SIZE_NARROW);
			}
			else
			{
				attrs.setValue(GateAttributes.ATTR_SIZE, GateAttributes.SIZE_NARROW);

				int ins = sub.Length;
				attrs.setValue(GateAttributes.ATTR_INPUTS, Convert.ToInt32(ins));
			}

			// determine layout's width
			Bounds bds = factory.getOffsetBounds(attrs);
			int betweenWidth = 40 + 10 * (sub.Length / 2 - 1);
			if (sub.Length == 1)
			{
				betweenWidth = 20;
			}
			if (subWidth == 0)
			{
				betweenWidth = 0;
			}
			int width = subWidth + betweenWidth + bds.Width;

			// determine outputY and layout's height.
			int outputY;
			if (sub.Length % 2 == 1)
			{ // odd number - match the middle input
				int i = (sub.Length - 1) / 2;
				outputY = sub[i].y + sub[i].outputY;
			}
			else
			{ // even number - halfway between middle two inputs
				int i0 = (sub.Length / 2) - 1;
				int i1 = (sub.Length / 2);
				int o0 = sub[i0].y + sub[i0].outputY;
				int o1 = sub[i1].y + sub[i1].outputY;
				outputY = roundDown((o0 + o1) / 2);
			}
			int height = subHeight;
			int minOutputY = roundUp(-bds.Y);
			if (minOutputY > outputY)
			{
				// we have to shift everything down because otherwise
				// the component will peek over the rectangle's top.
				int dy = minOutputY - outputY;
				for (int i = 0; i < sub.Length; i++)
				{
					sub[i].y += dy;
				}
				height += dy;
				outputY += dy;
			}
			int minHeight = outputY + bds.Y + bds.Height;
			if (minHeight > height)
			{
				height = minHeight;
			}

			// ok; create and return the layout.
			return new Layout(width, height, outputY, factory, attrs, sub, subWidth);
		}

		private static int roundDown(int value)
		{
			return value / 10 * 10;
		}

		private static int roundUp(int value)
		{
			return (value + 9) / 10 * 10;
		}

		//
		// computeInputData
		//
		private static InputData computeInputData(AnalyzerModel model)
		{
			InputData ret = new InputData();
			VariableList inputs = model.Inputs;
			int spineX = 60;
			ret.names = new string[inputs.size()];
			for (int i = 0; i < inputs.size(); i++)
			{
				string name = inputs.get(i);
				ret.names[i] = name;
				ret.inputs[name] = new SingleInput(spineX);
				spineX += 20;
			}
			ret.startX = spineX;
			return ret;
		}

		private class InputData
		{
			internal int startX;
			internal string[] names;
			internal Dictionary<string, SingleInput> inputs = new Dictionary<string, SingleInput>();

			internal InputData()
			{
			}

			internal virtual int StartX
			{
				get
				{
					return startX;
				}
			}

			internal virtual int getSpineX(string input)
			{
				SingleInput data = inputs[input];
				return data.spineX;
			}

			internal virtual void registerConnection(string input, Location loc)
			{
				SingleInput data = inputs[input];
				data.ys.Add(loc);
			}
		}

		private class SingleInput
		{
			internal int spineX;
			internal List<Location> ys = new List<Location>();

			internal SingleInput(int spineX)
			{
				this.spineX = spineX;
			}
		}

		//
		// placeComponents
		//
		/// <param name="circuit">   the circuit where to place the components. </param>
		/// <param name="layout">    the layout specifying the gates to place there. </param>
		/// <param name="x">         the left edge of where the layout should be placed. </param>
		/// <param name="y">         the top edge of where the layout should be placed. </param>
		/// <param name="inputData"> information about how to reach inputs. </param>
		/// <param name="output">    a point to which the output should be connected. </param>
		private static void placeComponents(CircuitMutation result, Layout layout, int x, int y, InputData inputData, Location output)
		{
			if (!string.ReferenceEquals(layout.inputName, null))
			{
				int inputX = inputData.getSpineX(layout.inputName);
				Location input = new Location(inputX, output.Y);
				inputData.registerConnection(layout.inputName, input);
				result.add(Wire.create(input, output));
				return;
			}

			Location compOutput = new Location(x + layout.width, output.Y);
			Component parent = layout.factory.createComponent(compOutput, layout.attrs);
			result.add(parent);
			if (!compOutput.Equals(output))
			{
				result.add(Wire.create(compOutput, output));
			}

			// handle a NOT gate pattern implemented with NAND as a special case
			if (layout.factory == NandGate.FACTORY && layout.subLayouts.Length == 1 && string.ReferenceEquals(layout.subLayouts[0].inputName, null))
			{
				Layout sub = layout.subLayouts[0];

				Location input0 = parent.getEnd(1).Location;
				Location input1 = parent.getEnd(2).Location;

				int midX = input0.X - 20;
				Location subOutput = new Location(midX, output.Y);
				Location midInput0 = new Location(midX, input0.Y);
				Location midInput1 = new Location(midX, input1.Y);
				result.add(Wire.create(subOutput, midInput0));
				result.add(Wire.create(midInput0, input0));
				result.add(Wire.create(subOutput, midInput1));
				result.add(Wire.create(midInput1, input1));

				int subX = x + layout.subX - sub.width;
				placeComponents(result, sub, subX, y + sub.y, inputData, subOutput);
				return;
			}

			if (layout.subLayouts.Length == parent.Ends.Count - 2)
			{
				int index = layout.subLayouts.Length / 2 + 1;
				object factory = parent.Factory;
				if (factory is AbstractGate)
				{
					Value val = ((AbstractGate) factory).Identity;
					int? valInt = Convert.ToInt32(val.toIntValue());
					Location loc = parent.getEnd(index).Location;
					AttributeSet attrs = Constant.FACTORY.createAttributeSet();
					attrs.setValue(Constant.ATTR_VALUE, valInt);
					result.add(Constant.FACTORY.createComponent(loc, attrs));
				}
			}

			for (int i = 0; i < layout.subLayouts.Length; i++)
			{
				Layout sub = layout.subLayouts[i];

				int inputIndex = i + 1;
				Location subDest = parent.getEnd(inputIndex).Location;

				int subOutputY = y + sub.y + sub.outputY;
				if (!string.ReferenceEquals(sub.inputName, null))
				{
					int destY = subDest.Y;
					if (i == 0 && destY < subOutputY || i == layout.subLayouts.Length - 1 && destY > subOutputY)
					{
						subOutputY = destY;
					}
				}

				Location subOutput;
				int numSubs = layout.subLayouts.Length;
				if (subOutputY == subDest.Y)
				{
					subOutput = subDest;
				}
				else
				{
					int back;
					if (i < numSubs / 2)
					{
						if (subOutputY < subDest.Y)
						{ // bending upward
							back = i;
						}
						else
						{
							back = ((numSubs - 1) / 2) - i;
						}
					}
					else
					{
						if (subOutputY > subDest.Y)
						{ // bending downward
							back = numSubs - 1 - i;
						}
						else
						{
							back = i - (numSubs / 2);
						}
					}
					int subOutputX = subDest.X - 20 - 10 * back;
					subOutput = new Location(subOutputX, subOutputY);
					Location mid = new Location(subOutputX, subDest.Y);
					result.add(Wire.create(subOutput, mid));
					result.add(Wire.create(mid, subDest));
				}

				int subX = x + layout.subX - sub.width;
				int subY = y + sub.y;
				placeComponents(result, sub, subX, subY, inputData, subOutput);
			}
		}

		//
		// placeOutput
		//
		private static void placeOutput(CircuitMutation result, Location loc, string name)
		{
			ComponentFactory factory = Pin.FACTORY;
			AttributeSet attrs = factory.createAttributeSet();
			attrs.setValue(StdAttr.FACING, Direction.West);
			attrs.setValue(Pin.ATTR_TYPE, true);
			attrs.setValue(StdAttr.LABEL, name);
			attrs.setValue(Pin.ATTR_LABEL_LOC, Direction.North);
			result.add(factory.createComponent(loc, attrs));
		}

		//
		// placeInputs
		//
		private static void placeInputs(CircuitMutation result, InputData inputData)
		{
			List<Location> forbiddenYs = new List<Location>();
			IComparer<Location> compareYs = new CompareYs();
			int curX = 40;
			int curY = 30;
			for (int i = 0; i < inputData.names.Length; i++)
			{
				string name = inputData.names[i];
				SingleInput singleInput = inputData.inputs[name];

				// determine point where we can intersect with spine
				int spineX = singleInput.spineX;
				Location spineLoc = new Location(spineX, curY);
				if (singleInput.ys.Count > 0)
				{
					// search for a Y that won't intersect with others
					// (we needn't bother if the pin doesn't connect
					// with anything anyway.)
					forbiddenYs.Sort(compareYs);
					while (Collections.binarySearch(forbiddenYs, spineLoc, compareYs) >= 0)
					{
						curY += 10;
						spineLoc = new Location(spineX, curY);
					}
					singleInput.ys.Add(spineLoc);
				}
				Location loc = new Location(curX, curY);

				// now create the pin
				ComponentFactory factory = Pin.FACTORY;
				AttributeSet attrs = factory.createAttributeSet();
				attrs.setValue(StdAttr.FACING, Direction.East);
				attrs.setValue(Pin.ATTR_TYPE, false);
				attrs.setValue(Pin.ATTR_TRISTATE, false);
				attrs.setValue(StdAttr.LABEL, name);
				attrs.setValue(Pin.ATTR_LABEL_LOC, Direction.North);
				result.add(factory.createComponent(loc, attrs));

				List<Location> spine = singleInput.ys;
				if (spine.Count > 0)
				{
					// create wire connecting pin to spine
					/*
					 * This should no longer matter - the wires will be repaired anyway by the circuit's WireRepair class.
					 * if (spine.size() == 2 && spine.get(0).equals(spine.get(1))) { // a freak accident where the input is
					 * used just once, // and it happens that the pin is placed where no // spine is necessary
					 * Iterator<Wire> it = circuit.getWires(spineLoc).iterator(); Wire existing = it.next(); Wire replace =
					 * Wire.create(loc, existing.getEnd1()); result.replace(existing, replace); } else {
					 */
					result.add(Wire.create(loc, spineLoc));
					// }

					// create spine
					spine.Sort(compareYs);
					Location prev = spine[0];
					for (int k = 1, n = spine.Count; k < n; k++)
					{
						Location cur = spine[k];
						if (!cur.Equals(prev))
						{
							result.add(Wire.create(prev, cur));
							prev = cur;
						}
					}
				}

				// advance y and forbid spine intersections for next pin
				forbiddenYs.AddRange(singleInput.ys);
				curY += 50;
			}
		}

		private class CompareYs : IComparer<Location>
		{
			public virtual int Compare(Location a, Location b)
			{
				return a.Y - b.Y;
			}
		}
	}

}
