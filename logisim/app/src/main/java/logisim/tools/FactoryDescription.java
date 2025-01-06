/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.tools;

import java.util.Arrays;
import java.util.List;

import javax.swing.Icon;

import logisim.comp.ComponentFactory;
import logisim.util.Icons;
import logisim.util.StringGetter;

/**
 * This class allows an object to be created holding all the information essential to showing a ComponentFactory in the
 * explorer window, but without actually loading the ComponentFactory unless a program genuinely gets around to needing
 * to use it. Note that for this to work, the relevant ComponentFactory class must be in the same package as its Library
 * class, the ComponentFactory class must be public, and it must include a public no-arguments constructor.
 */
public class FactoryDescription {
	public static List<Tool> getTools(Class<? extends Library> base, FactoryDescription[] descriptions) {
		Tool[] tools = new Tool[descriptions.length];
		for (int i = 0; i < tools.length; i++) tools[i] = new AddTool(base, descriptions[i]);
		return Arrays.asList(tools);
	}

	private String name;
	private StringGetter displayName;
	private String iconName;
	private boolean iconLoadAttempted;
	private Icon icon;
	private String factoryClassName;
	private boolean factoryLoadAttempted;
	private ComponentFactory factory;
	private StringGetter toolTip;

	public FactoryDescription(String name, StringGetter displayName, String iconName, String factoryClassName) {
		this(name, displayName, factoryClassName);
		this.iconName = iconName;
		iconLoadAttempted = false;
		icon = null;
	}

	public FactoryDescription(String name, StringGetter displayName, Icon icon, String factoryClassName) {
		this(name, displayName, factoryClassName);
		iconName = "???";
		iconLoadAttempted = true;
		this.icon = icon;
	}

	public FactoryDescription(String name, StringGetter displayName, String factoryClassName) {
		this.name = name;
		this.displayName = displayName;
		iconName = "???";
		iconLoadAttempted = true;
		icon = null;
		this.factoryClassName = factoryClassName;
		factoryLoadAttempted = false;
		factory = null;
		toolTip = null;
	}

	public String getName() {
		return name;
	}

	public String getDisplayName() {
		return displayName.get();
	}

	public boolean isFactoryLoaded() {
		return factoryLoadAttempted;
	}

	public Icon getIcon() {
		if (icon != null || iconLoadAttempted) return icon;
		icon = Icons.getIcon(iconName);
		iconLoadAttempted = true;
		return icon;
	}

	public ComponentFactory getFactory(Class<? extends Library> libraryClass) {
		ComponentFactory ret = factory;
		if (factory != null || factoryLoadAttempted) return ret;
		else {
			String msg = "";
			try {
				msg = "getting class loader";
				ClassLoader loader = libraryClass.getClassLoader();
				msg = "getting package name";
				String name;
				Package pack = libraryClass.getPackage();
				if (pack == null) name = factoryClassName;
				else name = pack.getName() + "." + factoryClassName;
				msg = "loading class";
				Class<?> factoryClass = loader.loadClass(name);
				msg = "creating instance";
				Object factoryValue = factoryClass.getConstructor().newInstance();
				msg = "converting to factory";
				if (factoryValue instanceof ComponentFactory) {
					ret = (ComponentFactory) factoryValue;
					factory = ret;
					factoryLoadAttempted = true;
					return ret;
				}
			}
			catch (Throwable t) {
				String name = t.getClass().getName();
				String m = t.getMessage();
				if (m != null)
					msg = msg + ": " + name + ": " + m;
				else
					msg = msg + ": " + name;
			}
			System.err.println("error while " + msg); // OK
			factory = null;
			factoryLoadAttempted = true;
			return null;
		}
	}

	public FactoryDescription setToolTip(StringGetter getter) {
		toolTip = getter;
		return this;
	}

	public String getToolTip() {
		StringGetter getter = toolTip;
		return getter == null ? null : getter.get();
	}
}
