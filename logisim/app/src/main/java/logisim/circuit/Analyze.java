/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.circuit;

import java.util.*;
import java.util.stream.Collectors;

import kotlin.Pair;
import logisim.analyze.model.AnalyzerModel;
import logisim.analyze.model.Entry;
import logisim.analyze.model.Expression;
import logisim.analyze.model.Expressions;
import logisim.analyze.model.TruthTable;
import logisim.circuit.AnalyzeException.CannotHandle;
import logisim.circuit.AnalyzeException.Circular;
import logisim.circuit.AnalyzeException.Conflict;
import logisim.comp.Component;
import logisim.data.Direction;
import logisim.data.Location;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.instance.Instance;
import logisim.instance.InstanceState;
import logisim.instance.StdAttr;
import logisim.proj.Project;
import logisim.std.wiring.Pin;

public class Analyze {
	private static final int MAX_ITERATIONS = 100;

	private Analyze() {
	}

	//
	// getPinLabels
	//
	/**
	 * Returns a sorted map from Pin objects to String objects, listed in canonical order (top-down order, with ties
	 * broken left-right).
	 */
	public static SortedMap<Instance, String> getPinLabels(Circuit circuit) {
		Comparator<Instance> locOrder = (ac, bc) -> {
			Location a = ac.getLocation();
			Location b = bc.getLocation();
			int ret = Integer.compare(a.y(), b.y());
			if(ret != 0)
				return ret;
			ret = Integer.compare(a.x(), b.x());
			if(ret != 0)
				return ret;
			return a.hashCode() - b.hashCode();
		};
		SortedMap<Instance, String> ret = new TreeMap<>(locOrder);

		// Put the pins into the TreeMap, with null labels
		for (Instance pin : circuit.getAppearance().getPortOffsets(Direction.East).values()) ret.put(pin, null);

		// Process first the pins that the user has given labels.
		HashSet<String> labelsTaken = new HashSet<>();
		for (Instance pin : ret.keySet()) {
			String label = pin.getAttributeSet().getValue(StdAttr.LABEL);
			label = toValidLabel(label);
			if (label != null) {
				if (labelsTaken.contains(label)) {
					int i = 2;
					while (labelsTaken.contains(label + i))
						i++;
					label = label + i;
				}
				ret.put(pin, label);
				labelsTaken.add(label);
			}
		}

		// Now process the unlabeled pins.
		for (Instance pin : ret.keySet()) {
			if (ret.get(pin) != null)
				continue;

			String defaultList;
			if (Pin.FACTORY.isInputPin(pin)) {
				defaultList = Strings.get("defaultInputLabels");
				if (!defaultList.contains(","))
					defaultList = "a,b,c,d,e,f,g,h";
			} else {
				defaultList = Strings.get("defaultOutputLabels");
				if (!defaultList.contains(","))
					defaultList = "x,y,z,u,v,w,s,t";
			}

			String[] options = defaultList.split(",");
			String label = null;
			for (int i = 0; label == null && i < options.length; i++)
				if (!labelsTaken.contains(options[i]))
					label = options[i];
			if (label == null) {
				// This is an extreme measure that should never happen
				// if the default labels are defined properly and the
				// circuit doesn't exceed the maximum number of pins.
				int i = 1;
				do {
					i++;
					label = "x" + i;
				} while (labelsTaken.contains(label));
			}

			labelsTaken.add(label);
			ret.put(pin, label);
		}

		return ret;
	}

