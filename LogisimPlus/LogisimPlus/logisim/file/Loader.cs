// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.IO;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.file
{

	using Builtin = logisim.std.Builtin;
	using Library = logisim.tools.Library;
	using JFileChoosers = logisim.util.JFileChoosers;
	using StringUtil = logisim.util.StringUtil;
	using ZipClassLoader = logisim.util.ZipClassLoader;

	public class Loader : LibraryLoader
	{
		public const string LOGISIM_EXTENSION = ".circ";
		public static readonly FileFilter LOGISIM_FILTER = new LogisimFileFilter();
		public static readonly FileFilter JAR_FILTER = new JarFileFilter();

		private class LogisimFileFilter : FileFilter
		{
			public override bool accept(File f)
			{
				return f.isDirectory() || f.getName().EndsWith(LOGISIM_EXTENSION);
			}

			public override string Description
			{
				get
				{
					return Strings.get("logisimFileFilter");
				}
			}
		}

		private class JarFileFilter : FileFilter
		{
			public override bool accept(File f)
			{
				return f.isDirectory() || f.getName().EndsWith(".jar");
			}

			public override string Description
			{
				get
				{
					return Strings.get("jarFileFilter");
				}
			}
		}

		// fixed
		private Component parent;
		private Builtin builtin = new Builtin();

		// to be cleared with each new file
		private File mainFile = null;
		private Stack<File> filesOpening = new Stack<File>();
		private IDictionary<File, File> substitutions = new Dictionary<File, File>();

		public Loader(Component parent)
		{
			this.parent = parent;
			clear();
		}

		public virtual Builtin Builtin
		{
			get
			{
				return builtin;
			}
		}

		public virtual Component Parent
		{
			set
			{
				parent = value;
			}
		}

		private File getSubstitution(File source)
		{
			File ret = substitutions[source];
			return ret == null ? source : ret;
		}

		//
		// file chooser related methods
		//
		public virtual File MainFile
		{
			get
			{
				return mainFile;
			}
			set
			{
				mainFile = value;
			}
		}

		public virtual JFileChooser createChooser()
		{
			return JFileChoosers.createAt(CurrentDirectory);
		}

		// used here and in LibraryManager only
		internal virtual File CurrentDirectory
		{
			get
			{
				File @ref;
				if (filesOpening.Count > 0)
				{
					@ref = filesOpening.Peek();
				}
				else
				{
					@ref = mainFile;
				}
				return @ref == null ? null : @ref.getParentFile();
			}
		}


		//
		// more substantive methods accessed from outside this package
		//
		public virtual void clear()
		{
			filesOpening.Clear();
			mainFile = null;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public LogisimFile openLogisimFile(java.io.File file, java.util.Map<java.io.File, java.io.File> substitutions) throws LoadFailedException
		public virtual LogisimFile openLogisimFile(File file, IDictionary<File, File> substitutions)
		{
			this.substitutions = substitutions;
			try
			{
				return openLogisimFile(file);
			}
			finally
			{
				this.substitutions = Collections.emptyMap();
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public LogisimFile openLogisimFile(java.io.File file) throws LoadFailedException
		public virtual LogisimFile openLogisimFile(File file)
		{
			try
			{
				LogisimFile ret = loadLogisimFile(file);
				if (ret != null)
				{
					MainFile = file;
				}
				showMessages(ret);
				return ret;
			}
			catch (LoaderException e)
			{
				throw new LoadFailedException(e.Message, e.Shown);
			}
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: public LogisimFile openLogisimFile(java.io.InputStream reader) throws LoadFailedException, java.io.IOException
		public virtual LogisimFile openLogisimFile(Stream reader)
		{
			LogisimFile ret = null;
			try
			{
				ret = LogisimFile.load(reader, this);
			}
			catch (LoaderException)
			{
				return null;
			}
			showMessages(ret);
			return ret;
		}

		public virtual Library loadLogisimLibrary(File file)
		{
			File actual = getSubstitution(file);
			LoadedLibrary ret = LibraryManager.instance.loadLogisimLibrary(this, actual);
			if (ret != null)
			{
				LogisimFile retBase = (LogisimFile) ret.Base;
				showMessages(retBase);
			}
			return ret;
		}

		public virtual Library loadJarLibrary(File file, string className)
		{
			File actual = getSubstitution(file);
			return LibraryManager.instance.loadJarLibrary(this, actual, className);
		}

		public virtual void reload(LoadedLibrary lib)
		{
			LibraryManager.instance.reload(this, lib);
		}

		public virtual bool save(LogisimFile file, File dest)
		{
			Library reference = LibraryManager.instance.findReference(file, dest);
			if (reference != null)
			{
				JOptionPane.showMessageDialog(parent, StringUtil.format(Strings.get("fileCircularError"), reference.DisplayName), Strings.get("fileSaveErrorTitle"), JOptionPane.ERROR_MESSAGE);
				return false;
			}

			File backup = determineBackupName(dest);
			bool backupCreated = backup != null && dest.renameTo(backup);

			FileStream fwrite = null;
			try
			{
				fwrite = new FileStream(dest, FileMode.Create, FileAccess.Write);
				file.write(fwrite, this);
				file.Name = toProjectName(dest);

				File oldFile = MainFile;
				MainFile = dest;
				LibraryManager.instance.fileSaved(this, dest, oldFile, file);
			}
			catch (IOException e)
			{
				if (backupCreated)
				{
					recoverBackup(backup, dest);
				}
				if (dest.exists() && dest.length() == 0)
				{
					dest.delete();
				}
				JOptionPane.showMessageDialog(parent, StringUtil.format(Strings.get("fileSaveError"), e.ToString()), Strings.get("fileSaveErrorTitle"), JOptionPane.ERROR_MESSAGE);
				return false;
			}
			finally
			{
				if (fwrite != null)
				{
					try
					{
						fwrite.Close();
					}
					catch (IOException e)
					{
						if (backupCreated)
						{
							recoverBackup(backup, dest);
						}
						if (dest.exists() && dest.length() == 0)
						{
							dest.delete();
						}
						JOptionPane.showMessageDialog(parent, StringUtil.format(Strings.get("fileSaveCloseError"), e.ToString()), Strings.get("fileSaveErrorTitle"), JOptionPane.ERROR_MESSAGE);
						return false;
					}
				}
			}

			if (!dest.exists() || dest.length() == 0)
			{
				if (backupCreated && backup != null && backup.exists())
				{
					recoverBackup(backup, dest);
				}
				else
				{
					dest.delete();
				}
				JOptionPane.showMessageDialog(parent, Strings.get("fileSaveZeroError"), Strings.get("fileSaveErrorTitle"), JOptionPane.ERROR_MESSAGE);
				return false;
			}

			if (backupCreated && backup != null && backup.exists())
			{
				backup.delete();
			}
			return true;
		}

		private static File determineBackupName(File @base)
		{
			File dir = @base.getParentFile();
			string name = @base.getName();
			if (name.EndsWith(LOGISIM_EXTENSION, StringComparison.Ordinal))
			{
				name = name.Substring(0, name.Length - LOGISIM_EXTENSION.Length);
			}
			for (int i = 1; i <= 20; i++)
			{
				string ext = i == 1 ? ".bak" : (".bak" + i);
				File candidate = new File(dir, name + ext);
				if (!candidate.exists())
				{
					return candidate;
				}
			}
			return null;
		}

		private static void recoverBackup(File backup, File dest)
		{
			if (backup != null && backup.exists())
			{
				if (dest.exists())
				{
					dest.delete();
				}
				backup.renameTo(dest);
			}
		}

		//
		// methods for LibraryManager
		//
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: LogisimFile loadLogisimFile(java.io.File request) throws LoadFailedException
		internal virtual LogisimFile loadLogisimFile(File request)
		{
			File actual = getSubstitution(request);
			foreach (File fileOpening in filesOpening)
			{
				if (fileOpening.Equals(actual))
				{
					throw new LoadFailedException(StringUtil.format(Strings.get("logisimCircularError"), toProjectName(actual)));
				}
			}

			LogisimFile ret = null;
			filesOpening.Push(actual);
			try
			{
				ret = LogisimFile.load(actual, this);
			}
			catch (IOException e)
			{
				throw new LoadFailedException(StringUtil.format(Strings.get("logisimLoadError"), toProjectName(actual), e.ToString()));
			}
			finally
			{
				filesOpening.Pop();
			}
			ret.Name = toProjectName(actual);
			return ret;
		}

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: logisim.tools.Library loadJarFile(java.io.File request, String className) throws LoadFailedException
		internal virtual Library loadJarFile(File request, string className)
		{
			File actual = getSubstitution(request);
			// Up until 2.1.8, this was written to use a URLClassLoader, which
			// worked pretty well, except that the class never releases its file
			// handles. For this reason, with 2.2.0, it's been switched to use
			// a custom-written class ZipClassLoader instead. The ZipClassLoader
			// is based on something downloaded off a forum, and I'm not as sure
			// that it works as well. It certainly does more file accesses.

			// Anyway, here's the line for this new version:
			ZipClassLoader loader = new ZipClassLoader(actual);

			// And here's the code that was present up until 2.1.8, and which I
			// know to work well except for the closing-files bit. If necessary, we
			// can revert by deleting the above declaration and reinstating the below.
			/*
			 * URL url; try { url = new URL("file", "localhost", file.getCanonicalPath()); } catch (MalformedURLException
			 * e1) { throw new LoadFailedException("Internal error: Malformed URL"); } catch (IOException e1) { throw new
			 * LoadFailedException(Strings.get("jarNotOpenedError")); } URLClassLoader loader = new URLClassLoader(new URL[]
			 * { url });
			 */

			// load library class from loader
			Type retClass;
			try
			{
				retClass = loader.loadClass(className);
			}
			catch (ClassNotFoundException)
			{
				throw new LoadFailedException(StringUtil.format(Strings.get("jarClassNotFoundError"), className));
			}
			if (!(retClass.IsAssignableFrom(typeof(Library))))
			{
				throw new LoadFailedException(StringUtil.format(Strings.get("jarClassNotLibraryError"), className));
			}

			// instantiate library
			Library ret;
			try
			{
				ret = (Library) retClass.getDeclaredConstructor().newInstance();
			}
			catch (Exception)
			{
				throw new LoadFailedException(StringUtil.format(Strings.get("jarLibraryNotCreatedError"), className));
			}
			return ret;
		}

		//
		// Library methods
		//
		public virtual Library loadLibrary(string desc)
		{
			return LibraryManager.instance.loadLibrary(this, desc);
		}

		public virtual string getDescriptor(Library lib)
		{
			return LibraryManager.instance.getDescriptor(this, lib);
		}

		public virtual void showError(string description)
		{
			if (filesOpening.Count > 0)
			{
				File top = filesOpening.Peek();
				string init = toProjectName(top) + ":";
				if (description.Contains("\n"))
				{
					description = init + "\n" + description;
				}
				else
				{
					description = init + " " + description;
				}
			}

			if (description.Contains("\n") || description.Length > 60)
			{
				int lines = 1;
				for (int pos = description.IndexOf('\n'); pos >= 0; pos = description.IndexOf('\n', pos + 1))
				{
					lines++;
				}
				lines = Math.Max(4, Math.Min(lines, 7));

				JTextArea textArea = new JTextArea(lines, 60);
				textArea.setEditable(false);
				textArea.setText(description);
				textArea.setCaretPosition(0);

				JScrollPane scrollPane = new JScrollPane(textArea);
				scrollPane.setPreferredSize(new Dimension(350, 150));
				JOptionPane.showMessageDialog(parent, scrollPane, Strings.get("fileErrorTitle"), JOptionPane.ERROR_MESSAGE);
			}
			else
			{
				JOptionPane.showMessageDialog(parent, description, Strings.get("fileErrorTitle"), JOptionPane.ERROR_MESSAGE);
			}
		}

		private void showMessages(LogisimFile source)
		{
			if (source == null)
			{
				return;
			}
			string message = source.Message;
			while (!string.ReferenceEquals(message, null))
			{
				JOptionPane.showMessageDialog(parent, message, Strings.get("fileMessageTitle"), JOptionPane.INFORMATION_MESSAGE);
				message = source.Message;
			}
		}

		//
		// helper methods
		//
		internal virtual File getFileFor(string name, FileFilter filter)
		{
			// Determine the actual file name.
			File file = new File(name);
			if (!file.isAbsolute())
			{
				File currentDirectory = CurrentDirectory;
				if (currentDirectory != null)
				{
					file = new File(currentDirectory, name);
				}
			}
			while (!file.canRead())
			{
				// It doesn't exist. Figure it out from the user.
				JOptionPane.showMessageDialog(parent, StringUtil.format(Strings.get("fileLibraryMissingError"), file.getName()));
				JFileChooser chooser = createChooser();
				chooser.setFileFilter(filter);
				chooser.setDialogTitle(StringUtil.format(Strings.get("fileLibraryMissingTitle"), file.getName()));
				int action = chooser.showDialog(parent, Strings.get("fileLibraryMissingButton"));
				if (action != JFileChooser.APPROVE_OPTION)
				{
					throw new LoaderException(Strings.get("fileLoadCanceledError"));
				}
				file = chooser.getSelectedFile();
			}
			return file;
		}

		private string toProjectName(File file)
		{
			string ret = file.getName();
			if (ret.EndsWith(LOGISIM_EXTENSION, StringComparison.Ordinal))
			{
				return ret.Substring(0, ret.Length - LOGISIM_EXTENSION.Length);
			}
			else
			{
				return ret;
			}
		}

	}

}
