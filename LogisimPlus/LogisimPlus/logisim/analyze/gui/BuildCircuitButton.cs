// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{


	using AnalyzerModel = logisim.analyze.model.AnalyzerModel;
	using Expression = logisim.analyze.model.Expression;
	using VariableList = logisim.analyze.model.VariableList;
	using Circuit = logisim.circuit.Circuit;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using LogisimFileActions = logisim.file.LogisimFileActions;
	using Project = logisim.proj.Project;
	using Projects = logisim.proj.Projects;
	using CircuitBuilder = logisim.std.gates.CircuitBuilder;
	using StringUtil = logisim.util.StringUtil;

	internal class BuildCircuitButton : JButton
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class ProjectItem
		{
			internal Project project;

			internal ProjectItem(Project project)
			{
				this.project = project;
			}

			public override string ToString()
			{
				return project.LogisimFile.DisplayName;
			}
		}

		private class DialogPanel : JPanel
		{
			private readonly BuildCircuitButton outerInstance;

			internal JLabel projectLabel = new JLabel();
			internal JComboBox<object> project;
			internal JLabel nameLabel = new JLabel();
			internal JTextField name = new JTextField(10);
			internal JCheckBox twoInputs = new JCheckBox();
			internal JCheckBox nands = new JCheckBox();

			internal DialogPanel(BuildCircuitButton outerInstance)
			{
				this.outerInstance = outerInstance;
				List<Project> projects = Projects.OpenProjects;
				object[] options = new object[projects.Count];
				object initialSelection = null;
				for (int i = 0; i < options.Length; i++)
				{
					Project proj = projects[i];
					options[i] = new ProjectItem(proj);
					if (proj == outerInstance.model.CurrentProject)
					{
						initialSelection = options[i];
					}
				}
				project = new JComboBox<object>(options);
				if (options.Length == 1)
				{
					project.setSelectedItem(options[0]);
					project.setEnabled(false);
				}
				else if (initialSelection != null)
				{
					project.setSelectedItem(initialSelection);
				}

				Circuit defaultCircuit = outerInstance.model.CurrentCircuit;
				if (defaultCircuit != null)
				{
					name.setText(defaultCircuit.Name);
					name.selectAll();
				}

				VariableList outputs = outerInstance.model.Outputs;
				bool enableNands = true;
				for (int i = 0; i < outputs.size(); i++)
				{
					string output = outputs.get(i);
					Expression expr = outerInstance.model.OutputExpressions.getExpression(output);
					if (expr != null && expr.containsXor())
					{
						enableNands = false;
						break;
					}
				}
				nands.setEnabled(enableNands);

				GridBagLayout gb = new GridBagLayout();
				GridBagConstraints gc = new GridBagConstraints();
				setLayout(gb);
				gc.anchor = GridBagConstraints.LINE_START;
				gc.fill = GridBagConstraints.NONE;

				gc.gridx = 0;
				gc.gridy = 0;
				gb.setConstraints(projectLabel, gc);
				add(projectLabel);
				gc.gridx = 1;
				gb.setConstraints(project, gc);
				add(project);
				gc.gridy++;
				gc.gridx = 0;
				gb.setConstraints(nameLabel, gc);
				add(nameLabel);
				gc.gridx = 1;
				gb.setConstraints(name, gc);
				add(name);
				gc.gridy++;
				gb.setConstraints(twoInputs, gc);
				add(twoInputs);
				gc.gridy++;
				gb.setConstraints(nands, gc);
				add(nands);

				projectLabel.setText(Strings.get("buildProjectLabel"));
				nameLabel.setText(Strings.get("buildNameLabel"));
				twoInputs.setText(Strings.get("buildTwoInputsLabel"));
				nands.setText(Strings.get("buildNandsLabel"));
			}
		}

		private class MyListener : ActionListener
		{
			private readonly BuildCircuitButton outerInstance;

			public MyListener(BuildCircuitButton outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				Project dest = null;
				string name = null;
				bool twoInputs = false;
				bool useNands = false;
				bool replace = false;

				bool ok = false;
				while (!ok)
				{
					DialogPanel dlog = new DialogPanel(outerInstance);
					int action = JOptionPane.showConfirmDialog(outerInstance.parent, dlog, Strings.get("buildDialogTitle"), JOptionPane.OK_CANCEL_OPTION, JOptionPane.QUESTION_MESSAGE);
					if (action != JOptionPane.OK_OPTION)
					{
						return;
					}

					ProjectItem projectItem = (ProjectItem) dlog.project.getSelectedItem();
					if (projectItem == null)
					{
						JOptionPane.showMessageDialog(outerInstance.parent, Strings.get("buildNeedProjectError"), Strings.get("buildDialogErrorTitle"), JOptionPane.ERROR_MESSAGE);
						continue;
					}
					dest = projectItem.project;

					name = dlog.name.getText().Trim();
					if (name.Equals(""))
					{
						JOptionPane.showMessageDialog(outerInstance.parent, Strings.get("buildNeedCircuitError"), Strings.get("buildDialogErrorTitle"), JOptionPane.ERROR_MESSAGE);
						continue;
					}

					if (dest.LogisimFile.getCircuit(name) != null)
					{
						int choice = JOptionPane.showConfirmDialog(outerInstance.parent, StringUtil.format(Strings.get("buildConfirmReplaceMessage"), name), Strings.get("buildConfirmReplaceTitle"), JOptionPane.YES_NO_OPTION);
						if (choice != JOptionPane.YES_OPTION)
						{
							continue;
						}
						replace = true;
					}

					twoInputs = dlog.twoInputs.isSelected();
					useNands = dlog.nands.isSelected();
					ok = true;
				}

				outerInstance.performAction(dest, name, replace, twoInputs, useNands);
			}
		}

		private MyListener myListener;
		private JFrame parent;
		private AnalyzerModel model;

		internal BuildCircuitButton(JFrame parent, AnalyzerModel model)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.parent = parent;
			this.model = model;
			addActionListener(myListener);
		}

		internal virtual void localeChanged()
		{
			setText(Strings.get("buildCircuitButton"));
		}

		private void performAction(Project dest, string name, bool replace, in bool twoInputs, in bool useNands)
		{
			if (replace)
			{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final logisim.circuit.Circuit circuit = dest.getLogisimFile().getCircuit(name);
				Circuit circuit = dest.LogisimFile.getCircuit(name);
				if (circuit == null)
				{
					JOptionPane.showMessageDialog(parent, "Internal error prevents replacing circuit.", "Internal Error", JOptionPane.ERROR_MESSAGE);
					return;
				}

				CircuitMutation xn = CircuitBuilder.build(circuit, model, twoInputs, useNands);
				dest.doAction(xn.toAction(Strings.getter("replaceCircuitAction")));
			}
			else
			{
				// add the circuit
				Circuit circuit = new Circuit(name);
				CircuitMutation xn = CircuitBuilder.build(circuit, model, twoInputs, useNands);
				xn.execute();
				dest.doAction(LogisimFileActions.addCircuit(circuit));
				dest.CurrentCircuit = circuit;
			}
		}
	}

}
