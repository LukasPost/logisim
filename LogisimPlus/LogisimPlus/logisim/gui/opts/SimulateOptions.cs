// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.opts
{


	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeOption = logisim.data.AttributeOption;
	using AttributeSet = logisim.data.AttributeSet;
	using Options = logisim.file.Options;
	using TableLayout = logisim.util.TableLayout;

	internal class SimulateOptions : OptionsPanel
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class MyListener : ActionListener, AttributeListener
		{
			private readonly SimulateOptions outerInstance;

			public MyListener(SimulateOptions outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				object source = @event.getSource();
				if (source == outerInstance.simLimit)
				{
					int? opt = (int?) outerInstance.simLimit.getSelectedItem();
					if (opt != null)
					{
						AttributeSet attrs = outerInstance.Options.AttributeSet;
						outerInstance.Project.doAction(OptionsActions.setAttribute(attrs, Options.sim_limit_attr, opt));
					}
				}
				else if (source == outerInstance.simRandomness)
				{
					AttributeSet attrs = outerInstance.Options.AttributeSet;
					int val = outerInstance.simRandomness.isSelected() ? Options.sim_rand_dflt : Convert.ToInt32(0);
					outerInstance.Project.doAction(OptionsActions.setAttribute(attrs, Options.sim_rand_attr, val));
				}
				else if (source == outerInstance.gateUndefined)
				{
					ComboOption opt = (ComboOption) outerInstance.gateUndefined.getSelectedItem();
					if (opt != null)
					{
						AttributeSet attrs = outerInstance.Options.AttributeSet;
						outerInstance.Project.doAction(OptionsActions.setAttribute(attrs, Options.ATTR_GATE_UNDEFINED, opt.Value));
					}
				}
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: logisim.data.Attribute<?> attr = e.getAttribute();
				Attribute attr = e.Attribute;
				object val = e.Value;
				if (attr == Options.sim_limit_attr)
				{
					loadSimLimit((int?) val);
				}
				else if (attr == Options.sim_rand_attr)
				{
					loadSimRandomness((int?) val);
				}
			}

			internal virtual void loadSimLimit(int? val)
			{
				int value = val.Value;
				ComboBoxModel<int> model = outerInstance.simLimit.getModel();
				for (int i = 0; i < model.getSize(); i++)
				{
					int? opt = (int?) model.getElementAt(i);
					if (opt.Value == value)
					{
						outerInstance.simLimit.setSelectedItem(opt);
					}
				}
			}

			internal virtual void loadGateUndefined(AttributeOption val)
			{
				ComboOption.setSelected(outerInstance.gateUndefined, val);
			}

			internal virtual void loadSimRandomness(int? val)
			{
				outerInstance.simRandomness.setSelected(val.Value > 0);
			}
		}

		private MyListener myListener;

		private JLabel simLimitLabel = new JLabel();
		private JComboBox<int> simLimit = new JComboBox<int>(new int?[] {Convert.ToInt32(200), Convert.ToInt32(500), Convert.ToInt32(1000), Convert.ToInt32(2000), Convert.ToInt32(5000), Convert.ToInt32(10000), Convert.ToInt32(20000), Convert.ToInt32(50000)});
		private JCheckBox simRandomness = new JCheckBox();
		private JLabel gateUndefinedLabel = new JLabel();
		private JComboBox<ComboOption> gateUndefined = new JComboBox<ComboOption>(new ComboOption[]
		{
			new ComboOption(Options.GATE_UNDEFINED_IGNORE),
			new ComboOption(Options.GATE_UNDEFINED_ERROR)
		});

		public SimulateOptions(OptionsFrame window) : base(window)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}

			JPanel simLimitPanel = new JPanel();
			simLimitPanel.add(simLimitLabel);
			simLimitPanel.add(simLimit);
			simLimit.addActionListener(myListener);

			JPanel gateUndefinedPanel = new JPanel();
			gateUndefinedPanel.add(gateUndefinedLabel);
			gateUndefinedPanel.add(gateUndefined);
			gateUndefined.addActionListener(myListener);

			simRandomness.addActionListener(myListener);

			setLayout(new TableLayout(1));
			add(simLimitPanel);
			add(gateUndefinedPanel);
			add(simRandomness);

			window.Options.AttributeSet.addAttributeListener(myListener);
			AttributeSet attrs = Options.AttributeSet;
			myListener.loadSimLimit(attrs.getValue(Options.sim_limit_attr));
			myListener.loadGateUndefined(attrs.getValue(Options.ATTR_GATE_UNDEFINED));
			myListener.loadSimRandomness(attrs.getValue(Options.sim_rand_attr));
		}

		public override string Title
		{
			get
			{
				return Strings.get("simulateTitle");
			}
		}

		public override string HelpText
		{
			get
			{
				return Strings.get("simulateHelp");
			}
		}

		public override void localeChanged()
		{
			simLimitLabel.setText(Strings.get("simulateLimit"));
			gateUndefinedLabel.setText(Strings.get("gateUndefined"));
			simRandomness.setText(Strings.get("simulateRandomness"));
		}
	}

}
