/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.gui.log;

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.WindowEvent;
import java.util.HashMap;
import java.util.Map;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JTabbedPane;

import logisim.circuit.CircuitState;
import logisim.circuit.Simulator;
import logisim.circuit.SimulatorEvent;
import logisim.circuit.SimulatorListener;
import logisim.file.LibraryEvent;
import logisim.file.LibraryListener;
import logisim.gui.generic.LFrame;
import logisim.gui.menu.LogisimMenuBar;
import logisim.proj.Project;
import logisim.proj.ProjectEvent;
import logisim.proj.ProjectListener;
import logisim.util.LocaleListener;
import logisim.util.LocaleManager;
import logisim.util.StringUtil;
import logisim.util.WindowMenuItemManager;

public class LogFrame extends LFrame {
	// TODO should automatically repaint icons when component attr change
	// TODO ? moving a component using Select tool removes it from selection
	private class WindowMenuManager extends WindowMenuItemManager
			implements LocaleListener, ProjectListener, LibraryListener {
		WindowMenuManager() {
			super(Strings.get("logFrameMenuItem"), false);
			project.addProjectListener(this);
			project.addLibraryListener(this);
		}

		@Override
		public JFrame getJFrame(boolean create) {
			return LogFrame.this;
		}

		public void localeChanged() {
			String title = project.getLogisimFile().getDisplayName();
			setText(StringUtil.format(Strings.get("logFrameMenuItem"), title));
		}

		public void projectChanged(ProjectEvent event) {
			if (event.getAction() == ProjectEvent.ACTION_SET_FILE) localeChanged();
		}

		public void libraryChanged(LibraryEvent event) {
			if (event.getAction() == LibraryEvent.SET_NAME) localeChanged();
		}
	}

	private class MyListener
			implements ActionListener, ProjectListener, LibraryListener, SimulatorListener, LocaleListener {
		public void actionPerformed(ActionEvent event) {
			Object src = event.getSource();
			if (src == close) {
				WindowEvent e = new WindowEvent(LogFrame.this, WindowEvent.WINDOW_CLOSING);
				processWindowEvent(e);
			}
		}

		public void projectChanged(ProjectEvent event) {
			int action = event.getAction();
			if (action == ProjectEvent.ACTION_SET_STATE)
				setSimulator(event.getProject().getSimulator(), event.getProject().getCircuitState());
			else if (action == ProjectEvent.ACTION_SET_FILE) setTitle(computeTitle(curModel, project));
		}

		public void libraryChanged(LibraryEvent event) {
			int action = event.getAction();
			if (action == LibraryEvent.SET_NAME) setTitle(computeTitle(curModel, project));
		}

		public void localeChanged() {
			setTitle(computeTitle(curModel, project));
			for (int i = 0; i < panels.length; i++) {
				tabbedPane.setTitleAt(i, panels[i].getTitle());
				tabbedPane.setToolTipTextAt(i, panels[i].getToolTipText());
				panels[i].localeChanged();
			}
			close.setText(Strings.get("closeButton"));
			windowManager.localeChanged();
		}

		public void propagationCompleted(SimulatorEvent e) {
			curModel.propagationCompleted();
		}

		public void tickCompleted(SimulatorEvent e) {
		}

		public void simulatorStateChanged(SimulatorEvent e) {
		}
	}

	private Project project;
	private Simulator curSimulator;
	private Model curModel;
	private Map<CircuitState, Model> modelMap = new HashMap<>();
	private MyListener myListener = new MyListener();
	private WindowMenuManager windowManager;

	private LogPanel[] panels;
	private JTabbedPane tabbedPane;
	private JButton close = new JButton();

	public LogFrame(Project project) {
		this.project = project;
		windowManager = new WindowMenuManager();
		project.addProjectListener(myListener);
		project.addLibraryListener(myListener);
		setDefaultCloseOperation(HIDE_ON_CLOSE);
		setJMenuBar(new LogisimMenuBar(this, project));
		setSimulator(project.getSimulator(), project.getCircuitState());

		panels = new LogPanel[] { new SelectionPanel(this), new ScrollPanel(this), new FilePanel(this), };
		tabbedPane = new JTabbedPane();
		for (LogPanel panel : panels) tabbedPane.addTab(panel.getTitle(), null, panel, panel.getToolTipText());

		JPanel buttonPanel = new JPanel();
		buttonPanel.add(close);
		close.addActionListener(myListener);

		Container contents = getContentPane();
		tabbedPane.setPreferredSize(new Dimension(450, 300));
		contents.add(tabbedPane, BorderLayout.CENTER);
		contents.add(buttonPanel, BorderLayout.SOUTH);

		LocaleManager.addLocaleListener(myListener);
		myListener.localeChanged();
		pack();
	}

	public Project getProject() {
		return project;
	}

	Model getModel() {
		return curModel;
	}

	private void setSimulator(Simulator value, CircuitState state) {
		if ((value == null) == (curModel == null))
			if (value == null || value.getCircuitState() == curModel.getCircuitState())
				return;

		LogisimMenuBar menubar = (LogisimMenuBar) getJMenuBar();
		menubar.setCircuitState(value, state);

		if (curSimulator != null)
			curSimulator.removeSimulatorListener(myListener);
		if (curModel != null)
			curModel.setSelected(false);

		Model oldModel = curModel;
		Model data = null;
		if (value != null) {
			data = modelMap.get(value.getCircuitState());
			if (data == null) {
				data = new Model(value.getCircuitState());
				modelMap.put(data.getCircuitState(), data);
			}
		}
		curSimulator = value;
		curModel = data;

		if (curSimulator != null)
			curSimulator.addSimulatorListener(myListener);
		if (curModel != null)
			curModel.setSelected(true);
		setTitle(computeTitle(curModel, project));
		if (panels != null) for (LogPanel panel : panels) panel.modelChanged(oldModel, curModel);
	}

	@Override
	public void setVisible(boolean value) {
		if (value) windowManager.frameOpened(this);
		super.setVisible(value);
	}

	LogPanel[] getPrefPanels() {
		return panels;
	}

	private static String computeTitle(Model data, Project proj) {
		String name = data == null ? "???" : data.getCircuitState().getCircuit().getName();
		return StringUtil.format(Strings.get("logFrameTitle"), name, proj.getLogisimFile().getDisplayName());
	}
}