	private static String toValidLabel(String label) {
		if (label == null)
			return null;
		StringBuilder end = null;
		StringBuilder ret = new StringBuilder();
		boolean afterWhitespace = false;
		for (int i = 0; i < label.length(); i++) {
			char c = label.charAt(i);
			if (Character.isJavaIdentifierStart(c)) {
				if (afterWhitespace) {
					// capitalize words after the first one
					c = Character.toTitleCase(c);
					afterWhitespace = false;
				}
				ret.append(c);
			} else if (Character.isJavaIdentifierPart(c)) {
				// If we can't place it at the start, we'll dump it
				// onto the end.
				if (!ret.isEmpty())
					ret.append(c);
				else {
					if (end == null)
						end = new StringBuilder();
					end.append(c);
				}
				afterWhitespace = false;
			} else if (Character.isWhitespace(c))
				afterWhitespace = true;
		}
		if (end != null && !ret.isEmpty())
			ret.append(end);
		if (ret.isEmpty())
			return null;
		return ret.toString();
	}

	//
	// computeExpression
	//
	/**
	 * Computes the expression corresponding to the given circuit, or raises ComputeException if difficulties arise.
	 */
	public static void computeExpression(AnalyzerModel model, Circuit circuit, Map<Instance, String> pinNames) throws AnalyzeException {
		ExpressionMap expressionMap = new ExpressionMap(circuit);

		ArrayList<String> inputNames = new ArrayList<>();
		ArrayList<String> outputNames = new ArrayList<>();
		ArrayList<Instance> outputPins = new ArrayList<>();
		for (Map.Entry<Instance, String> entry : pinNames.entrySet()) {
			Instance pin = entry.getKey();
			String label = entry.getValue();
			if (Pin.FACTORY.isInputPin(pin)) {
				expressionMap.currentCause = Instance.getComponentFor(pin);
				Expression e = Expressions.variable(label);
				expressionMap.put(pin.getLocation(), e);
				inputNames.add(label);
			} else {
				outputPins.add(pin);
				outputNames.add(label);
			}
		}

		propagateComponents(expressionMap, circuit.getNonWires());

		for (int iterations = 0; !expressionMap.dirtyPoints.isEmpty(); iterations++) {
			if (iterations > MAX_ITERATIONS)
				throw new Circular();

			propagateWires(expressionMap, new HashSet<>(expressionMap.dirtyPoints));

			Set<Component> dirtyComponents = getDirtyComponents(circuit, expressionMap.dirtyPoints);
			expressionMap.dirtyPoints.clear();
			propagateComponents(expressionMap, dirtyComponents);

			Expression expr = checkForCircularExpressions(expressionMap);
			if (expr != null)
				throw new Circular();
		}

		model.setVariables(inputNames, outputNames);
		for (int i = 0; i < outputPins.size(); i++) {
			Instance pin = outputPins.get(i);
			model.getOutputExpressions().setExpression(outputNames.get(i), expressionMap.get(pin.getLocation()));
		}
	}

	private static class ExpressionMap extends HashMap<Location, Expression> {
		private Circuit circuit;
		private Set<Location> dirtyPoints = new HashSet<>();
		private Map<Location, Component> causes = new HashMap<>();
		private Component currentCause;

		ExpressionMap(Circuit circuit) {
			this.circuit = circuit;
		}

		@Override
		public Expression put(Location point, Expression expression) {
			Expression ret = super.put(point, expression);
			if (currentCause != null)
				causes.put(point, currentCause);
			if (!Objects.equals(ret, expression))
				dirtyPoints.add(point);
			return ret;
		}
	}

	// propagates expressions down wires
	private static void propagateWires(ExpressionMap expressionMap, HashSet<Location> pointsToProcess) throws AnalyzeException {
		expressionMap.currentCause = null;
		for (Location p : pointsToProcess) {
			Expression e = expressionMap.get(p);
			expressionMap.currentCause = expressionMap.causes.get(p);
			WireBundle bundle = expressionMap.circuit.wires.getWireBundle(p);
			if (e == null || bundle == null || bundle.points == null)
				continue;
			for (Location p2 : bundle.points) {
				if (p2.equals(p))
					continue;
				Expression old = expressionMap.get(p2);
				if (old != null) {
					Component eCause = expressionMap.currentCause;
					Component oldCause = expressionMap.causes.get(p2);
					if (eCause != oldCause && !old.equals(e))
						throw new Conflict();
				}
				expressionMap.put(p2, e);
			}
		}
	}

