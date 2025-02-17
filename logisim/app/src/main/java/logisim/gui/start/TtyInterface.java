/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.start;

import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.Map;
import java.util.Map.Entry;

import logisim.circuit.Analyze;
import logisim.circuit.Circuit;
import logisim.circuit.CircuitState;
import logisim.circuit.Propagator;
import logisim.comp.Component;
import logisim.data.WireValue.WireValue;
import logisim.data.WireValue.WireValues;
import logisim.file.FileStatistics.Count;
import logisim.file.LoadFailedException;
import logisim.file.Loader;
import logisim.file.LogisimFile;
import logisim.file.FileStatistics;
import logisim.instance.Instance;
import logisim.instance.InstanceState;
import logisim.proj.Project;
import logisim.std.io.Keyboard;
import logisim.std.io.Tty;
import logisim.std.memory.Ram;
import logisim.std.wiring.Pin;
import logisim.tools.Library;
import logisim.util.StringUtil;

public class TtyInterface {
	public static final int FORMAT_TABLE = 1;
	public static final int FORMAT_SPEED = 2;
	public static final int FORMAT_TTY = 4;
	public static final int FORMAT_HALT = 8;
	public static final int FORMAT_STATISTICS = 16;

	private static boolean lastIsNewline = true;

	public static void sendFromTty(char c) {
		lastIsNewline = c == '\n';
		System.out.print(c); // OK
	}

	private static void ensureLineTerminated() {
		if (!lastIsNewline) {
			lastIsNewline = true;
			System.out.print('\n'); // OK
		}
	}

	public static void run(Startup args) {
		File fileToOpen = args.getFilesToOpen().getFirst();
		Loader loader = new Loader(null);
		LogisimFile file;
		try {
			file = loader.openLogisimFile(fileToOpen, args.getSubstitutions());
		}
		catch (LoadFailedException e) {
			System.err.println(Strings.get("ttyLoadError", fileToOpen.getName())); // OK
			System.exit(-1);
			return;
		}

		int format = args.getTtyFormat();
		if ((format & FORMAT_STATISTICS) != 0) {
			format &= ~FORMAT_STATISTICS;
			displayStatistics(file);
		}
		// no simulation remaining to perform, so just exit
		if (format == 0) System.exit(0);

		Project proj = new Project(file);
		Circuit circuit = file.getMainCircuit();
		Map<Instance, String> pinNames = Analyze.getPinLabels(circuit);
		ArrayList<Instance> outputPins = new ArrayList<>();
		Instance haltPin = null;
		for (Entry<Instance, String> entry : pinNames.entrySet()) {
			Instance pin = entry.getKey();
			String pinName = entry.getValue();
			if (!Pin.FACTORY.isInputPin(pin)) {
				outputPins.add(pin);
				if ("halt".equals(pinName)) haltPin = pin;
			}
		}

		CircuitState circState = new CircuitState(proj, circuit);
		// we have to do our initial propagation before the simulation starts -
		// it's necessary to populate the circuit with substates.
		circState.getPropagator().propagate();
		if (args.getLoadFile() != null) try {
			boolean loaded = loadRam(circState, args.getLoadFile());
			if (!loaded) {
				System.err.println(Strings.get("loadNoRamError")); // OK
				System.exit(-1);
			}
		} catch (IOException e) {
			System.err.println(Strings.get("loadIoError") + ": " + e); // OK
			System.exit(-1);
		}
		int ttyFormat = args.getTtyFormat();
		int simCode = runSimulation(circState, outputPins, haltPin, ttyFormat);
		System.exit(simCode);
	}

	private static void displayStatistics(LogisimFile file) {
		FileStatistics stats = FileStatistics.compute(file, file.getMainCircuit());
		Count total = stats.getTotalWithSubcircuits();
		int maxName = 0;
		for (Count count : stats.getCounts()) {
			int nameLength = count.getFactory().getDisplayName().length();
			if (nameLength > maxName)
				maxName = nameLength;
		}
		String fmt = "%" + countDigits(total.getUniqueCount()) + "d\t" + "%" + countDigits(total.getRecursiveCount())
				+ "d\t";
		String fmtNormal = fmt + "%-" + maxName + "s\t%s\n";
		for (Count count : stats.getCounts()) {
			Library lib = count.getLibrary();
			String libName = lib == null ? "-" : lib.getDisplayName();
			System.out.printf(fmtNormal, // OK
					count.getUniqueCount(), count.getRecursiveCount(),
					count.getFactory().getDisplayName(), libName);
		}
		Count totalWithout = stats.getTotalWithoutSubcircuits();
		System.out.printf(fmt + "%s\n", // OK
				totalWithout.getUniqueCount(), totalWithout.getRecursiveCount(),
				Strings.get("statsTotalWithout"));
		System.out.printf(fmt + "%s\n", // OK
				total.getUniqueCount(), total.getRecursiveCount(),
				Strings.get("statsTotalWith"));
	}

	private static int countDigits(int num) {
		int digits = 1;
		int lessThan = 10;
		while (num >= lessThan) {
			digits++;
			lessThan *= 10;
		}
		return digits;
	}

	private static boolean loadRam(CircuitState circState, File loadFile) throws IOException {
		if (loadFile == null)
			return false;

		boolean found = false;
		for (Component comp : circState.getCircuit().getNonWires())
			if (comp.getFactory() instanceof Ram ramFactory) {
				InstanceState ramState = circState.getInstanceState(comp);
				ramFactory.loadImage(ramState, loadFile);
				found = true;
			}

		for (CircuitState sub : circState.getSubstates()) found |= loadRam(sub, loadFile);
		return found;
	}

