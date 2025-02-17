/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.opts;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.ComboBoxModel;
import javax.swing.JCheckBox;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;

import logisim.data.Attribute;
import logisim.data.AttributeEvent;
import logisim.data.AttributeListener;
import logisim.data.AttributeOption;
import logisim.data.AttributeSet;
import logisim.file.Options;
import logisim.util.TableLayout;

class SimulateOptions extends OptionsPanel {
	private class MyListener implements ActionListener, AttributeListener {
		public void actionPerformed(ActionEvent event) {
			Object source = event.getSource();
			if (source == simLimit) {
				Integer opt = (Integer) simLimit.getSelectedItem();
				if (opt != null) {
					AttributeSet attrs = getOptions().getAttributeSet();
					getProject().doAction(OptionsActions.setAttribute(attrs, Options.sim_limit_attr, opt));
				}
			} else if (source == simRandomness) {
				AttributeSet attrs = getOptions().getAttributeSet();
				int val = simRandomness.isSelected() ? Options.sim_rand_dflt : Integer.valueOf(0);
				getProject().doAction(OptionsActions.setAttribute(attrs, Options.sim_rand_attr, val));
			} else if (source == gateUndefined) {
				ComboOption opt = (ComboOption) gateUndefined.getSelectedItem();
				if (opt != null) {
					AttributeSet attrs = getOptions().getAttributeSet();
					getProject()
							.doAction(OptionsActions.setAttribute(attrs, Options.ATTR_GATE_UNDEFINED, opt.getValue()));
				}
			}
		}

		public void attributeListChanged(AttributeEvent e) {
		}

		public void attributeValueChanged(AttributeEvent e) {
			Attribute<?> attr = e.getAttribute();
			Object val = e.getValue();
			if (attr == Options.sim_limit_attr) loadSimLimit((Integer) val);
			else if (attr == Options.sim_rand_attr) loadSimRandomness((Integer) val);
		}

		private void loadSimLimit(Integer val) {
			int value = val;
			ComboBoxModel<Integer> model = simLimit.getModel();
			for (int i = 0; i < model.getSize(); i++) {
				Integer opt = model.getElementAt(i);
				if (opt == value) simLimit.setSelectedItem(opt);
			}
		}

		private void loadGateUndefined(AttributeOption val) {
			ComboOption.setSelected(gateUndefined, val);
		}

		private void loadSimRandomness(Integer val) {
			simRandomness.setSelected(val > 0);
		}
	}

	private JLabel simLimitLabel = new JLabel();
	private JComboBox<Integer> simLimit = new JComboBox<>(
			new Integer[] {200, 500, 1000, 2000,
					5000, 10000, 20000, 50000, });
	private JCheckBox simRandomness = new JCheckBox();
	private JLabel gateUndefinedLabel = new JLabel();
	private JComboBox<ComboOption> gateUndefined = new JComboBox<>(new ComboOption[] { new ComboOption(Options.GATE_UNDEFINED_IGNORE),
			new ComboOption(Options.GATE_UNDEFINED_ERROR) });

	public SimulateOptions(OptionsFrame window) {
		super(window);

		JPanel simLimitPanel = new JPanel();
		simLimitPanel.add(simLimitLabel);
		simLimitPanel.add(simLimit);
		MyListener myListener = new MyListener();
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

		window.getOptions().getAttributeSet().addAttributeListener(myListener);
		AttributeSet attrs = getOptions().getAttributeSet();
		myListener.loadSimLimit(attrs.getValue(Options.sim_limit_attr));
		myListener.loadGateUndefined(attrs.getValue(Options.ATTR_GATE_UNDEFINED));
		myListener.loadSimRandomness(attrs.getValue(Options.sim_rand_attr));
	}

	@Override
	public String getTitle() {
		return Strings.get("simulateTitle");
	}

	@Override
	public String getHelpText() {
		return Strings.get("simulateHelp");
	}

	@Override
	public void localeChanged() {
		simLimitLabel.setText(Strings.get("simulateLimit"));
		gateUndefinedLabel.setText(Strings.get("gateUndefined"));
		simRandomness.setText(Strings.get("simulateRandomness"));
	}
}
