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
	using LFrame = logisim.gui.generic.LFrame;
	using LogisimMenuBar = logisim.gui.menu.LogisimMenuBar;
	using LocaleListener = logisim.util.LocaleListener;
	using LocaleManager = logisim.util.LocaleManager;

	public class Analyzer : LFrame
	{
		private bool instanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			myListener = new MyListener(this);
			editListener = new EditListener(this);
		}

		// used by circuit analysis to select the relevant tab automatically.
		public const int INPUTS_TAB = 0;
		public const int OUTPUTS_TAB = 1;
		public const int TABLE_TAB = 2;
		public const int EXPRESSION_TAB = 3;
		public const int MINIMIZED_TAB = 4;

		private class MyListener : LocaleListener
		{
			private readonly Analyzer outerInstance;

			public MyListener(Analyzer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void localeChanged()
			{
				outerInstance.setTitle(Strings.get("analyzerWindowTitle"));
				outerInstance.tabbedPane.setTitleAt(INPUTS_TAB, Strings.get("inputsTab"));
				outerInstance.tabbedPane.setTitleAt(OUTPUTS_TAB, Strings.get("outputsTab"));
				outerInstance.tabbedPane.setTitleAt(TABLE_TAB, Strings.get("tableTab"));
				outerInstance.tabbedPane.setTitleAt(EXPRESSION_TAB, Strings.get("expressionTab"));
				outerInstance.tabbedPane.setTitleAt(MINIMIZED_TAB, Strings.get("minimizedTab"));
				outerInstance.tabbedPane.setToolTipTextAt(INPUTS_TAB, Strings.get("inputsTabTip"));
				outerInstance.tabbedPane.setToolTipTextAt(OUTPUTS_TAB, Strings.get("outputsTabTip"));
				outerInstance.tabbedPane.setToolTipTextAt(TABLE_TAB, Strings.get("tableTabTip"));
				outerInstance.tabbedPane.setToolTipTextAt(EXPRESSION_TAB, Strings.get("expressionTabTip"));
				outerInstance.tabbedPane.setToolTipTextAt(MINIMIZED_TAB, Strings.get("minimizedTabTip"));
				outerInstance.buildCircuit.setText(Strings.get("buildCircuitButton"));
				outerInstance.inputsPanel.localeChanged();
				outerInstance.outputsPanel.localeChanged();
				outerInstance.truthTablePanel.localeChanged();
				outerInstance.expressionPanel.localeChanged();
				outerInstance.minimizedPanel.localeChanged();
				outerInstance.buildCircuit.localeChanged();
			}
		}

		private class EditListener : ActionListener, ChangeListener
		{
			private readonly Analyzer outerInstance;

			public EditListener(Analyzer outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal virtual void register(LogisimMenuBar menubar)
			{
				menubar.addActionListener(LogisimMenuBar.CUT, this);
				menubar.addActionListener(LogisimMenuBar.COPY, this);
				menubar.addActionListener(LogisimMenuBar.PASTE, this);
				menubar.addActionListener(LogisimMenuBar.DELETE, this);
				menubar.addActionListener(LogisimMenuBar.SELECT_ALL, this);
				outerInstance.tabbedPane.addChangeListener(this);
				enableItems(menubar);
			}

			public virtual void actionPerformed(ActionEvent e)
			{
				object src = e.getSource();
				Component c = outerInstance.tabbedPane.getSelectedComponent();
				if (c is JScrollPane)
				{
					c = ((JScrollPane) c).getViewport().getView();
				}
				if (!(c is TabInterface))
				{
					return;
				}
				TabInterface tab = (TabInterface) c;
				if (src == LogisimMenuBar.CUT)
				{
					tab.copy();
					tab.delete();
				}
				else if (src == LogisimMenuBar.COPY)
				{
					tab.copy();
				}
				else if (src == LogisimMenuBar.PASTE)
				{
					tab.paste();
				}
				else if (src == LogisimMenuBar.DELETE)
				{
					tab.delete();
				}
				else if (src == LogisimMenuBar.SELECT_ALL)
				{
					tab.selectAll();
				}
			}

			internal virtual void enableItems(LogisimMenuBar menubar)
			{
				Component c = outerInstance.tabbedPane.getSelectedComponent();
				if (c is JScrollPane)
				{
					c = ((JScrollPane) c).getViewport().getView();
				}
				bool support = c is TabInterface;
				menubar.setEnabled(LogisimMenuBar.CUT, support);
				menubar.setEnabled(LogisimMenuBar.COPY, support);
				menubar.setEnabled(LogisimMenuBar.PASTE, support);
				menubar.setEnabled(LogisimMenuBar.DELETE, support);
				menubar.setEnabled(LogisimMenuBar.SELECT_ALL, support);
			}

			public virtual void stateChanged(ChangeEvent e)
			{
				enableItems((LogisimMenuBar) getJMenuBar());

				object selected = outerInstance.tabbedPane.getSelectedComponent();
				if (selected is JScrollPane)
				{
					selected = ((JScrollPane) selected).getViewport().getView();
				}
				if (selected is AnalyzerTab)
				{
					((AnalyzerTab) selected).updateTab();
				}
			}
		}

		private MyListener myListener;
		private EditListener editListener;
		private AnalyzerModel model = new AnalyzerModel();
		private JTabbedPane tabbedPane = new JTabbedPane();

		private VariableTab inputsPanel;
		private VariableTab outputsPanel;
		private TableTab truthTablePanel;
		private ExpressionTab expressionPanel;
		private MinimizedTab minimizedPanel;
		private BuildCircuitButton buildCircuit;

		internal Analyzer()
		{
			if (!instanceFieldsInitialized)
			{
				InitializeInstanceFields();
				instanceFieldsInitialized = true;
			}
			inputsPanel = new VariableTab(model.Inputs);
			outputsPanel = new VariableTab(model.Outputs);
			truthTablePanel = new TableTab(model.TruthTable);
			expressionPanel = new ExpressionTab(model);
			minimizedPanel = new MinimizedTab(model);
			buildCircuit = new BuildCircuitButton(this, model);

			truthTablePanel.addMouseListener(new TruthTableMouseListener());

			tabbedPane = new JTabbedPane();
			addTab(INPUTS_TAB, inputsPanel);
			addTab(OUTPUTS_TAB, outputsPanel);
			addTab(TABLE_TAB, truthTablePanel);
			addTab(EXPRESSION_TAB, expressionPanel);
			addTab(MINIMIZED_TAB, minimizedPanel);

			Container contents = getContentPane();
			JPanel vertStrut = new JPanel(null);
			vertStrut.setPreferredSize(new Size(0, 300));
			JPanel horzStrut = new JPanel(null);
			horzStrut.setPreferredSize(new Size(450, 0));
			JPanel buttonPanel = new JPanel();
			buttonPanel.add(buildCircuit);
			contents.add(vertStrut, BorderLayout.WEST);
			contents.add(horzStrut, BorderLayout.NORTH);
			contents.add(tabbedPane, BorderLayout.CENTER);
			contents.add(buttonPanel, BorderLayout.SOUTH);

			DefaultRegistry registry = new DefaultRegistry(getRootPane());
			inputsPanel.registerDefaultButtons(registry);
			outputsPanel.registerDefaultButtons(registry);
			expressionPanel.registerDefaultButtons(registry);

			LocaleManager.addLocaleListener(myListener);
			myListener.localeChanged();

			LogisimMenuBar menubar = new LogisimMenuBar(this, null);
			setJMenuBar(menubar);
			editListener.register(menubar);
		}

		private void addTab(int index, in JComponent comp)
		{
// JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
// ORIGINAL LINE: final javax.swing.JScrollPane pane = new javax.swing.JScrollPane(comp, javax.swing.ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, javax.swing.ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			JScrollPane pane = new JScrollPane(comp, ScrollPaneConstants.VERTICAL_SCROLLBAR_ALWAYS, ScrollPaneConstants.HORIZONTAL_SCROLLBAR_NEVER);
			if (comp is TableTab)
			{
				pane.setVerticalScrollBar(((TableTab) comp).VerticalScrollBar);
			}
			pane.addComponentListener(new ComponentListenerAnonymousInnerClass(this, comp, pane));
			tabbedPane.insertTab("Untitled", null, pane, null, index);
		}

		private class ComponentListenerAnonymousInnerClass : ComponentListener
		{
			private readonly Analyzer outerInstance;

			private JComponent comp;
			private JScrollPane pane;

			public ComponentListenerAnonymousInnerClass(Analyzer outerInstance, JComponent comp, JScrollPane pane)
			{
				this.outerInstance = outerInstance;
				this.comp = comp;
				this.pane = pane;
			}

			public void componentResized(ComponentEvent @event)
			{
				int width = pane.getViewport().getWidth();
				comp.setSize(new Size(width, comp.getHeight()));
			}

			public void componentMoved(ComponentEvent arg0)
			{
			}

			public void componentShown(ComponentEvent arg0)
			{
			}

			public void componentHidden(ComponentEvent arg0)
			{
			}
		}

		public virtual AnalyzerModel Model
		{
			get
			{
				return model;
			}
		}

		public virtual int SelectedTab
		{
			set
			{
				object found = tabbedPane.getComponentAt(value);
				if (found is AnalyzerTab)
				{
					((AnalyzerTab) found).updateTab();
				}
				tabbedPane.setSelectedIndex(value);
			}
		}

		public static void Main(string[] args)
		{
			Analyzer frame = new Analyzer();
			frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
			frame.pack();
			frame.setVisible(true);
		}
	}

}
