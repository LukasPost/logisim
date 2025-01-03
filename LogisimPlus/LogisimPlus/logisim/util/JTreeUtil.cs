// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{


	/* This comes from "Denis" at http://forum.java.sun.com/thread.jspa?forumID=57&threadID=296255 */

	/*
	 * My program use 4 classes. The application displays two trees.
	 * You can drag (move by default or copy with ctrl pressed) a node from the left tree to the right one, from the right to the left and inside the same tree.
	 * The rules for moving are :
	 *   - you can't move the root
	 *   - you can't move the selected node to its subtree (in the same tree).
	 *   - you can't move the selected node to itself (in the same tree).
	 *   - you can't move the selected node to its parent (in the same tree).
	 *   - you can move a node to anywhere you want according to the 4 previous rules.
	 *  The rules for copying are :
	 *   - you can copy a node to anywhere you want.
	 *
	 * In the implementation I used DnD version of Java 1.3 because in 1.4 the DnD is too restrictive :
	 * you can't do what you want (displaying the image of the node while dragging, changing the cursor
	 * according to where you are dragging, etc...). In 1.4, the DnD is based on the 1.3 version but
	 * it is too encapsulated.
	 */

	public class JTreeUtil
	{
		private static readonly Insets DEFAULT_INSETS = new Insets(20, 20, 20, 20);
		private static readonly DataFlavor NODE_FLAVOR = new DataFlavor(DataFlavor.javaJVMLocalObjectMimeType, "Node");

		private static object draggedNode;
		private static BufferedImage image = null; // buff image

		private class TransferableNode : Transferable
		{
			internal object node;
			internal DataFlavor[] flavors = new DataFlavor[] {NODE_FLAVOR};

			public TransferableNode(object nd)
			{
				node = nd;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public synchronized Object getTransferData(java.awt.datatransfer.DataFlavor flavor) throws java.awt.datatransfer.UnsupportedFlavorException
			public virtual object getTransferData(DataFlavor flavor)
			{
				lock (this)
				{
					if (flavor == NODE_FLAVOR)
					{
						return node;
					}
					else
					{
						throw new UnsupportedFlavorException(flavor);
					}
				}
			}

			public virtual DataFlavor[] TransferDataFlavors
			{
				get
				{
					return flavors;
				}
			}

			public virtual bool isDataFlavorSupported(DataFlavor flavor)
			{
				return flavors.contains(flavor);
			}
		}

		/*
		 * This class is the most important. It manages all the DnD behavior. It is abstract because it contains two
		 * abstract methods: public abstract boolean canPerformAction(JTree target, Object draggedNode, int action, Point
		 * location); public abstract boolean executeDrop(DNDTree tree, Object draggedNode, Object newParentNode, int
		 * action); we have to override to give the required behavior of DnD in your tree.
		 */
		private class TreeTransferHandler : DragGestureListener, DragSourceListener, DropTargetListener
		{
			internal JTree tree;
			internal JTreeDragController controller;
			internal DragSource dragSource; // dragsource
			internal Rectangle rect2D = new Rectangle();
			internal bool drawImage;

			protected internal TreeTransferHandler(JTree tree, JTreeDragController controller, int action, bool drawIcon)
			{
				this.tree = tree;
				this.controller = controller;
				drawImage = drawIcon;
				dragSource = new DragSource();
				dragSource.createDefaultDragGestureRecognizer(tree, action, this);
			}

			/* Methods for DragSourceListener */
			public virtual void dragDropEnd(DragSourceDropEvent dsde)
			{
				/*
				 * if (dsde.getDropSuccess() && dsde.getDropAction() == DnDConstants.ACTION_MOVE && draggedNodeParent !=
				 * null) { ((DefaultTreeModel) tree.getModel()) .nodeStructureChanged(draggedNodeParent); }
				 */
			}

			public void dragEnter(DragSourceDragEvent dsde)
			{
				int action = dsde.getDropAction();
				if (action == DnDConstants.ACTION_COPY)
				{
					dsde.getDragSourceContext().setCursor(DragSource.DefaultCopyDrop);
				}
				else
				{
					if (action == DnDConstants.ACTION_MOVE)
					{
						dsde.getDragSourceContext().setCursor(DragSource.DefaultMoveDrop);
					}
					else
					{
						dsde.getDragSourceContext().setCursor(DragSource.DefaultMoveNoDrop);
					}
				}
			}

			public void dragOver(DragSourceDragEvent dsde)
			{
				int action = dsde.getDropAction();
				if (action == DnDConstants.ACTION_COPY)
				{
					dsde.getDragSourceContext().setCursor(DragSource.DefaultCopyDrop);
				}
				else
				{
					if (action == DnDConstants.ACTION_MOVE)
					{
						dsde.getDragSourceContext().setCursor(DragSource.DefaultMoveDrop);
					}
					else
					{
						dsde.getDragSourceContext().setCursor(DragSource.DefaultMoveNoDrop);
					}
				}
			}

			public void dropActionChanged(DragSourceDragEvent dsde)
			{
				int action = dsde.getDropAction();
				if (action == DnDConstants.ACTION_COPY)
				{
					dsde.getDragSourceContext().setCursor(DragSource.DefaultCopyDrop);
				}
				else
				{
					if (action == DnDConstants.ACTION_MOVE)
					{
						dsde.getDragSourceContext().setCursor(DragSource.DefaultMoveDrop);
					}
					else
					{
						dsde.getDragSourceContext().setCursor(DragSource.DefaultMoveNoDrop);
					}
				}
			}

			public void dragExit(DragSourceEvent dse)
			{
				dse.getDragSourceContext().setCursor(DragSource.DefaultMoveNoDrop);
			}

			/* Methods for DragGestureListener */
			public void dragGestureRecognized(DragGestureEvent dge)
			{
				TreePath path = tree.getSelectionPath();
				if (path != null)
				{
					draggedNode = path.getLastPathComponent();
					if (drawImage)
					{
						Rectangle pathBounds = tree.getPathBounds(path); // getpathbounds
																			// of
																			// selectionpath
						JComponent lbl = (JComponent) tree.getCellRenderer().getTreeCellRendererComponent(tree, draggedNode, false, tree.isExpanded(path), tree.getModel().isLeaf(path.getLastPathComponent()), 0, false); // returning the label
						lbl.setBounds(pathBounds); // setting bounds to lbl
						image = new BufferedImage(lbl.getWidth(), lbl.getHeight(), BufferedImage.TYPE_INT_ARGB_PRE); // buffered
																				// image
																				// reference
																				// passing
																				// the
																				// label's
																				// ht
																				// and
																				// width
						JGraphics2D JGraphics = image.createJGraphics(); // creating
																		// the
																		// JGraphics
																		// for
																		// buffered
																		// image
						JGraphics.setComposite(AlphaComposite.getInstance(AlphaComposite.SRC_OVER, 0.5f)); // Sets the
																											// Composite for
																											// the
																											// JGraphics2D
																											// context
						lbl.setOpaque(false);
						lbl.paint(JGraphics); // painting the JGraphics to label
						JGraphics.dispose();
					}
					dragSource.startDrag(dge, DragSource.DefaultMoveNoDrop, image, new Point(0, 0), new TransferableNode(draggedNode), this);
				}
			}

			/* Methods for DropTargetListener */

			public void dragEnter(DropTargetDragEvent dtde)
			{
				Point pt = dtde.getLocation();
				int action = dtde.getDropAction();
				if (drawImage)
				{
					paintImage(pt);
				}
				if (controller.canPerformAction(tree, draggedNode, action, pt))
				{
					dtde.acceptDrag(action);
				}
				else
				{
					dtde.rejectDrag();
				}
			}

			public void dragExit(DropTargetEvent dte)
			{
				if (drawImage)
				{
					clearImage();
				}
			}

			public void dragOver(DropTargetDragEvent dtde)
			{
				Point pt = dtde.getLocation();
				int action = dtde.getDropAction();
				autoscroll(tree, pt);
				if (drawImage)
				{
					paintImage(pt);
				}
				if (controller.canPerformAction(tree, draggedNode, action, pt))
				{
					dtde.acceptDrag(action);
				}
				else
				{
					dtde.rejectDrag();
				}
			}

			public void dropActionChanged(DropTargetDragEvent dtde)
			{
				Point pt = dtde.getLocation();
				int action = dtde.getDropAction();
				if (drawImage)
				{
					paintImage(pt);
				}
				if (controller.canPerformAction(tree, draggedNode, action, pt))
				{
					dtde.acceptDrag(action);
				}
				else
				{
					dtde.rejectDrag();
				}
			}

			public void drop(DropTargetDropEvent dtde)
			{
				try
				{
					if (drawImage)
					{
						clearImage();
					}
					int action = dtde.getDropAction();
					Transferable transferable = dtde.getTransferable();
					Point pt = dtde.getLocation();
					if (transferable.isDataFlavorSupported(NODE_FLAVOR) && controller.canPerformAction(tree, draggedNode, action, pt))
					{
						TreePath pathTarget = tree.getPathForLocation(pt.x, pt.y);
						object node = transferable.getTransferData(NODE_FLAVOR);
						object newParentNode = pathTarget.getLastPathComponent();
						if (controller.executeDrop(tree, node, newParentNode, action))
						{
							dtde.acceptDrop(action);
							dtde.dropComplete(true);
							return;
						}
					}
					dtde.rejectDrop();
					dtde.dropComplete(false);
				}
				catch (Exception)
				{
					dtde.rejectDrop();
					dtde.dropComplete(false);
				}
			}

			internal void paintImage(Point pt)
			{
				tree.paintImmediately(rect2D.getBounds());
				rect2D.setRect((int) pt.getX(), (int) pt.getY(), image.getWidth(), image.getHeight());
				tree.getJGraphics().drawImage(image, (int) pt.getX(), (int) pt.getY(), tree);
			}

			internal void clearImage()
			{
				tree.paintImmediately(rect2D.getBounds());
			}
		}

		public static void configureDragAndDrop(JTree tree, JTreeDragController controller)
		{
			tree.setAutoscrolls(true);
			new TreeTransferHandler(tree, controller, DnDConstants.ACTION_COPY_OR_MOVE, true);
		}

		private static void autoscroll(JTree tree, Point cursorLocation)
		{
			Insets insets = DEFAULT_INSETS;
			Rectangle outer = tree.getVisibleRect();
			Rectangle inner = new Rectangle(outer.X + insets.left, outer.Y + insets.top, outer.Width - (insets.left + insets.right), outer.Height - (insets.top + insets.bottom));
			if (!inner.contains(cursorLocation))
			{
				Rectangle scrollRect = new Rectangle(cursorLocation.x - insets.left, cursorLocation.y - insets.top, insets.left + insets.right, insets.top + insets.bottom);
				tree.scrollRectToVisible(scrollRect);
			}
		}
	}

}