	private static boolean prepareForTty(CircuitState circState, ArrayList<InstanceState> keybStates) {
		boolean found = false;
		for (Component comp : circState.getCircuit().getNonWires()) {
			Object factory = comp.getFactory();
			if (factory instanceof Tty ttyFactory) {
				InstanceState ttyState = circState.getInstanceState(comp);
				ttyFactory.sendToStdout(ttyState);
				found = true;
			} else if (factory instanceof Keyboard) {
				keybStates.add(circState.getInstanceState(comp));
				found = true;
			}
		}

		for (CircuitState sub : circState.getSubstates()) found |= prepareForTty(sub, keybStates);
		return found;
	}

	private static int runSimulation(CircuitState circState, ArrayList<Instance> outputPins, Instance haltPin,
			int format) {
		boolean showTable = (format & FORMAT_TABLE) != 0;
		boolean showSpeed = (format & FORMAT_SPEED) != 0;
		boolean showTty = (format & FORMAT_TTY) != 0;
		boolean showHalt = (format & FORMAT_HALT) != 0;

		ArrayList<InstanceState> keyboardStates = null;
		StdinThread stdinThread = null;
		if (showTty) {
			keyboardStates = new ArrayList<>();
			boolean ttyFound = prepareForTty(circState, keyboardStates);
			if (!ttyFound) {
				System.err.println(Strings.get("ttyNoTtyError")); // OK
				System.exit(-1);
			}
			if (keyboardStates.isEmpty()) keyboardStates = null;
			else {
				stdinThread = new StdinThread();
				stdinThread.start();
			}
		}

		int retCode;
		long tickCount = 0;
		long start = System.currentTimeMillis();
		boolean halted = false;
		ArrayList<WireValue> prevOutputs = null;
		Propagator prop = circState.getPropagator();
		while (true) {
			ArrayList<WireValue> curOutputs = new ArrayList<>();
			for (Instance pin : outputPins) {
				InstanceState pinState = circState.getInstanceState(pin);
				WireValue val = Pin.FACTORY.getValue(pinState);
				if (pin == haltPin) halted |= val.equals(WireValues.TRUE);
				else if (showTable) curOutputs.add(val);
			}
			if (showTable) displayTableRow(prevOutputs, curOutputs);

			if (halted) {
				retCode = 0; // normal exit
				break;
			}
			if (prop.isOscillating()) {
				retCode = 1; // abnormal exit
				break;
			}
			if (keyboardStates != null) {
				char[] buffer = stdinThread.getBuffer();
				if (buffer != null)
					for (InstanceState keyState : keyboardStates) Keyboard.addToBuffer(keyState, buffer);
			}
			prevOutputs = curOutputs;
			tickCount++;
			prop.tick();
			prop.propagate();
		}
		long elapse = System.currentTimeMillis() - start;
		if (showTty)
			ensureLineTerminated();
		if (showHalt || retCode != 0) if (retCode == 0) System.out.println(Strings.get("ttyHaltReasonPin")); // OK
		else System.out.println(Strings.get("ttyHaltReasonOscillation")); // OK
		if (showSpeed) displaySpeed(tickCount, elapse);
		return retCode;
	}

	private static void displayTableRow(ArrayList<WireValue> prevOutputs, ArrayList<WireValue> curOutputs) {
		boolean shouldPrint = false;
		if (prevOutputs == null) shouldPrint = true;
		else for (int i = 0; i < curOutputs.size(); i++) {
			WireValue a = prevOutputs.get(i);
			WireValue b = curOutputs.get(i);
			if (!a.equals(b)) {
				shouldPrint = true;
				break;
			}
		}
		if (shouldPrint) {
			for (int i = 0; i < curOutputs.size(); i++) {
				if (i != 0)
					System.out.print("\t"); // OK
				System.out.print(curOutputs.get(i)); // OK
			}
			System.out.println(); // OK
		}
	}

	private static void displaySpeed(long tickCount, long elapse) {
		double hertz = (double) tickCount / elapse * 1000.0;
		double precision;
		if (hertz >= 100)
			precision = 1.0;
		else if (hertz >= 10)
			precision = 0.1;
		else if (hertz >= 1)
			precision = 0.01;
		else if (hertz >= 0.01)
			precision = 0.0001;
		else
			precision = 0.0000001;
		hertz = (int) (hertz / precision) * precision;
		String hertzStr = "" + hertz;
		System.out.println(StringUtil.format(Strings.get("ttySpeedMsg"), // OK
				hertzStr, "" + tickCount, "" + elapse));
	}

	// It's possible to avoid using the separate thread using System.in.available(),
	// but this doesn't quite work because on some systems, the keyboard input
	// is not interactively echoed until System.in.read() is invoked.
	private static class StdinThread extends Thread {
		private final LinkedList<char[]> queue; // of char[]

		public StdinThread() {
			queue = new LinkedList<>();
		}

		public char[] getBuffer() {
			synchronized (queue) {
				if (queue.isEmpty()) return null;
				else return queue.removeFirst();
			}
		}

		@Override
		public void run() {
			InputStreamReader stdin = new InputStreamReader(System.in);
			char[] buffer = new char[32];
			while (true) try {
				int nbytes = stdin.read(buffer);
				if (nbytes > 0) {
					char[] add = new char[nbytes];
					System.arraycopy(buffer, 0, add, 0, nbytes);
					synchronized (queue) {
						queue.addLast(add);
					}
				}
			} catch (IOException e) {
			}
		}
	}
}
