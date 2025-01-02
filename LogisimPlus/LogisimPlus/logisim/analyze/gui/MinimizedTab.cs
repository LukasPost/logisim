// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.analyze.gui
{


	using AnalyzerModel = logisim.analyze.model.AnalyzerModel;
	using OutputExpressions = logisim.analyze.model.OutputExpressions;
	using OutputExpressionsEvent = logisim.analyze.model.OutputExpressionsEvent;
	using OutputExpressionsListener = logisim.analyze.model.OutputExpressionsListener;

	internal class MinimizedTab : AnalyzerTab
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class FormatModel : AbstractListModel<string>, ComboBoxModel<string>
		{
			internal static int getFormatIndex(int choice)
			{
				switch (choice)
				{
				case AnalyzerModel.FORMAT_PRODUCT_OF_SUMS:
					return 1;
				default:
					return 0;
				}
			}

			internal string[] choices;
			internal int selected;

			internal FormatModel()
			{
				selected = 0;
				choices = new string[2];
				localeChanged();
			}

			internal virtual void localeChanged()
			{
				choices[0] = Strings.get("minimizedSumOfProducts");
				choices[1] = Strings.get("minimizedProductOfSums");
				fireContentsChanged(this, 0, choices.Length);
			}

			internal virtual int SelectedFormat
			{
				get
				{
					switch (selected)
					{
					case 1:
						return AnalyzerModel.FORMAT_PRODUCT_OF_SUMS;
					default:
						return AnalyzerModel.FORMAT_SUM_OF_PRODUCTS;
					}
				}
			}

			public virtual int Size
			{
				get
				{
					return choices.Length;
				}
			}

			public virtual string getElementAt(int index)
			{
				return choices[index];
			}

			public virtual string SelectedItem
			{
				get
				{
					return choices[selected];
				}
				set
				{
					for (int i = 0; i < choices.Length; i++)
					{
						if (choices[i].Equals(value))
						{
							selected = i;
						}
					}
				}
			}

		}

		private class MyListener : OutputExpressionsListener, ActionListener, ItemListener
		{
			private readonly MinimizedTab outerInstance;

			public MyListener(MinimizedTab outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void expressionChanged(OutputExpressionsEvent @event)
			{
				string output = outerInstance.CurrentVariable;
				if (@event.Type == OutputExpressionsEvent.OUTPUT_MINIMAL && @event.Variable.Equals(output))
				{
					outerInstance.minimizedExpr.Expression = outerInstance.outputExprs.getMinimalExpression(output);
					outerInstance.validate();
				}
				outerInstance.setAsExpr.setEnabled(!string.ReferenceEquals(output, null) && !outerInstance.outputExprs.isExpressionMinimal(output));
				int format = outerInstance.outputExprs.getMinimizedFormat(output);
				outerInstance.formatChoice.setSelectedIndex(FormatModel.getFormatIndex(format));
			}

			public virtual void actionPerformed(ActionEvent @event)
			{
				string output = outerInstance.CurrentVariable;
				int format = outerInstance.outputExprs.getMinimizedFormat(output);
				outerInstance.formatChoice.setSelectedIndex(FormatModel.getFormatIndex(format));
				outerInstance.outputExprs.setExpression(output, outerInstance.outputExprs.getMinimalExpression(output));
			}

			public virtual void itemStateChanged(ItemEvent @event)
			{
				if (@event.getSource() == outerInstance.formatChoice)
				{
					string output = outerInstance.CurrentVariable;
					FormatModel model = (FormatModel) outerInstance.formatChoice.getModel();
					outerInstance.outputExprs.setMinimizedFormat(output, model.SelectedFormat);
				}
				else
				{
					outerInstance.updateTab();
				}
			}
		}

		private OutputSelector selector;
		private KarnaughMapPanel karnaughMap;
		private JLabel formatLabel = new JLabel();
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: private javax.swing.JComboBox<?> formatChoice = new javax.swing.JComboBox<>(new FormatModel());
		private JComboBox<object> formatChoice = new JComboBox<object>(new FormatModel());
		private ExpressionView minimizedExpr = new ExpressionView();
		private JButton setAsExpr = new JButton();

		private MyListener myListener;
		private OutputExpressions outputExprs;

		public MinimizedTab(AnalyzerModel model)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.outputExprs = model.OutputExpressions;
			outputExprs.addOutputExpressionsListener(myListener);

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
			addRow(gb, gc, selector.Label, selector.ComboBox);
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

			string selected = selector.SelectedOutput;
			setAsExpr.setEnabled(!string.ReferenceEquals(selected, null) && !outputExprs.isExpressionMinimal(selected));
		}

		private void addRow<T1>(GridBagLayout gb, GridBagConstraints gc, JLabel label, JComboBox<T1> choice)
		{
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

		internal override void localeChanged()
		{
			selector.localeChanged();
			karnaughMap.localeChanged();
			minimizedExpr.localeChanged();
			setAsExpr.setText(Strings.get("minimizedSetButton"));
			formatLabel.setText(Strings.get("minimizedFormat"));
			((FormatModel) formatChoice.getModel()).localeChanged();
		}

		internal override void updateTab()
		{
			string output = CurrentVariable;
			karnaughMap.Output = output;
			int format = outputExprs.getMinimizedFormat(output);
			formatChoice.setSelectedIndex(FormatModel.getFormatIndex(format));
			minimizedExpr.Expression = outputExprs.getMinimalExpression(output);
			setAsExpr.setEnabled(!string.ReferenceEquals(output, null) && !outputExprs.isExpressionMinimal(output));
		}

		private string CurrentVariable
		{
			get
			{
				return selector.SelectedOutput;
			}
		}
	}

}
