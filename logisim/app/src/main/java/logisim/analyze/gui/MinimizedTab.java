/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.gui;

import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.GridLayout;
import java.awt.Insets;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.ItemEvent;
import java.awt.event.ItemListener;

import javax.swing.AbstractListModel;
import javax.swing.ComboBoxModel;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;

import logisim.analyze.model.AnalyzerModel;
import logisim.analyze.model.OutputExpressions;
import logisim.analyze.model.OutputExpressionsEvent;
import logisim.analyze.model.OutputExpressionsListener;

class MinimizedTab extends AnalyzerTab {
	private static class FormatModel extends AbstractListModel<String> implements ComboBoxModel<String> {
		static int getFormatIndex(int choice) {
			return choice == AnalyzerModel.FORMAT_PRODUCT_OF_SUMS ? 1 : 0;
		}

		private String[] choices;
		private int selected;

		private FormatModel() {
			selected = 0;
			choices = new String[2];
			localeChanged();
		}

		void localeChanged() {
			choices[0] = Strings.get("minimizedSumOfProducts");
			choices[1] = Strings.get("minimizedProductOfSums");
			fireContentsChanged(this, 0, choices.length);
		}

		int getSelectedFormat() {
			return selected == 1 ? AnalyzerModel.FORMAT_PRODUCT_OF_SUMS : AnalyzerModel.FORMAT_SUM_OF_PRODUCTS;
		}

		public int getSize() {
			return choices.length;
		}

		public String getElementAt(int index) {
			return choices[index];
		}

		public String getSelectedItem() {
			return choices[selected];
		}

		public void setSelectedItem(Object value) {
			for (int i = 0; i < choices.length; i++)
				if (choices[i].equals(value))
					selected = i;
		}
	}

	private class MyListener implements OutputExpressionsListener, ActionListener, ItemListener {
		public void expressionChanged(OutputExpressionsEvent event) {
			String output = getCurrentVariable();
			if (event.getType() == OutputExpressionsEvent.OUTPUT_MINIMAL && event.getVariable().equals(output)) {
				minimizedExpr.setExpression(outputExpressions.getMinimalExpression(output));
				validate();
			}
			setAsExpr.setEnabled(output != null && !outputExpressions.isExpressionMinimal(output));
			int format = outputExpressions.getMinimizedFormat(output);
			formatChoice.setSelectedIndex(FormatModel.getFormatIndex(format));
		}

		public void actionPerformed(ActionEvent event) {
			String output = getCurrentVariable();
			int format = outputExpressions.getMinimizedFormat(output);
			formatChoice.setSelectedIndex(FormatModel.getFormatIndex(format));
			outputExpressions.setExpression(output, outputExpressions.getMinimalExpression(output));
		}

		public void itemStateChanged(ItemEvent event) {
			if (event.getSource() == formatChoice) {
				String output = getCurrentVariable();
				FormatModel model = (FormatModel) formatChoice.getModel();
				outputExpressions.setMinimizedFormat(output, model.getSelectedFormat());
			} else
				updateTab();
		}
	}

	private OutputSelector selector;
	private KarnaughMapPanel karnaughMap;
	private JLabel formatLabel = new JLabel();
	private JComboBox<?> formatChoice = new JComboBox<>(new FormatModel());
	private ExpressionView minimizedExpr = new ExpressionView();
	private JButton setAsExpr = new JButton();

	private OutputExpressions outputExpressions;

	public MinimizedTab(AnalyzerModel model) {
		outputExpressions = model.getOutputExpressions();
		MyListener myListener = new MyListener();
		outputExpressions.addOutputExpressionsListener(myListener);

		selector = new OutputSelector(model);
		selector.addItemListener(myListener);
		karnaughMap = new KarnaughMapPanel(model);
		karnaughMap.addMouseListener(new TruthTableMouseListener());
		setAsExpr.addActionListener(myListener);
		formatChoice.addItemListener(myListener);

		JPanel buttons = new JPanel(new GridLayout(1, 1));
		buttons.add(setAsExpr);

		JPanel formatPanel = new JPanel();
		formatPanel.add(formatLabel);
		formatPanel.add(formatChoice);

		GridBagLayout gb = new GridBagLayout();
		GridBagConstraints gc = new GridBagConstraints();
		setLayout(gb);
		gc.gridx = 0;
		gc.gridy = 0;
		addRow(gb, gc, selector.getLabel(), selector.getComboBox());
		addRow(gb, gc, formatLabel, formatChoice);

		gc.weightx = 0.0;
		gc.gridx = 0;
		gc.gridwidth = 2;
		gc.gridy = GridBagConstraints.RELATIVE;
		gc.fill = GridBagConstraints.BOTH;
		gc.anchor = GridBagConstraints.CENTER;
		gb.setConstraints(karnaughMap, gc);
		add(karnaughMap);
		Insets oldInsets = gc.insets;
		gc.insets = new Insets(20, 0, 0, 0);
		gb.setConstraints(minimizedExpr, gc);
		add(minimizedExpr);
		gc.insets = oldInsets;
		gc.fill = GridBagConstraints.NONE;
		gb.setConstraints(buttons, gc);
		add(buttons);

		String selected = selector.getSelectedOutput();
		setAsExpr.setEnabled(selected != null && !outputExpressions.isExpressionMinimal(selected));
	}

	private void addRow(GridBagLayout gb, GridBagConstraints gc, JLabel label, JComboBox<?> choice) {
		Insets oldInsets = gc.insets;
		gc.weightx = 0.0;
		gc.gridx = 0;
		gc.fill = GridBagConstraints.HORIZONTAL;
		gc.anchor = GridBagConstraints.LINE_START;
		gc.insets = new Insets(5, 5, 5, 5);
		gb.setConstraints(label, gc);
		add(label);
		gc.gridx = 1;
		gc.fill = GridBagConstraints.VERTICAL;
		gb.setConstraints(choice, gc);
		add(choice);
		gc.gridy++;
		gc.insets = oldInsets;
	}

	void localeChanged() {
		selector.localeChanged();
		karnaughMap.localeChanged();
		minimizedExpr.localeChanged();
		setAsExpr.setText(Strings.get("minimizedSetButton"));
		formatLabel.setText(Strings.get("minimizedFormat"));
		((FormatModel) formatChoice.getModel()).localeChanged();
	}

	@Override
	void updateTab() {
		String output = getCurrentVariable();
		karnaughMap.setOutput(output);
		int format = outputExpressions.getMinimizedFormat(output);
		formatChoice.setSelectedIndex(FormatModel.getFormatIndex(format));
		minimizedExpr.setExpression(outputExpressions.getMinimalExpression(output));
		setAsExpr.setEnabled(output != null && !outputExpressions.isExpressionMinimal(output));
	}

	private String getCurrentVariable() {
		return selector.getSelectedOutput();
	}
}
