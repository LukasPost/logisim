/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.analyze.gui;

import javax.swing.JFrame;

import logisim.util.LocaleListener;
import logisim.util.LocaleManager;
import logisim.util.WindowMenuItemManager;

public class AnalyzerManager extends WindowMenuItemManager implements LocaleListener {
	public static void initialize() {
		analysisManager = new AnalyzerManager();
	}

	public static Analyzer getAnalyzer() {
		if (analysisWindow == null) {
			analysisWindow = new Analyzer();
			analysisWindow.pack();
			if (analysisManager != null)
				analysisManager.frameOpened(analysisWindow);
		}
		return analysisWindow;
	}

	private static Analyzer analysisWindow;
	private static AnalyzerManager analysisManager;

	private AnalyzerManager() {
		super(Strings.get("analyzerWindowTitle"), true);
		LocaleManager.addLocaleListener(this);
	}

	@Override
	public JFrame getJFrame(boolean create) {
		return create ? getAnalyzer() : analysisWindow;
	}

	public void localeChanged() {
		setText(Strings.get("analyzerWindowTitle"));
	}
}
