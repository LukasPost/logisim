// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.Collections.Generic;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{

	using Circuit = logisim.circuit.Circuit;
	using logisim.data;
	using AttributeSet = logisim.data.AttributeSet;
	using Action = logisim.proj.Action;
	using Project = logisim.proj.Project;
	using ProjectActions = logisim.proj.ProjectActions;
	using AddTool = logisim.tools.AddTool;
	using Library = logisim.tools.Library;
	using Tool = logisim.tools.Tool;

	public class LogisimFileActions
	{
		private LogisimFileActions()
		{
		}

		public static Action addCircuit(Circuit circuit)
		{
			return new AddCircuit(circuit);
		}

		public static Action removeCircuit(Circuit circuit)
		{
			return new RemoveCircuit(circuit);
		}

		public static Action moveCircuit(AddTool tool, int toIndex)
		{
			return new MoveCircuit(tool, toIndex);
		}

		public static Action loadLibrary(Library lib)
		{
			return new LoadLibraries(new Library[] {lib});
		}

		public static Action loadLibraries(Library[] libs)
		{
			return new LoadLibraries(libs);
		}

		public static Action unloadLibrary(Library lib)
		{
			return new UnloadLibraries(new Library[] {lib});
		}

		public static Action unloadLibraries(Library[] libs)
		{
			return new UnloadLibraries(libs);
		}

		public static Action setMainCircuit(Circuit circuit)
		{
			return new SetMainCircuit(circuit);
		}

		public static Action revertDefaults()
		{
			return new RevertDefaults();
		}

		private class AddCircuit : Action
		{
			internal Circuit circuit;

			internal AddCircuit(Circuit circuit)
			{
				this.circuit = circuit;
			}

			public override string Name
			{
				get
				{
					return Strings.get("addCircuitAction");
				}
			}

			public override void doIt(Project proj)
			{
				proj.LogisimFile.addCircuit(circuit);
			}

			public override void undo(Project proj)
			{
				proj.LogisimFile.removeCircuit(circuit);
			}
		}

		private class RemoveCircuit : Action
		{
			internal Circuit circuit;
			internal int index;

			internal RemoveCircuit(Circuit circuit)
			{
				this.circuit = circuit;
			}

			public override string Name
			{
				get
				{
					return Strings.get("removeCircuitAction");
				}
			}

			public override void doIt(Project proj)
			{
				index = proj.LogisimFile.Circuits.IndexOf(circuit);
				proj.LogisimFile.removeCircuit(circuit);
			}

			public override void undo(Project proj)
			{
				proj.LogisimFile.addCircuit(circuit, index);
			}
		}

		private class MoveCircuit : Action
		{
			internal AddTool tool;
			internal int fromIndex;
			internal int toIndex;

			internal MoveCircuit(AddTool tool, int toIndex)
			{
				this.tool = tool;
				this.toIndex = toIndex;
			}

			public override string Name
			{
				get
				{
					return Strings.get("moveCircuitAction");
				}
			}

			public override void doIt(Project proj)
			{
				fromIndex = proj.LogisimFile.Tools.IndexOf(tool);
				proj.LogisimFile.moveCircuit(tool, toIndex);
			}

			public override void undo(Project proj)
			{
				proj.LogisimFile.moveCircuit(tool, fromIndex);
			}

			public override bool shouldAppendTo(Action other)
			{
				return other is MoveCircuit && ((MoveCircuit) other).tool == this.tool;
			}

			public override Action append(Action other)
			{
				MoveCircuit ret = new MoveCircuit(tool, ((MoveCircuit) other).toIndex);
				ret.fromIndex = this.fromIndex;
				return ret.fromIndex == ret.toIndex ? null : ret;
			}
		}

		private class LoadLibraries : Action
		{
			internal Library[] libs;

			internal LoadLibraries(Library[] libs)
			{
				this.libs = libs;
			}

			public override string Name
			{
				get
				{
					if (libs.Length == 1)
					{
						return Strings.get("loadLibraryAction");
					}
					else
					{
						return Strings.get("loadLibrariesAction");
					}
				}
			}

			public override void doIt(Project proj)
			{
				for (int i = 0; i < libs.Length; i++)
				{
					proj.LogisimFile.addLibrary(libs[i]);
				}
			}

			public override void undo(Project proj)
			{
				for (int i = libs.Length - 1; i >= 0; i--)
				{
					proj.LogisimFile.removeLibrary(libs[i]);
				}
			}
		}

		private class UnloadLibraries : Action
		{
			internal Library[] libs;

			internal UnloadLibraries(Library[] libs)
			{
				this.libs = libs;
			}

			public override string Name
			{
				get
				{
					if (libs.Length == 1)
					{
						return Strings.get("unloadLibraryAction");
					}
					else
					{
						return Strings.get("unloadLibrariesAction");
					}
				}
			}

			public override void doIt(Project proj)
			{
				for (int i = libs.Length - 1; i >= 0; i--)
				{
					proj.LogisimFile.removeLibrary(libs[i]);
				}
			}

			public override void undo(Project proj)
			{
				for (int i = 0; i < libs.Length; i++)
				{
					proj.LogisimFile.addLibrary(libs[i]);
				}
			}
		}

		private class SetMainCircuit : Action
		{
			internal Circuit oldval;
			internal Circuit newval;

			internal SetMainCircuit(Circuit circuit)
			{
				newval = circuit;
			}

			public override string Name
			{
				get
				{
					return Strings.get("setMainCircuitAction");
				}
			}

			public override void doIt(Project proj)
			{
				oldval = proj.LogisimFile.MainCircuit;
				proj.LogisimFile.MainCircuit = newval;
			}

			public override void undo(Project proj)
			{
				proj.LogisimFile.MainCircuit = oldval;
			}
		}

		private class RevertAttributeValue
		{
			internal AttributeSet attrs;
			internal Attribute<object> attr;
			internal object value;

			internal RevertAttributeValue(AttributeSet attrs, Attribute<object> attr, object value)
			{
				this.attrs = attrs;
				this.attr = attr;
				this.value = value;
			}
		}

		private class RevertDefaults : Action
		{
			internal Options oldOpts;
			internal List<Library> libraries = null;
			internal List<RevertAttributeValue> attrValues;

			internal RevertDefaults()
			{
				libraries = null;
				attrValues = new List<RevertAttributeValue>();
			}

			public override string Name
			{
				get
				{
					return Strings.get("revertDefaultsAction");
				}
			}

			public override void doIt(Project proj)
			{
				LogisimFile src = ProjectActions.createNewFile(proj);
				LogisimFile dst = proj.LogisimFile;

				copyToolAttributes(src, dst);
				foreach (Library srcLib in src.Libraries)
				{
					Library dstLib = dst.getLibrary(srcLib.Name);
					if (dstLib == null)
					{
						string desc = src.Loader.getDescriptor(srcLib);
						dstLib = dst.Loader.loadLibrary(desc);
						proj.LogisimFile.addLibrary(dstLib);
						if (libraries == null)
						{
							libraries = new List<Library>();
						}
						libraries.Add(dstLib);
					}
					copyToolAttributes(srcLib, dstLib);
				}

				Options newOpts = proj.Options;
				oldOpts = new Options();
				oldOpts.copyFrom(newOpts, dst);
				newOpts.copyFrom(src.Options, dst);
			}

			internal virtual void copyToolAttributes(Library srcLib, Library dstLib)
			{
				foreach (Tool srcTool in srcLib.Tools)
				{
					AttributeSet srcAttrs = srcTool.AttributeSet;
					Tool dstTool = dstLib.getTool(srcTool.Name);
					if (srcAttrs != null && dstTool != null)
					{
						AttributeSet dstAttrs = dstTool.AttributeSet;
// JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
// ORIGINAL LINE: for (logisim.data.Attribute<?> attrBase : srcAttrs.getAttributes())
						foreach (Attribute<object> attrBase in srcAttrs.Attributes)
						{
// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unchecked") logisim.data.Attribute<Object> attr = (logisim.data.Attribute<Object>) attrBase;
							Attribute<object> attr = (Attribute<object>) attrBase;
							object srcValue = srcAttrs.getValue(attr);
							object dstValue = dstAttrs.getValue(attr);
							if (!dstValue.Equals(srcValue))
							{
								dstAttrs.setValue(attr, srcValue);
								attrValues.Add(new RevertAttributeValue(dstAttrs, attr, dstValue));
							}
						}
					}
				}
			}

			public override void undo(Project proj)
			{
				proj.Options.copyFrom(oldOpts, proj.LogisimFile);

				foreach (RevertAttributeValue attrValue in attrValues)
				{
					attrValue.attrs.setValue(attrValue.attr, attrValue.value);
				}

				if (libraries != null)
				{
					foreach (Library lib in libraries)
					{
						proj.LogisimFile.removeLibrary(lib);
					}
				}
			}
		}
	}

}
