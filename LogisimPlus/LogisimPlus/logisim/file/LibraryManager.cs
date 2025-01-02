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

	using Library = logisim.tools.Library;
	using StringUtil = logisim.util.StringUtil;

	internal class LibraryManager
	{
		public static readonly LibraryManager instance = new LibraryManager();

		private static char desc_sep = '#';

		private abstract class LibraryDescriptor
		{
			internal abstract bool concernsFile(File query);

			internal abstract string toDescriptor(Loader loader);

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: abstract void setBase(Loader loader, LoadedLibrary lib) throws LoadFailedException;
			internal abstract void setBase(Loader loader, LoadedLibrary lib);
		}

		private class LogisimProjectDescriptor : LibraryDescriptor
		{
			internal File file;

			internal LogisimProjectDescriptor(File file)
			{
				this.file = file;
			}

			internal override bool concernsFile(File query)
			{
				return file.Equals(query);
			}

			internal override string toDescriptor(Loader loader)
			{
				return "file#" + toRelative(loader, file);
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override void setBase(Loader loader, LoadedLibrary lib) throws LoadFailedException
			internal override void setBase(Loader loader, LoadedLibrary lib)
			{
				lib.Base = loader.loadLogisimFile(file);
			}

			public override bool Equals(object other)
			{
				if (!(other is LogisimProjectDescriptor))
				{
					return false;
				}
				LogisimProjectDescriptor o = (LogisimProjectDescriptor) other;
				return this.file.Equals(o.file);
			}

			public override int GetHashCode()
			{
				return file.GetHashCode();
			}
		}

		private class JarDescriptor : LibraryDescriptor
		{
			internal File file;
			internal string className;

			internal JarDescriptor(File file, string className)
			{
				this.file = file;
				this.className = className;
			}

			internal override bool concernsFile(File query)
			{
				return file.Equals(query);
			}

			internal override string toDescriptor(Loader loader)
			{
				return "jar#" + toRelative(loader, file) + desc_sep + className;
			}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: @Override void setBase(Loader loader, LoadedLibrary lib) throws LoadFailedException
			internal override void setBase(Loader loader, LoadedLibrary lib)
			{
				lib.Base = loader.loadJarFile(file, className);
			}

			public override bool Equals(object other)
			{
				if (!(other is JarDescriptor))
				{
					return false;
				}
				JarDescriptor o = (JarDescriptor) other;
				return this.file.Equals(o.file) && this.className.Equals(o.className);
			}

			public override int GetHashCode()
			{
				return file.GetHashCode() * 31 + className.GetHashCode();
			}
		}

		private Dictionary<LibraryDescriptor, WeakReference<LoadedLibrary>> fileMap;
		private WeakHashMap<LoadedLibrary, LibraryDescriptor> invMap;

		private LibraryManager()
		{
			fileMap = new Dictionary<LibraryDescriptor, WeakReference<LoadedLibrary>>();
			invMap = new WeakHashMap<LoadedLibrary, LibraryDescriptor>();
			ProjectsDirty.initialize();
		}

		internal virtual void setDirty(File file, bool dirty)
		{
			LoadedLibrary lib = findKnown(file);
			if (lib != null)
			{
				lib.Dirty = dirty;
			}
		}

		internal virtual ICollection<LogisimFile> LogisimLibraries
		{
			get
			{
				List<LogisimFile> ret = new List<LogisimFile>();
				foreach (LoadedLibrary lib in invMap.keySet())
				{
					if (lib.Base is LogisimFile)
					{
						ret.Add((LogisimFile) lib.Base);
					}
				}
				return ret;
			}
		}

		public virtual Library loadLibrary(Loader loader, string desc)
		{
			// It may already be loaded.
			// Otherwise we'll have to decode it.
			int sep = desc.IndexOf(desc_sep);
			if (sep < 0)
			{
				loader.showError(StringUtil.format(Strings.get("fileDescriptorError"), desc));
				return null;
			}
			string type = desc.Substring(0, sep);
			string name = desc.Substring(sep + 1);

			if (type.Equals(""))
			{
				Library ret = loader.Builtin.getLibrary(name);
				if (ret == null)
				{
					loader.showError(StringUtil.format(Strings.get("fileBuiltinMissingError"), name));
					return null;
				}
				return ret;
			}
			else if (type.Equals("file"))
			{
				File toRead = loader.getFileFor(name, Loader.LOGISIM_FILTER);
				return loadLogisimLibrary(loader, toRead);
			}
			else if (type.Equals("jar"))
			{
				int sepLoc = name.LastIndexOf(desc_sep);
				string fileName = name.Substring(0, sepLoc);
				string className = name.Substring(sepLoc + 1);
				File toRead = loader.getFileFor(fileName, Loader.JAR_FILTER);
				return loadJarLibrary(loader, toRead, className);
			}
			else
			{
				loader.showError(StringUtil.format(Strings.get("fileTypeError"), type, desc));
				return null;
			}
		}

		public virtual LoadedLibrary loadLogisimLibrary(Loader loader, File toRead)
		{
			LoadedLibrary ret = findKnown(toRead);
			if (ret != null)
			{
				return ret;
			}

			try
			{
				ret = new LoadedLibrary(loader.loadLogisimFile(toRead));
			}
			catch (LoadFailedException e)
			{
				loader.showError(e.Message);
				return null;
			}

			LogisimProjectDescriptor desc = new LogisimProjectDescriptor(toRead);
			fileMap[desc] = new WeakReference<LoadedLibrary>(ret);
			invMap.put(ret, desc);
			return ret;
		}

		public virtual LoadedLibrary loadJarLibrary(Loader loader, File toRead, string className)
		{
			JarDescriptor jarDescriptor = new JarDescriptor(toRead, className);
			LoadedLibrary ret = findKnown(jarDescriptor);
			if (ret != null)
			{
				return ret;
			}

			try
			{
				ret = new LoadedLibrary(loader.loadJarFile(toRead, className));
			}
			catch (LoadFailedException e)
			{
				loader.showError(e.Message);
				return null;
			}

			fileMap[jarDescriptor] = new WeakReference<LoadedLibrary>(ret);
			invMap.put(ret, jarDescriptor);
			return ret;
		}

		public virtual void reload(Loader loader, LoadedLibrary lib)
		{
			LibraryDescriptor descriptor = invMap.get(lib);
			if (descriptor == null)
			{
				loader.showError(StringUtil.format(Strings.get("unknownLibraryFileError"), lib.DisplayName));
			}
			else
			{
				try
				{
					descriptor.setBase(loader, lib);
				}
				catch (LoadFailedException e)
				{
					loader.showError(e.Message);
				}
			}
		}

		public virtual Library findReference(LogisimFile file, File query)
		{
			foreach (Library lib in file.Libraries)
			{
				LibraryDescriptor desc = invMap.get(lib);
				if (desc != null && desc.concernsFile(query))
				{
					return lib;
				}
				if (lib is LoadedLibrary)
				{
					LoadedLibrary loadedLib = (LoadedLibrary) lib;
					if (loadedLib.Base is LogisimFile)
					{
						LogisimFile loadedProj = (LogisimFile) loadedLib.Base;
						Library ret = findReference(loadedProj, query);
						if (ret != null)
						{
							return lib;
						}
					}
				}
			}
			return null;
		}

		public virtual void fileSaved(Loader loader, File dest, File oldFile, LogisimFile file)
		{
			LoadedLibrary old = findKnown(oldFile);
			if (old != null)
			{
				old.Dirty = false;
			}

			LoadedLibrary lib = findKnown(dest);
			if (lib != null)
			{
				LogisimFile clone = file.cloneLogisimFile(loader);
				clone.Name = file.Name;
				clone.Dirty = false;
				lib.Base = clone;
			}
		}

		public virtual string getDescriptor(Loader loader, Library lib)
		{
			if (loader.Builtin.Libraries.Contains(lib))
			{
				return desc_sep + lib.Name;
			}
			else
			{
				LibraryDescriptor desc = invMap.get(lib);
				if (desc != null)
				{
					return desc.toDescriptor(loader);
				}
				else
				{
					throw new LoaderException(StringUtil.format(Strings.get("fileDescriptorUnknownError"), lib.DisplayName));
				}
			}
		}

		private LoadedLibrary findKnown(object key)
		{
			WeakReference<LoadedLibrary> retLibRef;
			retLibRef = fileMap[key];
			if (retLibRef == null)
			{
				return null;
			}
			else
			{
				LoadedLibrary retLib = retLibRef.get();
				if (retLib == null)
				{
					fileMap.Remove(key);
					return null;
				}
				else
				{
					return retLib;
				}
			}
		}

		private static string toRelative(Loader loader, File file)
		{
			File currentDirectory = loader.CurrentDirectory;
			if (currentDirectory == null)
			{
				try
				{
					return file.getCanonicalPath();
				}
				catch (IOException)
				{
					return file.ToString();
				}
			}

			File fileDir = file.getParentFile();
			if (fileDir != null)
			{
				if (currentDirectory.Equals(fileDir))
				{
					return file.getName();
				}
				else if (currentDirectory.Equals(fileDir.getParentFile()))
				{
					return fileDir.getName() + "/" + file.getName();
				}
				else if (fileDir.Equals(currentDirectory.getParentFile()))
				{
					return "../" + file.getName();
				}
			}
			try
			{
				return file.getCanonicalPath();
			}
			catch (IOException)
			{
				return file.ToString();
			}
		}
	}
}