	// computes outputs of affected components
	private static Set<Component> getDirtyComponents(Circuit circuit, Set<Location> pointsToProcess) {
		return pointsToProcess.stream()
				.map(circuit::getNonWires)
				.flatMap(Collection::stream)
				.collect(Collectors.toSet());
	}

	private static void propagateComponents(ExpressionMap expressionMap, Collection<Component> components) throws AnalyzeException {
		for (Component comp : components) {
			ExpressionComputer computer = (ExpressionComputer) comp.getFeature(ExpressionComputer.class);
			// pins are handled elsewhere
			if (computer == null)
				throw new CannotHandle(comp.getFactory().getDisplayName());
			expressionMap.currentCause = comp;
			computer.computeExpression(expressionMap);
		}
	}

	/**
	 * Checks whether any of the recently placed expressions in the expression map are self-referential; if so, return
	 * it.
	 */
	private static Expression checkForCircularExpressions(ExpressionMap expressionMap) {
		return expressionMap.dirtyPoints.stream()
				.map(expressionMap::get)
				.filter(Expression::isCircular)
				.findFirst()
				.orElse(null);
	}

	//
	// ComputeTable
	//
	/** Returns a truth table corresponding to the circuit. */
	public static void computeTable(AnalyzerModel model, Project proj, Circuit circuit, Map<Instance, String> pinLabels) {
		ArrayList<Pair<Instance,String>> inputs = new ArrayList<>();
		ArrayList<Pair<Instance, String>> outputs = new ArrayList<>();
		for (Map.Entry<Instance, String> entry : pinLabels.entrySet()) {
			Instance pin = entry.getKey();
			if (Pin.FACTORY.isInputPin(pin))
				inputs.add(new Pair<>(pin, entry.getValue()));
			else
				outputs.add(new Pair<>(pin, entry.getValue()));
		}

		int inputCount = inputs.size();
		int rowCount = 1 << inputCount;
		Entry[][] columns = new Entry[outputs.size()][rowCount];

		for (int i = 0; i < rowCount; i++) {
			CircuitState circuitState = new CircuitState(proj, circuit);
			for (int j = 0; j < inputCount; j++) {
				Instance pin = inputs.get(j).component1();
				InstanceState pinState = circuitState.getInstanceState(pin);
				boolean value = TruthTable.isInputSet(i, j, inputCount);
				Pin.FACTORY.setValue(pinState, value ? WireValues.TRUE : WireValues.FALSE);
			}

			Propagator prop = circuitState.getPropagator();
			prop.propagate();
			/*
			 * TODO for the SimulatorPrototype class do { prop.step(); } while (prop.isPending());
			 */
			// TODO: Search for circuit state

			if (prop.isOscillating()) {
				for (int j = 0; j < columns.length; j++)
					columns[j][i] = Entry.OSCILLATE_ERROR;
			}
			else {
				for (int j = 0; j < columns.length; j++) {
					Instance pin = outputs.get(j).component1();
					InstanceState pinState = circuitState.getInstanceState(pin);
					Entry out;
					WireValue outValue = Pin.FACTORY.getValue(pinState).get(0);
					if (outValue == WireValues.TRUE)
						out = Entry.ONE;
					else if (outValue == WireValues.FALSE)
						out = Entry.ZERO;
					else if (outValue == WireValues.ERROR)
						out = Entry.BUS_ERROR;
					else
						out = Entry.DONT_CARE;
					columns[j][i] = out;
				}
			}
		}

		model.setVariables(inputs.stream().map(Pair::component2).toList(), outputs.stream().map(Pair::component2).toList());
		for (int i = 0; i < columns.length; i++) model.getTruthTable().setOutputColumn(i, columns[i]);
	}
}
