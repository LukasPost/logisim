// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools
{


	using LogisimVersion = logisim.LogisimVersion;
	using Circuit = logisim.circuit.Circuit;
	using CircuitException = logisim.circuit.CircuitException;
	using CircuitMutation = logisim.circuit.CircuitMutation;
	using SubcircuitFactory = logisim.circuit.SubcircuitFactory;
	using Component = logisim.comp.Component;
	using ComponentFactory = logisim.comp.ComponentFactory;
	using ComponentDrawContext = logisim.comp.ComponentDrawContext;
	using logisim.data;
	using AttributeEvent = logisim.data.AttributeEvent;
	using AttributeListener = logisim.data.AttributeListener;
	using AttributeSet = logisim.data.AttributeSet;
	using Bounds = logisim.data.Bounds;
	using Direction = logisim.data.Direction;
	using Location = logisim.data.Location;
	using Canvas = logisim.gui.main.Canvas;
	using SelectionActions = logisim.gui.main.SelectionActions;
	using ToolAttributeAction = logisim.gui.main.ToolAttributeAction;
	using AppPreferences = logisim.prefs.AppPreferences;
	using Action = logisim.proj.Action;
	using Dependencies = logisim.proj.Dependencies;
	using Project = logisim.proj.Project;
	using KeyConfigurationEvent = logisim.tools.key.KeyConfigurationEvent;
	using KeyConfigurator = logisim.tools.key.KeyConfigurator;
	using KeyConfigurationResult = logisim.tools.key.KeyConfigurationResult;
	using StringUtil = logisim.util.StringUtil;
    using LogisimPlus.Java;

    public class AddTool : Tool
	{
		private static int INVALID_COORD = int.MinValue;

		private static int SHOW_NONE = 0;
		private static int SHOW_GHOST = 1;
		private static int SHOW_ADD = 2;
		private static int SHOW_ADD_NO = 3;

		private static Cursor cursor = Cursor.getPredefinedCursor(Cursor.CROSSHAIR_CURSOR);

		private class MyAttributeListener : AttributeListener
		{
			private readonly AddTool outerInstance;

			public MyAttributeListener(AddTool outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			public virtual void attributeListChanged(AttributeEvent e)
			{
				outerInstance.bounds = null;
			}

			public virtual void attributeValueChanged(AttributeEvent e)
			{
				outerInstance.bounds = null;
			}
		}

		private Type descriptionBase;
		private FactoryDescription description;
		private bool sourceLoadAttempted;
		private ComponentFactory factory;
		private AttributeSet attrs;
		private Bounds bounds;
		private bool shouldSnap;
		private int lastX = INVALID_COORD;
		private int lastY = INVALID_COORD;
		private int state = SHOW_GHOST;
		private Action lastAddition;
		private bool keyHandlerTried;
		private KeyConfigurator keyHandler;

		public AddTool(Type @base, FactoryDescription description)
		{
			this.descriptionBase = @base;
			this.description = description;
			this.sourceLoadAttempted = false;
			this.shouldSnap = true;
			this.attrs = new FactoryAttributes(@base, description);
			attrs.addAttributeListener(new MyAttributeListener(this));
			this.keyHandlerTried = false;
		}

		public AddTool(ComponentFactory source)
		{
			this.description = null;
			this.sourceLoadAttempted = true;
			this.factory = source;
			this.bounds = null;
			this.attrs = new FactoryAttributes(source);
			attrs.addAttributeListener(new MyAttributeListener(this));
			bool? value = (bool?) source.getFeature(ComponentFactory.SHOULD_SNAP, attrs);
			this.shouldSnap = value == null ? true : value.Value;
		}

		private AddTool(AddTool @base)
		{
			this.descriptionBase = @base.descriptionBase;
			this.description = @base.description;
			this.sourceLoadAttempted = @base.sourceLoadAttempted;
			this.factory = @base.factory;
			this.bounds = @base.bounds;
			this.shouldSnap = @base.shouldSnap;
			this.attrs = (AttributeSet) @base.attrs.clone();
			attrs.addAttributeListener(new MyAttributeListener(this));
		}

		public override bool Equals(object other)
		{
			if (!(other is AddTool))
			{
				return false;
			}
			AddTool o = (AddTool) other;
			if (this.description != null)
			{
				return this.descriptionBase == o.descriptionBase && this.description.Equals(o.description);
			}
			else
			{
				return this.factory.Equals(o.factory);
			}
		}

		public override int GetHashCode()
		{
			FactoryDescription desc = description;
			return desc != null ? desc.GetHashCode() : factory.GetHashCode();
		}

		public override bool sharesSource(Tool other)
		{
			if (!(other is AddTool))
			{
				return false;
			}
			AddTool o = (AddTool) other;
			if (this.sourceLoadAttempted && o.sourceLoadAttempted)
			{
				return this.factory.Equals(o.factory);
			}
			else if (this.description == null)
			{
				return o.description == null;
			}
			else
			{
				return this.description.Equals(o.description);
			}
		}

		public virtual ComponentFactory getFactory(bool forceLoad)
		{
			return forceLoad ? Factory : factory;
		}

		public virtual ComponentFactory Factory
		{
			get
			{
				ComponentFactory ret = factory;
				if (ret != null || sourceLoadAttempted)
				{
					return ret;
				}
				else
				{
					ret = description.getFactory(descriptionBase);
					if (ret != null)
					{
						AttributeSet @base = BaseAttributes;
						bool? value = (bool?) ret.getFeature(ComponentFactory.SHOULD_SNAP, @base);
						shouldSnap = value == null ? true : value.Value;
					}
					factory = ret;
					sourceLoadAttempted = true;
					return ret;
				}
			}
		}

		public override string Name
		{
			get
			{
				FactoryDescription desc = description;
				return desc == null ? factory.Name : desc.Name;
			}
		}

		public override string DisplayName
		{
			get
			{
				FactoryDescription desc = description;
				return desc == null ? factory.DisplayName : desc.DisplayName;
			}
		}

		public override string Description
		{
			get
			{
				string ret;
				FactoryDescription desc = description;
				if (desc != null)
				{
					ret = desc.getToolTip();
				}
				else
				{
					ComponentFactory source = Factory;
					if (source != null)
					{
						ret = (string) source.getFeature(ComponentFactory.TOOL_TIP, AttributeSet);
					}
					else
					{
						ret = null;
					}
				}
				if (string.ReferenceEquals(ret, null))
				{
					ret = StringUtil.format(Strings.get("addToolText"), DisplayName);
				}
				return ret;
			}
		}

		public override Tool cloneTool()
		{
			return new AddTool(this);
		}

		public override AttributeSet AttributeSet
		{
			get
			{
				return attrs;
			}
		}

		public override bool isAllDefaultValues(AttributeSet attrs, LogisimVersion ver)
		{
			return this.attrs == attrs && attrs is FactoryAttributes && !((FactoryAttributes) attrs).FactoryInstantiated;
		}

		public virtual object getDefaultAttributeValue<T1>(Attribute attr, LogisimVersion ver)
		{
			return Factory.getDefaultAttributeValue(attr, ver);
		}

		public override void draw(Canvas canvas, ComponentDrawContext context)
		{
			// next "if" suggested roughly by Kevin Walsh of Cornell to take care of
			// repaint problems on OpenJDK under Ubuntu
			int x = lastX;
			int y = lastY;
			if (x == INVALID_COORD || y == INVALID_COORD)
			{
				return;
			}
			ComponentFactory source = Factory;
			if (source == null)
			{
				return;
			}
			if (state == SHOW_GHOST)
			{
				source.drawGhost(context, Color.Gray, x, y, BaseAttributes);
			}
			else if (state == SHOW_ADD)
			{
				source.drawGhost(context, Color.Black, x, y, BaseAttributes);
			}
		}

		private AttributeSet BaseAttributes
		{
			get
			{
				AttributeSet ret = attrs;
				if (ret is FactoryAttributes)
				{
					ret = ((FactoryAttributes) ret).Base;
				}
				return ret;
			}
		}

		public virtual void cancelOp()
		{
		}

		public override void select(Canvas canvas)
		{
			setState(canvas, SHOW_GHOST);
			bounds = null;
		}

		public override void deselect(Canvas canvas)
		{
			setState(canvas, SHOW_GHOST);
			moveTo(canvas, canvas.getJGraphics(), INVALID_COORD, INVALID_COORD);
			bounds = null;
			lastAddition = null;
		}

		private void moveTo(Canvas canvas, JGraphics g, int x, int y)
		{
			lock (this)
			{
				if (state != SHOW_NONE)
				{
					expose(canvas, lastX, lastY);
				}
				lastX = x;
				lastY = y;
				if (state != SHOW_NONE)
				{
					expose(canvas, lastX, lastY);
				}
			}
		}

		public override void mouseEntered(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (state == SHOW_GHOST || state == SHOW_NONE)
			{
				setState(canvas, SHOW_GHOST);
				canvas.requestFocusInWindow();
			}
			else if (state == SHOW_ADD_NO)
			{
				setState(canvas, SHOW_ADD);
				canvas.requestFocusInWindow();
			}
		}

		public override void mouseExited(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (state == SHOW_GHOST)
			{
				moveTo(canvas, canvas.getJGraphics(), INVALID_COORD, INVALID_COORD);
				setState(canvas, SHOW_NONE);
			}
			else if (state == SHOW_ADD)
			{
				moveTo(canvas, canvas.getJGraphics(), INVALID_COORD, INVALID_COORD);
				setState(canvas, SHOW_ADD_NO);
			}
		}

		public override void mouseMoved(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (state != SHOW_NONE)
			{
				if (shouldSnap)
				{
					Canvas.snapToGrid(e);
				}
				moveTo(canvas, g, e.getX(), e.getY());
			}
		}

		public override void mousePressed(Canvas canvas, JGraphics g, MouseEvent e)
		{
			// verify the addition would be valid
			Circuit circ = canvas.Circuit;
			if (!canvas.Project.LogisimFile.contains(circ))
			{
				canvas.ErrorMessage = Strings.getter("cannotModifyError");
				return;
			}
			if (factory is SubcircuitFactory)
			{
				SubcircuitFactory circFact = (SubcircuitFactory) factory;
				Dependencies depends = canvas.Project.Dependencies;
				if (!depends.canAdd(circ, circFact.Subcircuit))
				{
					canvas.ErrorMessage = Strings.getter("circularError");
					return;
				}
			}

			if (shouldSnap)
			{
				Canvas.snapToGrid(e);
			}
			moveTo(canvas, g, e.getX(), e.getY());
			setState(canvas, SHOW_ADD);
		}

		public override void mouseDragged(Canvas canvas, JGraphics g, MouseEvent e)
		{
			if (state != SHOW_NONE)
			{
				if (shouldSnap)
				{
					Canvas.snapToGrid(e);
				}
				moveTo(canvas, g, e.getX(), e.getY());
			}
		}

		public override void mouseReleased(Canvas canvas, JGraphics g, MouseEvent e)
		{
			Component added = null;
			if (state == SHOW_ADD)
			{
				Circuit circ = canvas.Circuit;
				if (!canvas.Project.LogisimFile.contains(circ))
				{
					return;
				}
				if (shouldSnap)
				{
					Canvas.snapToGrid(e);
				}
				moveTo(canvas, g, e.getX(), e.getY());

				Location loc = new Location(e.getX(), e.getY());
				AttributeSet attrsCopy = (AttributeSet) attrs.clone();
				ComponentFactory source = Factory;
				if (source == null)
				{
					return;
				}
				Component c = source.createComponent(loc, attrsCopy);

				if (circ.hasConflict(c))
				{
					canvas.ErrorMessage = Strings.getter("exclusiveError");
					return;
				}

				Bounds bds = c.getBounds(g);
				if (bds.X < 0 || bds.Y < 0)
				{
					canvas.ErrorMessage = Strings.getter("negativeCoordError");
					return;
				}

				try
				{
					CircuitMutation mutation = new CircuitMutation(circ);
					mutation.add(c);
					Action action = mutation.toAction(Strings.getter("addComponentAction", factory.DisplayGetter));
					canvas.Project.doAction(action);
					lastAddition = action;
					added = c;
				}
				catch (CircuitException ex)
				{
					JOptionPane.showMessageDialog(canvas.Project.Frame, ex.Message);
				}
				setState(canvas, SHOW_GHOST);
			}
			else if (state == SHOW_ADD_NO)
			{
				setState(canvas, SHOW_NONE);
			}

			Project proj = canvas.Project;
			Tool next = determineNext(proj);
			if (next != null)
			{
				proj.Tool = next;
				Action act = SelectionActions.dropAll(canvas.Selection);
				if (act != null)
				{
					proj.doAction(act);
				}
				if (added != null)
				{
					canvas.Selection.add(added);
				}
			}
		}

		private Tool determineNext(Project proj)
		{
			string afterAdd = AppPreferences.ADD_AFTER.get();
			if (afterAdd.Equals(AppPreferences.ADD_AFTER_UNCHANGED))
			{
				return null;
			}
			else
			{ // switch to Edit Tool
				Library @base = proj.LogisimFile.getLibrary("Base");
				if (@base == null)
				{
					return null;
				}
				else
				{
					return @base.getTool("Edit Tool");
				}
			}
		}

		public override void keyPressed(Canvas canvas, KeyEvent @event)
		{
			processKeyEvent(canvas, @event, KeyConfigurationEvent.KEY_PRESSED);

			if (!@event.isConsumed() && @event.getModifiersEx() == 0)
			{
				switch (@event.getKeyCode())
				{
				case KeyEvent.VK_UP:
					setFacing(canvas, Direction.North);
					break;
				case KeyEvent.VK_DOWN:
					setFacing(canvas, Direction.South);
					break;
				case KeyEvent.VK_LEFT:
					setFacing(canvas, Direction.West);
					break;
				case KeyEvent.VK_RIGHT:
					setFacing(canvas, Direction.East);
					break;
				case KeyEvent.VK_BACK_SPACE:
					if (lastAddition != null && canvas.Project.LastAction == lastAddition)
					{
						canvas.Project.undoAction();
						lastAddition = null;
					}
				break;
				}
			}
		}

		public override void keyReleased(Canvas canvas, KeyEvent @event)
		{
			processKeyEvent(canvas, @event, KeyConfigurationEvent.KEY_RELEASED);
		}

		public override void keyTyped(Canvas canvas, KeyEvent @event)
		{
			processKeyEvent(canvas, @event, KeyConfigurationEvent.KEY_TYPED);
		}

		private void processKeyEvent(Canvas canvas, KeyEvent @event, int type)
		{
			KeyConfigurator handler = keyHandler;
			if (!keyHandlerTried)
			{
				ComponentFactory source = Factory;
				AttributeSet baseAttrs = BaseAttributes;
				handler = (KeyConfigurator) source.getFeature(typeof(KeyConfigurator), baseAttrs);
				keyHandler = handler;
				keyHandlerTried = true;
			}

			if (handler != null)
			{
				AttributeSet baseAttrs = BaseAttributes;
				KeyConfigurationEvent e = new KeyConfigurationEvent(type, baseAttrs, @event, this);
				KeyConfigurationResult r = handler.keyEventReceived(e);
				if (r != null)
				{
					Action act = ToolAttributeAction.create(r);
					canvas.Project.doAction(act);
				}
			}
		}

		private void setFacing(Canvas canvas, Direction facing)
		{
			ComponentFactory source = Factory;
			if (source == null)
			{
				return;
			}
			AttributeSet @base = BaseAttributes;
			object feature = source.getFeature(ComponentFactory.FACING_ATTRIBUTE_KEY, @base);
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<logisim.data.Direction> attr = (logisim.data.Attribute<logisim.data.Direction>) feature;
			Attribute<Direction> attr = (Attribute<Direction>) feature;
			if (attr != null)
			{
				Action act = ToolAttributeAction.create(this, attr, facing);
				canvas.Project.doAction(act);
			}
		}

		public override void paintIcon(ComponentDrawContext c, int x, int y)
		{
			FactoryDescription desc = description;
			if (desc != null && !desc.FactoryLoaded)
			{
				Icon icon = desc.Icon;
				if (icon != null)
				{
					icon.paintIcon(c.Destination, c.Graphics, x + 2, y + 2);
					return;
				}
			}

			ComponentFactory source = Factory;
			if (source != null)
			{
				AttributeSet @base = BaseAttributes;
				source.paintIcon(c, x, y, @base);
			}
		}

		private void expose(java.awt.Component c, int x, int y)
		{
			Bounds bds = Bounds;
			c.repaint(x + bds.X, y + bds.Y, bds.Width, bds.Height);
		}

		public override Cursor Cursor
		{
			get
			{
				return cursor;
			}
		}

		private void setState(Canvas canvas, int value)
		{
			if (value == SHOW_GHOST)
			{
				if (canvas.Project.LogisimFile.contains(canvas.Circuit) && AppPreferences.ADD_SHOW_GHOSTS.Boolean)
				{
					state = SHOW_GHOST;
				}
				else
				{
					state = SHOW_NONE;
				}
			}
			else
			{
				state = value;
			}
		}

		private Bounds Bounds
		{
			get
			{
				Bounds ret = bounds;
				if (ret == null)
				{
					ComponentFactory source = Factory;
					if (source == null)
					{
						ret = Bounds.EMPTY_BOUNDS;
					}
					else
					{
						AttributeSet @base = BaseAttributes;
						ret = source.getOffsetBounds(@base).expand(5);
					}
					bounds = ret;
				}
				return ret;
			}
		}
	}

}
