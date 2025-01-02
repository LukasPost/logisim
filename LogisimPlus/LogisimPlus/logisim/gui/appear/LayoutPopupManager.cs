// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.appear
{


	using SelectionEvent = draw.canvas.SelectionEvent;
	using SelectionListener = draw.canvas.SelectionListener;
	using CanvasObject = draw.model.CanvasObject;
	using CircuitState = logisim.circuit.CircuitState;
	using AppearancePort = logisim.circuit.appear.AppearancePort;
	using Location = logisim.data.Location;
	using CanvasPane = logisim.gui.generic.CanvasPane;
	using Instance = logisim.instance.Instance;

	internal class LayoutPopupManager : SelectionListener, MouseListener, MouseMotionListener
	{
		private CanvasPane canvasPane;
		private AppearanceCanvas canvas;
		private Popup curPopup;
		private long curPopupTime;
		private Location dragStart;

		public LayoutPopupManager(CanvasPane canvasPane, AppearanceCanvas canvas)
		{
			this.canvasPane = canvasPane;
			this.canvas = canvas;
			this.curPopup = null;
			this.dragStart = null;

			canvas.Selection.addSelectionListener(this);
			canvas.addMouseListener(this);
			canvas.addMouseMotionListener(this);
		}

		public virtual void hideCurrentPopup()
		{
			Popup cur = curPopup;
			if (cur != null)
			{
				curPopup = null;
				dragStart = null;
				cur.hide();
			}
		}

		public virtual void selectionChanged(SelectionEvent e)
		{
			int act = e.Action;
			if (act == SelectionEvent.ACTION_ADDED)
			{
				ISet<AppearancePort> ports = shouldShowPopup(e.Affected);
				if (ports == null)
				{
					hideCurrentPopup();
				}
				else
				{
					showPopup(ports);
				}
			}
		}

		private ISet<AppearancePort> shouldShowPopup(ICollection<CanvasObject> add)
		{
			bool found = false;
			foreach (CanvasObject o in add)
			{
				if (o is AppearancePort)
				{
					found = true;
					break;
				}
			}
			if (found)
			{
				ISet<AppearancePort> ports = SelectedPorts;
				if (ports.Count > 0 && isPortUnselected(ports))
				{
					return ports;
				}
			}
			return null;
		}

		// returns all the ports in the current selection
		private ISet<AppearancePort> SelectedPorts
		{
			get
			{
				HashSet<AppearancePort> ports = new HashSet<AppearancePort>();
				foreach (CanvasObject o in canvas.Selection.Selected)
				{
					if (o is AppearancePort)
					{
						ports.Add((AppearancePort) o);
					}
				}
				return ports;
			}
		}

		// returns true if the canvas contains any port not in the given set
		private bool isPortUnselected(ISet<AppearancePort> selected)
		{
			foreach (CanvasObject o in canvas.Model.ObjectsFromBottom)
			{
				if (o is AppearancePort)
				{
					if (!selected.Contains(o))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void showPopup(ISet<AppearancePort> portObjects)
		{
			dragStart = null;
			CircuitState circuitState = canvas.CircuitState;
			if (circuitState == null)
			{
				return;
			}
			List<Instance> ports = new List<Instance>(portObjects.Count);
			foreach (AppearancePort portObject in portObjects)
			{
				ports.Add(portObject.Pin);
			}

			hideCurrentPopup();
			LayoutThumbnail layout = new LayoutThumbnail();
			layout.setCircuit(circuitState, ports);
			JViewport owner = canvasPane.getViewport();
			Point ownerLoc = owner.getLocationOnScreen();
			Size ownerDim = owner.getSize();
			Size layoutDim = layout.getPreferredSize();
			int x = ownerLoc.x + Math.Max(0, ownerDim.width - layoutDim.width - 5);
			int y = ownerLoc.y + Math.Max(0, ownerDim.height - layoutDim.height - 5);
			PopupFactory factory = PopupFactory.getSharedInstance();
			Popup popup = factory.getPopup(canvasPane.getViewport(), layout, x, y);
			popup.show();
			curPopup = popup;
			curPopupTime = DateTimeHelper.CurrentUnixTimeMillis();
		}

		public virtual void mouseClicked(MouseEvent e)
		{
		}

		public virtual void mouseEntered(MouseEvent e)
		{
			hideCurrentPopup();
		}

		public virtual void mouseExited(MouseEvent e)
		{
			long sincePopup = DateTimeHelper.CurrentUnixTimeMillis() - curPopupTime;
			if (sincePopup > 50)
			{
				hideCurrentPopup();
			}
		}

		public virtual void mousePressed(MouseEvent e)
		{
			long sincePopup = DateTimeHelper.CurrentUnixTimeMillis() - curPopupTime;
			if (sincePopup > 50)
			{
				hideCurrentPopup();
			}
			dragStart = new Location(e.getX(), e.getY());
		}

		public virtual void mouseReleased(MouseEvent e)
		{
		}

		public virtual void mouseDragged(MouseEvent e)
		{
			Location start = dragStart;
			if (start != null && start.manhattanDistanceTo(e.getX(), e.getY()) > 4)
			{
				hideCurrentPopup();
			}
		}

		public virtual void mouseMoved(MouseEvent arg0)
		{
		}

	}

}
