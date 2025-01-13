// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using LogisimPlus.Java;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.start
{

	using Analyze = logisim.circuit.Analyze;
	using Circuit = logisim.circuit.Circuit;
	using CircuitState = logisim.circuit.CircuitState;
	using Propagator = logisim.circuit.Propagator;
	using Component = logisim.comp.Component;
	using Value = logisim.data.Value;
	using LoadFailedException = logisim.file.LoadFailedException;
	using Loader = logisim.file.Loader;
	using LogisimFile = logisim.file.LogisimFile;
	using FileStatistics = logisim.file.FileStatistics;
	using Instance = logisim.instance.Instance;
	using InstanceState = logisim.instance.InstanceState;
	using Project = logisim.proj.Project;
	using Keyboard = logisim.std.io.Keyboard;
	using Tty = logisim.std.io.Tty;
	using Ram = logisim.std.memory.Ram;
	using Pin = logisim.std.wiring.Pin;
	using Library = logisim.tools.Library;
	using StringUtil = logisim.util.StringUtil;

	public class TtyInterface
	{
		public const int FORMAT_TABLE = 1;
		public const int FORMAT_SPEED = 2;
		public const int FORMAT_TTY = 4;
		public const int FORMAT_HALT = 8;
		public const int FORMAT_STATISTICS = 16;

		private static bool lastIsNewline = true;

		public static void sendFromTty(char c)
		{
			lastIsNewline = c == '\n';
			Console.Write(c); // OK
		}

		private static void ensureLineTerminated()
		{
			if (!lastIsNewline)
			{
				lastIsNewline = true;
				Console.Write('\n'); // OK
			}
		}

		public static void run(Startup args)
		{
			File fileToOpen = args.FilesToOpen[0];
			Loader loader = new Loader(null);
			LogisimFile file;
			try
			{
				file = loader.openLogisimFile(fileToOpen, args.Substitutions);
			}
			catch (LoadFailedException)
			{
				Console.Error.WriteLine(Strings.get("ttyLoadError", fileToOpen.getName())); // OK
				Environment.Exit(-1);
				return;
			}

			int format = args.TtyFormat;
			if ((format & FORMAT_STATISTICS) != 0)
			{
				format &= ~FORMAT_STATISTICS;
				displayStatistics(file);
			}
			if (format == 0)
			{ // no simulation remaining to perform, so just exit
				Environment.Exit(0);
			}

			Project proj = new Project(file);
			Circuit circuit = file.MainCircuit;
			SortedDictionary<Instance, string> pinNames = Analyze.getPinLabels(circuit);
			List<Instance> outputPins = new List<Instance>();
			Instance haltPin = null;
			foreach (KeyValuePair<Instance, string> entry in pinNames)
			{
				Instance pin = entry.Key;
				string pinName = entry.Value;
				if (!Pin.FACTORY.isInputPin(pin))
				{
					outputPins.Add(pin);
					if (pinName.Equals("halt"))
					{
						haltPin = pin;
					}
				}
			}

			CircuitState circState = new CircuitState(proj, circuit);
			// we have to do our initial propagation before the simulation starts -
			// it's necessary to populate the circuit with substates.
			circState.Propagator.propagate();
			if (args.LoadFile != null)
			{
				try
				{
					bool loaded = loadRam(circState, args.LoadFile);
					if (!loaded)
					{
						Console.Error.WriteLine(Strings.get("loadNoRamError")); // OK
						Environment.Exit(-1);
					}
				}
				catch (IOException e)
				{
					Console.Error.WriteLine(Strings.get("loadIoError") + ": " + e.ToString()); // OK
					Environment.Exit(-1);
				}
			}
			int ttyFormat = args.TtyFormat;
			int simCode = runSimulation(circState, outputPins, haltPin, ttyFormat);
			Environment.Exit(simCode);
		}

		private static void displayStatistics(LogisimFile file)
		{
			FileStatistics stats = FileStatistics.compute(file, file.MainCircuit);
			FileStatistics.Count total = stats.TotalWithSubcircuits;
			int maxName = 0;
			foreach (FileStatistics.Count count in stats.Counts)
			{
				int nameLength = count.Factory.DisplayName.Length;
				if (nameLength > maxName)
				{
					maxName = nameLength;
				}
			}
			string fmt = "%" + countDigits(total.UniqueCount) + "d\t" + "%" + countDigits(total.RecursiveCount) + "d\t";
			string fmtNormal = fmt + "%-" + maxName + "s\t%s\n";
			foreach (FileStatistics.Count count in stats.Counts)
			{
				Library lib = count.Library;
				string libName = lib == null ? "-" : lib.DisplayName;
				DebugOut.printf(fmtNormal, Convert.ToInt32(count.UniqueCount), Convert.ToInt32(count.RecursiveCount), count.Factory.DisplayName, libName);
			}
			FileStatistics.Count totalWithout = stats.TotalWithoutSubcircuits;
			DebugOut.printf(fmt + "%s\n", Convert.ToInt32(totalWithout.UniqueCount), Convert.ToInt32(totalWithout.RecursiveCount), Strings.get("statsTotalWithout"));
			DebugOut.printf(fmt + "%s\n", Convert.ToInt32(total.UniqueCount), Convert.ToInt32(total.RecursiveCount), Strings.get("statsTotalWith"));
		}

		private static int countDigits(int num)
		{
			int digits = 1;
			int lessThan = 10;
			while (num >= lessThan)
			{
				digits++;
				lessThan *= 10;
			}
			return digits;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private static boolean loadRam(logisim.circuit.CircuitState circState, java.io.File loadFile) throws java.io.IOException
		private static bool loadRam(CircuitState circState, File loadFile)
		{
			if (loadFile == null)
			{
				return false;
			}

			bool found = false;
			foreach (Component comp in circState.Circuit.NonWires)
			{
				if (comp.Factory is Ram)
				{
					Ram ramFactory = (Ram) comp.Factory;
					InstanceState ramState = circState.getInstanceState(comp);
					ramFactory.loadImage(ramState, loadFile);
					found = true;
				}
			}

			foreach (CircuitState sub in circState.Substates)
			{
				found |= loadRam(sub, loadFile);
			}
			return found;
		}

		private static bool prepareForTty(CircuitState circState, List<InstanceState> keybStates)
		{
			bool found = false;
			foreach (Component comp in circState.Circuit.NonWires)
			{
				object factory = comp.Factory;
				if (factory is Tty)
				{
					Tty ttyFactory = (Tty) factory;
					InstanceState ttyState = circState.getInstanceState(comp);
					ttyFactory.sendToStdout(ttyState);
					found = true;
				}
				else if (factory is Keyboard)
				{
					keybStates.Add(circState.getInstanceState(comp));
					found = true;
				}
			}

			foreach (CircuitState sub in circState.Substates)
			{
				found |= prepareForTty(sub, keybStates);
			}
			return found;
		}

		private static int runSimulation(CircuitState circState, List<Instance> outputPins, Instance haltPin, int format)
		{
			bool showTable = (format & FORMAT_TABLE) != 0;
			bool showSpeed = (format & FORMAT_SPEED) != 0;
			bool showTty = (format & FORMAT_TTY) != 0;
			bool showHalt = (format & FORMAT_HALT) != 0;

			List<InstanceState> keyboardStates = null;
			StdinThread stdinThread = null;
			if (showTty)
			{
				keyboardStates = new List<InstanceState>();
				bool ttyFound = prepareForTty(circState, keyboardStates);
				if (!ttyFound)
				{
					Console.Error.WriteLine(Strings.get("ttyNoTtyError")); // OK
					Environment.Exit(-1);
				}
				if (keyboardStates.Count == 0)
				{
					keyboardStates = null;
				}
				else
				{
					stdinThread = new StdinThread();
					stdinThread.run();
				}
			}

			int retCode;
			long tickCount = 0;
			long start = DateTimeHelper.CurrentUnixTimeMillis();
			bool halted = false;
			List<Value> prevOutputs = null;
			Propagator prop = circState.Propagator;
			while (true)
			{
				List<Value> curOutputs = new List<Value>();
				foreach (Instance pin in outputPins)
				{
					InstanceState pinState = circState.getInstanceState(pin);
					Value val = Pin.FACTORY.getValue(pinState);
					if (pin == haltPin)
					{
						halted |= val.Equals(Value.TRUE);
					}
					else if (showTable)
					{
						curOutputs.Add(val);
					}
				}
				if (showTable)
				{
					displayTableRow(prevOutputs, curOutputs);
				}

				if (halted)
				{
					retCode = 0; // normal exit
					break;
				}
				if (prop.Oscillating)
				{
					retCode = 1; // abnormal exit
					break;
				}
				if (keyboardStates != null && stdinThread != null)
				{
					char[] buffer = stdinThread.Buffer;
					if (buffer != null)
					{
						foreach (InstanceState keyState in keyboardStates)
						{
							Keyboard.addToBuffer(keyState, buffer);
						}
					}
				}
				prevOutputs = curOutputs;
				tickCount++;
				prop.tick();
				prop.propagate();
			}
			long elapse = DateTimeHelper.CurrentUnixTimeMillis() - start;
			if (showTty)
			{
				ensureLineTerminated();
			}
			if (showHalt || retCode != 0)
			{
				if (retCode == 0)
				{
					Console.WriteLine(Strings.get("ttyHaltReasonPin")); // OK
				}
				else if (retCode == 1)
				{
					Console.WriteLine(Strings.get("ttyHaltReasonOscillation")); // OK
				}
			}
			if (showSpeed)
			{
				displaySpeed(tickCount, elapse);
			}
			return retCode;
		}

		private static void displayTableRow(List<Value> prevOutputs, List<Value> curOutputs)
		{
			bool shouldPrint = false;
			if (prevOutputs == null)
			{
				shouldPrint = true;
			}
			else
			{
				for (int i = 0; i < curOutputs.Count; i++)
				{
					Value a = prevOutputs[i];
					Value b = curOutputs[i];
					if (!a.Equals(b))
					{
						shouldPrint = true;
						break;
					}
				}
			}
			if (shouldPrint)
			{
				for (int i = 0; i < curOutputs.Count; i++)
				{
					if (i != 0)
					{
						Console.Write("\t"); // OK
					}
					Console.Write(curOutputs[i]); // OK
				}
				Console.WriteLine(); // OK
			}
		}

		private static void displaySpeed(long tickCount, long elapse)
		{
			double hertz = (double) tickCount / elapse * 1000.0;
			double precision;
			if (hertz >= 100)
			{
				precision = 1.0;
			}
			else if (hertz >= 10)
			{
				precision = 0.1;
			}
			else if (hertz >= 1)
			{
				precision = 0.01;
			}
			else if (hertz >= 0.01)
			{
				precision = 0.0001;
			}
			else
			{
				precision = 0.0000001;
			}
			hertz = (int)(hertz / precision) * precision;
			string hertzStr = hertz == (int) hertz ? "" + (int) hertz : "" + hertz;
			Console.WriteLine(StringUtil.format(Strings.get("ttySpeedMsg"), hertzStr, "" + tickCount, "" + elapse));
		}

		// It's possible to avoid using the separate thread using System.in.available(),
		// but this doesn't quite work because on some systems, the keyboard input
		// is not interactively echoed until System.in.read() is invoked.
		private class StdinThread
		{
			internal Queue<char[]> queue; // of char[]

			public StdinThread()
			{
				queue = new Queue<char[]>();
			}

			public virtual char[] Buffer
			{
				get
				{
					lock (queue)
					{
						if (queue.Count == 0)
						{
							return null;
						}
						else
						{
							return queue.Dequeue();
						}
					}
				}
			}

			public void run()
			{
				Task.Run(() => {
					TextReader stdin = Console.In;
					char[] buffer = new char[32];
					while (true)
					{
						try
						{
							int nbytes = stdin.Read(buffer, 0, buffer.Length);
							if (nbytes > 0)
							{
								char[] add = new char[nbytes];
								Array.Copy(buffer, 0, add, 0, nbytes);
								lock (queue)
								{
									queue.Enqueue(add);
								}
							}
						}
						catch (IOException)
						{
						}
					}
				});
			}
		}
	}

}
