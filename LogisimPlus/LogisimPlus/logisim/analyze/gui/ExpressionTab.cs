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
	using OutputExpressionsEvent = logisim.analyze.model.OutputExpressionsEvent;
	using OutputExpressionsListener = logisim.analyze.model.OutputExpressionsListener;
	using Expression = logisim.analyze.model.Expression;
	using Parser = logisim.analyze.model.Parser;
	using ParserException = logisim.analyze.model.ParserException;
	using StringGetter = logisim.util.StringGetter;
    using LogisimPlus.Java;

    internal class ExpressionTab : AnalyzerTab, TabInterface
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
		}

		private class MyListener : AbstractAction, DocumentListener, OutputExpressionsListener, ItemListener
		{
			private readonly ExpressionTab outerInstance;

			public MyListener(ExpressionTab outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal bool edited = false;

			public virtual void actionPerformed(ActionEvent @event)
			{
				object src = @event.getSource();
				if (src == outerInstance.clear)
				{
					outerInstance.Error = null;
					outerInstance.field.setText("");
					outerInstance.field.grabFocus();
				}
				else if (src == outerInstance.revert)
				{
					outerInstance.Error = null;
					outerInstance.field.setText(CurrentString);
					outerInstance.field.grabFocus();
				}
				else if ((src == outerInstance.field || src == outerInstance.enter) && outerInstance.enter.isEnabled())
				{
					try
					{
						string exprString = outerInstance.field.getText();
						Expression expr = Parser.parse(outerInstance.field.getText(), outerInstance.model);
						outerInstance.Error = null;
						outerInstance.model.OutputExpressions.setExpression(outerInstance.CurrentVariable, expr, exprString);
						insertUpdate(null);
					}
					catch (ParserException ex)
					{
						outerInstance.Error = ex.MessageGetter;
						outerInstance.field.setCaretPosition(ex.Offset);
						outerInstance.field.moveCaretPosition(ex.EndOffset);
					}
					outerInstance.field.grabFocus();
				}
			}

			public virtual void insertUpdate(DocumentEvent @event)
			{
				string curText = outerInstance.field.getText();
				edited = curText.Length != outerInstance.curExprStringLength || !curText.Equals(CurrentString);

				bool enable = (edited && !string.ReferenceEquals(outerInstance.CurrentVariable, null));
				outerInstance.clear.setEnabled(curText.Length > 0);
				outerInstance.revert.setEnabled(enable);
				outerInstance.enter.setEnabled(enable);
			}

			public virtual void removeUpdate(DocumentEvent @event)
			{
				insertUpdate(@event);
			}

			public virtual void changedUpdate(DocumentEvent @event)
			{
				insertUpdate(@event);
			}

			public virtual void expressionChanged(OutputExpressionsEvent @event)
			{
				if (@event.Type == OutputExpressionsEvent.OUTPUT_EXPRESSION)
				{
					string output = @event.Variable;
					if (output.Equals(outerInstance.CurrentVariable))
					{
						outerInstance.prettyView.Expression = outerInstance.model.OutputExpressions.getExpression(output);
						currentStringChanged();
					}
				}
			}

			public virtual void itemStateChanged(ItemEvent @event)
			{
				outerInstance.updateTab();
			}

			internal virtual string CurrentString
			{
				get
				{
					string output = outerInstance.CurrentVariable;
					return string.ReferenceEquals(output, null) ? "" : outerInstance.model.OutputExpressions.getExpressionString(output);
				}
			}

			internal virtual void currentStringChanged()
			{
				string output = outerInstance.CurrentVariable;
				string exprString = outerInstance.model.OutputExpressions.getExpressionString(output);
				outerInstance.curExprStringLength = exprString.Length;
				if (!edited)
				{
					outerInstance.Error = null;
					outerInstance.field.setText(CurrentString);
				}
				else
				{
					insertUpdate(null);
				}
			}
		}

		private OutputSelector selector;
		private ExpressionView prettyView = new ExpressionView();
		private JTextArea field = new JTextArea(4, 25);
		private JButton clear = new JButton();
		private JButton revert = new JButton();
		private JButton enter = new JButton();
		private JLabel error = new JLabel();

		private MyListener myListener;
		private AnalyzerModel model;
		private int curExprStringLength = 0;
		private StringGetter errorMessage;

		public ExpressionTab(AnalyzerModel model)
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			this.model = model;
			selector = new OutputSelector(model);

			model.OutputExpressions.addOutputExpressionsListener(myListener);
			selector.addItemListener(myListener);
			clear.addActionListener(myListener);
			revert.addActionListener(myListener);
			enter.addActionListener(myListener);
			field.setLineWrap(true);
			field.setWrapStyleWord(true);
			field.getInputMap().put(KeyStroke.getKeyStroke(KeyEvent.VK_ENTER, 0), myListener);
			field.getDocument().addDocumentListener(myListener);
			field.setFont(new Font("Monospaced", Font.PLAIN, 14));

			JPanel buttons = new JPanel();
			buttons.add(clear);
			buttons.add(revert);
			buttons.add(enter);

			GridBagLayout gb = new GridBagLayout();
			GridBagConstraints gc = new GridBagConstraints();
			setLayout(gb);
			gc.weightx = 1.0;
			gc.gridx = 0;
			gc.gridy = GridBagConstraints.RELATIVE;
			gc.fill = GridBagConstraints.BOTH;

			JPanel selectorPanel = selector.createPanel();
			gb.setConstraints(selectorPanel, gc);
			add(selectorPanel);
			gb.setConstraints(prettyView, gc);
			add(prettyView);
			Insets oldInsets = gc.insets;
			gc.insets = new Insets(10, 10, 0, 10);
			JScrollPane fieldPane = new JScrollPane(field, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			gb.setConstraints(fieldPane, gc);
			add(fieldPane);
			gc.insets = oldInsets;
			gc.fill = GridBagConstraints.NONE;
			gc.anchor = GridBagConstraints.LINE_END;
			gb.setConstraints(buttons, gc);
			add(buttons);
			gc.fill = GridBagConstraints.BOTH;
			gb.setConstraints(error, gc);
			add(error);

			myListener.insertUpdate(null);
			Error = null;
		}

		internal override void localeChanged()
		{
			selector.localeChanged();
			prettyView.localeChanged();
			clear.setText(Strings.get("exprClearButton"));
			revert.setText(Strings.get("exprRevertButton"));
			enter.setText(Strings.get("exprEnterButton"));
			if (errorMessage != null)
			{
				error.setText(errorMessage.get());
			}
		}

		internal override void updateTab()
		{
			string output = CurrentVariable;
			prettyView.Expression = model.OutputExpressions.getExpression(output);
			myListener.currentStringChanged();
		}

		internal virtual void registerDefaultButtons(DefaultRegistry registry)
		{
			registry.registerDefaultButton(field, enter);
		}

		internal virtual string CurrentVariable
		{
			get
			{
				return selector.SelectedOutput;
			}
		}

		private StringGetter Error
		{
			set
			{
				if (value == null)
				{
					errorMessage = null;
					error.setText(" ");
				}
				else
				{
					errorMessage = value;
					error.setText(value.get());
				}
			}
		}

		public virtual void copy()
		{
			field.requestFocus();
			field.copy();
		}

		public virtual void paste()
		{
			field.requestFocus();
			field.paste();
		}

		public virtual void delete()
		{
			field.requestFocus();
			field.replaceSelection("");
		}

		public virtual void selectAll()
		{
			field.requestFocus();
			field.selectAll();
		}
	}
}
