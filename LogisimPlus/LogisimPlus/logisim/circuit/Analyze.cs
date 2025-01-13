// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.circuit
{

	using AnalyzerModel = logisim.analyze.model.AnalyzerModel;
	using Entry = logisim.analyze.model.Entry;
	using Expression = logisim.analyze.model.Expression;
	using Expressions = logisim.analyze.model.Expressions;
	using TruthTable = logisim.analyze.model.TruthTable;
	using Component = logisim.comp.Component;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Value = logisim.data.Value;
	using Instance = logisim.instance.Instance;
	using InstanceState = logisim.instance.InstanceState;
	using StdAttr = logisim.instance.StdAttr;
	using Project = logisim.proj.Project;
	using Pin = logisim.std.wiring.Pin;

	public class Analyze
	{
		private const int MAX_ITERATIONS = 100;

		private Analyze()
		{
		}

		//
		// getPinLabels
		//
		/// <summary>
		/// Returns a sorted map from Pin objects to String objects, listed in canonical order (top-down order, with ties
		/// broken left-right).
		/// </summary>
		public static SortedDictionary<Instance, string> getPinLabels(Circuit circuit)
		{
			IComparer<Instance> locOrder = new ComparatorAnonymousInnerClass();
			SortedDictionary<Instance, string> ret = new SortedDictionary<Instance, string>(locOrder);

			// Put the pins into the TreeMap, with null labels
			foreach (Instance pin in circuit.Appearance.getPortOffsets(Direction.East).Values)
			{
				ret[pin] = null;
			}

			// Process first the pins that the user has given labels.
			List<Instance> pinList = new List<Instance>(ret.Keys);
			HashSet<string> labelsTaken = new HashSet<string>();
			foreach (Instance pin in pinList)
			{
				string label = (string)pin.AttributeSet.getValue(StdAttr.LABEL);
				label = toValidLabel(label);
				if (!string.ReferenceEquals(label, null))
				{
					if (labelsTaken.Contains(label))
					{
						int i = 2;
						while (labelsTaken.Contains(label + i))
						{
							i++;
						}
						label = label + i;
					}
					ret[pin] = label;
					labelsTaken.Add(label);
				}
			}

			// Now process the unlabeled pins.
			foreach (Instance pin in pinList)
			{
				if (ret[pin] != null)
				{
					continue;
				}

				string defaultList;
				if (Pin.FACTORY.isInputPin(pin))
				{
					defaultList = Strings.get("defaultInputLabels");
					if (defaultList.IndexOf(",", StringComparison.Ordinal) < 0)
					{
						defaultList = "a,b,c,d,e,f,g,h";
					}
				}
				else
				{
					defaultList = Strings.get("defaultOutputLabels");
					if (defaultList.IndexOf(",", StringComparison.Ordinal) < 0)
					{
						defaultList = "x,y,z,u,v,w,s,t";
					}
				}

				string[] options = defaultList.Split(",", true);
				string label = null;
				for (int i = 0; string.ReferenceEquals(label, null) && i < options.Length; i++)
				{
					if (!labelsTaken.Contains(options[i]))
					{
						label = options[i];
					}
				}
				if (string.ReferenceEquals(label, null))
				{
					// This is an extreme measure that should never happen
					// if the default labels are defined properly and the
					// circuit doesn't exceed the maximum number of pins.
					int i = 1;
					do
					{
						i++;
						label = "x" + i;
					} while (labelsTaken.Contains(label));
				}

				labelsTaken.Add(label);
				ret[pin] = label;
			}

			return ret;
		}

		private class ComparatorAnonymousInnerClass : IComparer<Instance>
		{
			public int Compare(Instance ac, Instance bc)
			{
				Location a = ac.Location;
				Location b = bc.Location;
				if (a.Y < b.Y)
				{
					return -1;
				}
				if (a.Y > b.Y)
				{
					return 1;
				}
				if (a.X < b.X)
				{
					return -1;
				}
				if (a.X > b.X)
				{
					return 1;
				}
				return a.GetHashCode() - b.GetHashCode();
			}
		}

		private static string toValidLabel(string label)
		{
			if (string.ReferenceEquals(label, null))
			{
				return null;
			}
			StringBuilder end = null;
			StringBuilder ret = new StringBuilder();
			bool afterWhitespace = false;
			for (int i = 0; i < label.Length; i++)
			{
				char c = label[i];
				if (afterWhitespace)
				{
					// capitalize words after the first one
					c = char.ToUpper(c);
					afterWhitespace = false;
				}
				ret.Append(c);
				// If we can't place it at the start, we'll dump it
				// onto the end.
				if (ret.Length > 0)
				{
					ret.Append(c);
				}
				else
				{
					if (end == null)
					{
						end = new StringBuilder();
					}
					end.Append(c);
				}
				afterWhitespace = false;
				if (char.IsWhiteSpace(c))
				{
					afterWhitespace = true;
				}
			}
			if (end != null && ret.Length > 0)
			{
				ret.Append(end.ToString());
			}
			if (ret.Length == 0)
			{
				return null;
			}
			return ret.ToString();
		}

		//
		// computeExpression
		//
		/// <summary>
		/// Computes the expression corresponding to the given circuit, or raises ComputeException if difficulties arise.
		/// </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public static void computeExpression(logisim.analyze.model.AnalyzerModel model, Circuit circuit, java.util.Map<logisim.instance.Instance, String> pinNames) throws AnalyzeException
		public static void computeExpression(AnalyzerModel model, Circuit circuit, Dictionary<Instance, string> pinNames)
		{
			ExpressionMap expressionMap = new ExpressionMap(circuit);

			List<string> inputNames = new List<string>();
			List<string> outputNames = new List<string>();
			List<Instance> outputPins = new List<Instance>();
			foreach (KeyValuePair<Instance, string> entry in pinNames.SetOfKeyValuePairs())
			{
				Instance pin = entry.Key;
				string label = entry.Value;
				if (Pin.FACTORY.isInputPin(pin))
				{
					expressionMap.currentCause = Instance.getComponentFor(pin);
					Expression e = Expressions.variable(label);
					expressionMap[pin.Location] = e;
					inputNames.Add(label);
				}
				else
				{
					outputPins.Add(pin);
					outputNames.Add(label);
				}
			}

			propagateComponents(expressionMap, circuit.NonWires);

			for (int iterations = 0; expressionMap.dirtyPoints.Count > 0; iterations++)
			{
				if (iterations > MAX_ITERATIONS)
				{
					throw new AnalyzeException.Circular();
				}

				propagateWires(expressionMap, new HashSet<Location>(expressionMap.dirtyPoints));

				HashSet<Component> dirtyComponents = getDirtyComponents(circuit, expressionMap.dirtyPoints);
				expressionMap.dirtyPoints.Clear();
				propagateComponents(expressionMap, dirtyComponents);

				Expression expr = checkForCircularExpressions(expressionMap);
				if (expr != null)
				{
					throw new AnalyzeException.Circular();
				}
			}

			model.setVariables(inputNames, outputNames);
			for (int i = 0; i < outputPins.Count; i++)
			{
				Instance pin = outputPins[i];
				model.OutputExpressions.setExpression(outputNames[i], expressionMap[pin.Location]);
			}
		}

		private class ExpressionMap : Dictionary<Location, Expression>
		{
			internal Circuit circuit;
			internal HashSet<Location> dirtyPoints = new HashSet<Location>();
			internal Dictionary<Location, Component> causes = new Dictionary<Location, Component>();
			internal Component currentCause = null;

			internal ExpressionMap(Circuit circuit)
			{
				this.circuit = circuit;
			}

			public override Expression put(Location point, Expression expression)
			{
				Expression ret = base[point] = expression;
				if (currentCause != null)
				{
					causes[point] = currentCause;
				}
				if (ret == null ? expression != null :!ret.Equals(expression))
				{
					dirtyPoints.Add(point);
				}
				return ret;
			}
		}

		// propagates expressions down wires
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static void propagateWires(ExpressionMap expressionMap, java.util.HashSet<logisim.data.Location> pointsToProcess) throws AnalyzeException
		private static void propagateWires(ExpressionMap expressionMap, HashSet<Location> pointsToProcess)
		{
			expressionMap.currentCause = null;
			foreach (Location p in pointsToProcess)
			{
				Expression e = expressionMap[p];
				expressionMap.currentCause = expressionMap.causes[p];
				WireBundle bundle = expressionMap.circuit.wires.getWireBundle(p);
				if (e != null && bundle != null && bundle.points != null)
				{
					foreach (Location p2 in bundle.points)
					{
						if (p2.Equals(p))
						{
							continue;
						}
						Expression old = expressionMap[p2];
						if (old != null)
						{
							Component eCause = expressionMap.currentCause;
							Component oldCause = expressionMap.causes[p2];
							if (eCause != oldCause && !old.Equals(e))
							{
								throw new AnalyzeException.Conflict();
							}
						}
						expressionMap[p2] = e;
					}
				}
			}
		}

		// computes outputs of affected components
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static java.util.HashSet<logisim.comp.Component> getDirtyComponents(Circuit circuit, java.util.Set<logisim.data.Location> pointsToProcess) throws AnalyzeException
		private static HashSet<Component> getDirtyComponents(Circuit circuit, HashSet<Location> pointsToProcess)
		{
			HashSet<Component> dirtyComponents = new HashSet<Component>();
			foreach (Location point in pointsToProcess)
			{
				foreach (Component comp in circuit.getNonWires(point))
				{
					dirtyComponents.Add(comp);
				}
			}
			return dirtyComponents;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static void propagateComponents(ExpressionMap expressionMap, java.util.Collection<logisim.comp.Component> components) throws AnalyzeException
		private static void propagateComponents(ExpressionMap expressionMap, ICollection<Component> components)
		{
			foreach (Component comp in components)
			{
				ExpressionComputer computer = (ExpressionComputer) comp.getFeature(typeof(ExpressionComputer));
				if (computer != null)
				{
					try
					{
						expressionMap.currentCause = comp;
						computer.computeExpression(expressionMap);
					}
					catch (System.NotSupportedException)
					{
						throw new AnalyzeException.CannotHandle(comp.Factory.DisplayName);
					}
				}
				else if (comp.Factory is Pin)
				{
					; // pins are handled elsewhere
				}
				else
				{
					// pins are handled elsewhere
					throw new AnalyzeException.CannotHandle(comp.Factory.DisplayName);
				}
			}
		}

		/// <summary>
		/// Checks whether any of the recently placed expressions in the expression map are self-referential; if so, return
		/// it.
		/// </summary>
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static logisim.analyze.model.Expression checkForCircularExpressions(ExpressionMap expressionMap) throws AnalyzeException
		private static Expression checkForCircularExpressions(ExpressionMap expressionMap)
		{
			foreach (Location point in expressionMap.dirtyPoints)
			{
				Expression expr = expressionMap[point];
				if (expr.Circular)
				{
					return expr;
				}
			}
			return null;
		}

		//
		// ComputeTable
		//
		/// <summary>
		/// Returns a truth table corresponding to the circuit. </summary>
		public static void computeTable(AnalyzerModel model, Project proj, Circuit circuit, Dictionary<Instance, string> pinLabels)
		{
			List<Instance> inputPins = new List<Instance>();
			List<string> inputNames = new List<string>();
			List<Instance> outputPins = new List<Instance>();
			List<string> outputNames = new List<string>();
			foreach (KeyValuePair<Instance, string> entry in pinLabels.SetOfKeyValuePairs())
			{
				Instance pin = entry.Key;
				if (Pin.FACTORY.isInputPin(pin))
				{
					inputPins.Add(pin);
					inputNames.Add(entry.Value);
				}
				else
				{
					outputPins.Add(pin);
					outputNames.Add(entry.Value);
				}
			}

			int inputCount = inputPins.Count;
			int rowCount = 1 << inputCount;
// JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
// ORIGINAL LINE: Entry[][] columns = new Entry[outputPins.Count][rowCount];
			Entry[][] columns = RectangularArrays.RectangularEntryArray(outputPins.Count, rowCount);

			for (int i = 0; i < rowCount; i++)
			{
				CircuitState circuitState = new CircuitState(proj, circuit);
				for (int j = 0; j < inputCount; j++)
				{
					Instance pin = inputPins[j];
					InstanceState pinState = circuitState.getInstanceState(pin);
					bool value = TruthTable.isInputSet(i, j, inputCount);
					Pin.FACTORY.setValue(pinState, value ? Value.TRUE : Value.FALSE);
				}

				Propagator prop = circuitState.Propagator;
				prop.propagate();
				/*
				 * TODO for the SimulatorPrototype class do { prop.step(); } while (prop.isPending());
				 */
				// TODO: Search for circuit state

				if (prop.Oscillating)
				{
					for (int j = 0; j < columns.Length; j++)
					{
						columns[j][i] = Entry.OSCILLATE_ERROR;
					}
				}
				else
				{
					for (int j = 0; j < columns.Length; j++)
					{
						Instance pin = outputPins[j];
						InstanceState pinState = circuitState.getInstanceState(pin);
						Entry @out;
						Value outValue = Pin.FACTORY.getValue(pinState).get(0);
						if (outValue == Value.TRUE)
						{
							@out = Entry.ONE;
						}
						else if (outValue == Value.FALSE)
						{
							@out = Entry.ZERO;
						}
						else if (outValue == Value.ERROR)
						{
							@out = Entry.BUS_ERROR;
						}
						else
						{
							@out = Entry.DONT_CARE;
						}
						columns[j][i] = @out;
					}
				}
			}

			model.setVariables(inputNames, outputNames);
			for (int i = 0; i < columns.Length; i++)
			{
				model.TruthTable.setOutputColumn(i, columns[i]);
			}
		}
	}

}
