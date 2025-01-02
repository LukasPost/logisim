// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace draw.gui
{

	using Canvas = draw.canvas.Canvas;
	using CanvasObject = draw.model.CanvasObject;
	using Drawing = draw.model.Drawing;
	using Rectangle = draw.shapes.Rectangle;
	using DrawingAttributeSet = draw.tools.DrawingAttributeSet;
	using UndoLog = draw.undo.UndoLog;
	using UndoLogDispatcher = draw.undo.UndoLogDispatcher;
	using AttrTable = logisim.gui.generic.AttrTable;
	using HorizontalSplitPane = logisim.util.HorizontalSplitPane;
	using VerticalSplitPane = logisim.util.VerticalSplitPane;

	public class Main
	{
		public static void Main(string[] args)
		{
			DrawingAttributeSet attrs = new DrawingAttributeSet();
			Drawing model = new Drawing();
			CanvasObject rect = attrs.applyTo(new Rectangle(25, 25, 50, 50));
			model.addObjects(0, Collections.singleton(rect));

			showFrame(model, "Drawing 1");
			showFrame(model, "Drawing 2");
		}

		private static void showFrame(Drawing model, string title)
		{
			JFrame frame = new JFrame(title);
			DrawingAttributeSet attrs = new DrawingAttributeSet();

			Canvas canvas = new Canvas();
			Toolbar toolbar = new Toolbar(canvas, attrs);
			canvas.setModel(model, new UndoLogDispatcher(new UndoLog()));
			canvas.Tool = toolbar.DefaultTool;

			AttrTable table = new AttrTable(frame);
			AttrTableDrawManager manager = new AttrTableDrawManager(canvas, table, attrs);
			manager.attributesSelected();
			HorizontalSplitPane west = new HorizontalSplitPane(toolbar, table, 0.5);
			VerticalSplitPane all = new VerticalSplitPane(west, canvas, 0.3);

			frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
			frame.getContentPane().add(all, BorderLayout.CENTER);
			frame.pack();
			frame.setVisible(true);
		}
	}

}
