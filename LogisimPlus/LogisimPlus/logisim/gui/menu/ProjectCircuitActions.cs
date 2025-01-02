// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.menu
{


	using Analyzer = logisim.analyze.gui.Analyzer;
	using AnalyzerManager = logisim.analyze.gui.AnalyzerManager;
	using AnalyzerModel = logisim.analyze.model.AnalyzerModel;
	using Analyze = logisim.circuit.Analyze;
	using AnalyzeException = logisim.circuit.AnalyzeException;
	using Circuit = logisim.circuit.Circuit;
	using LogisimFileActions = logisim.file.LogisimFileActions;
	using Instance = logisim.instance.Instance;
	using StdAttr = logisim.instance.StdAttr;
	using Project = logisim.proj.Project;
	using Pin = logisim.std.wiring.Pin;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using StringUtil = logisim.util.StringUtil;

	public class ProjectCircuitActions
	{
		private ProjectCircuitActions()
		{
		}

		public static void doAddCircuit(Project proj)
		{
			string name = promptForCircuitName(proj.Frame, proj.LogisimFile, "");
			if (!string.ReferenceEquals(name, null))
			{
				Circuit circuit = new Circuit(name);
				proj.doAction(LogisimFileActions.addCircuit(circuit));
				proj.CurrentCircuit = circuit;
			}
		}

		private static string promptForCircuitName(JFrame frame, Library lib, string initialValue)
		{
			JLabel label = new JLabel(Strings.get("circuitNamePrompt"));
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final javax.swing.JTextField field = new javax.swing.JTextField(15);
			JTextField field = new JTextField(15);
			field.setText(initialValue);
			JLabel error = new JLabel(" ");
			GridBagLayout gb = new GridBagLayout();
			GridBagConstraints gc = new GridBagConstraints();
			JPanel strut = new JPanel(null);
			strut.setPreferredSize(new Dimension(3 * field.getPreferredSize().width / 2, 0));
			JPanel panel = new JPanel(gb);
			gc.gridx = 0;
			gc.gridy = GridBagConstraints.RELATIVE;
			gc.weightx = 1.0;
			gc.fill = GridBagConstraints.NONE;
			gc.anchor = GridBagConstraints.LINE_START;
			gb.setConstraints(label, gc);
			panel.add(label);
			gb.setConstraints(field, gc);
			panel.add(field);
			gb.setConstraints(error, gc);
			panel.add(error);
			gb.setConstraints(strut, gc);
			panel.add(strut);
			JOptionPane pane = new JOptionPane(panel, JOptionPane.QUESTION_MESSAGE, JOptionPane.OK_CANCEL_OPTION);
			pane.setInitialValue(field);
			JDialog dlog = pane.createDialog(frame, Strings.get("circuitNameDialogTitle"));
			dlog.addWindowFocusListener(new WindowFocusListenerAnonymousInnerClass(field));

			while (true)
			{
				field.selectAll();
				dlog.pack();
				dlog.setVisible(true);
				field.requestFocusInWindow();
				object action = pane.getValue();
				if (action == null || !(action is int?) || ((int?) action).Value != JOptionPane.OK_OPTION)
				{
					return null;
				}

				string name = field.getText().Trim();
				if (name.Equals(""))
				{
					error.setText(Strings.get("circuitNameMissingError"));
				}
				else
				{
					if (lib.getTool(name) == null)
					{
						return name;
					}
					else
					{
						error.setText(Strings.get("circuitNameDuplicateError"));
					}
				}
			}
		}

		private class WindowFocusListenerAnonymousInnerClass : WindowFocusListener
		{
			private JTextField field;

			public WindowFocusListenerAnonymousInnerClass(JTextField field)
			{
				this.field = field;
			}

			public void windowGainedFocus(WindowEvent arg0)
			{
				field.requestFocus();
			}

			public void windowLostFocus(WindowEvent arg0)
			{
			}
		}

		public static void doMoveCircuit(Project proj, Circuit cur, int delta)
		{
			AddTool tool = proj.LogisimFile.getAddTool(cur);
			if (tool != null)
			{
				int oldPos = proj.LogisimFile.Circuits.IndexOf(cur);
				int newPos = oldPos + delta;
				int toolsCount = proj.LogisimFile.Tools.Count;
				if (newPos >= 0 && newPos < toolsCount)
				{
					proj.doAction(LogisimFileActions.moveCircuit(tool, newPos));
				}
			}
		}

		public static void doSetAsMainCircuit(Project proj, Circuit circuit)
		{
			proj.doAction(LogisimFileActions.setMainCircuit(circuit));
		}

		public static void doRemoveCircuit(Project proj, Circuit circuit)
		{
			if (proj.LogisimFile.Tools.Count == 1)
			{
				JOptionPane.showMessageDialog(proj.Frame, Strings.get("circuitRemoveLastError"), Strings.get("circuitRemoveErrorTitle"), JOptionPane.ERROR_MESSAGE);
			}
			else if (!proj.Dependencies.canRemove(circuit))
			{
				JOptionPane.showMessageDialog(proj.Frame, Strings.get("circuitRemoveUsedError"), Strings.get("circuitRemoveErrorTitle"), JOptionPane.ERROR_MESSAGE);
			}
			else
			{
				proj.doAction(LogisimFileActions.removeCircuit(circuit));
			}
		}

		public static void doAnalyze(Project proj, Circuit circuit)
		{
			IDictionary<Instance, string> pinNames = Analyze.getPinLabels(circuit);
			List<string> inputNames = new List<string>();
			List<string> outputNames = new List<string>();
			foreach (KeyValuePair<Instance, string> entry in pinNames.SetOfKeyValuePairs())
			{
				Instance pin = entry.Key;
				bool isInput = Pin.FACTORY.isInputPin(pin);
				if (isInput)
				{
					inputNames.Add(entry.Value);
				}
				else
				{
					outputNames.Add(entry.Value);
				}
				if (pin.getAttributeValue(StdAttr.WIDTH).getWidth() > 1)
				{
					if (isInput)
					{
						analyzeError(proj, Strings.get("analyzeMultibitInputError"));
					}
					else
					{
						analyzeError(proj, Strings.get("analyzeMultibitOutputError"));
					}
					return;
				}
			}
			if (inputNames.Count > AnalyzerModel.MAX_INPUTS)
			{
				analyzeError(proj, StringUtil.format(Strings.get("analyzeTooManyInputsError"), "" + AnalyzerModel.MAX_INPUTS));
				return;
			}
			if (outputNames.Count > AnalyzerModel.MAX_OUTPUTS)
			{
				analyzeError(proj, StringUtil.format(Strings.get("analyzeTooManyOutputsError"), "" + AnalyzerModel.MAX_OUTPUTS));
				return;
			}

			Analyzer analyzer = AnalyzerManager.Analyzer;
			analyzer.Model.setCurrentCircuit(proj, circuit);
			configureAnalyzer(proj, circuit, analyzer, pinNames, inputNames, outputNames);
			analyzer.setVisible(true);
			analyzer.toFront();
		}

		private static void configureAnalyzer(Project proj, Circuit circuit, Analyzer analyzer, IDictionary<Instance, string> pinNames, List<string> inputNames, List<string> outputNames)
		{
			analyzer.Model.setVariables(inputNames, outputNames);

			// If there are no inputs, we stop with that tab selected
			if (inputNames.Count == 0)
			{
				analyzer.SelectedTab = Analyzer.INPUTS_TAB;
				return;
			}

			// If there are no outputs, we stop with that tab selected
			if (outputNames.Count == 0)
			{
				analyzer.SelectedTab = Analyzer.OUTPUTS_TAB;
				return;
			}

			// Attempt to show the corresponding expression
			try
			{
				Analyze.computeExpression(analyzer.Model, circuit, pinNames);
				analyzer.SelectedTab = Analyzer.EXPRESSION_TAB;
				return;
			}
			catch (AnalyzeException ex)
			{
				JOptionPane.showMessageDialog(proj.Frame, ex.Message, Strings.get("analyzeNoExpressionTitle"), JOptionPane.INFORMATION_MESSAGE);
			}

			// As a backup measure, we compute a truth table.
			Analyze.computeTable(analyzer.Model, proj, circuit, pinNames);
			analyzer.SelectedTab = Analyzer.TABLE_TAB;
		}

		private static void analyzeError(Project proj, string message)
		{
			JOptionPane.showMessageDialog(proj.Frame, message, Strings.get("analyzeErrorTitle"), JOptionPane.ERROR_MESSAGE);
			return;
		}
	}

}
