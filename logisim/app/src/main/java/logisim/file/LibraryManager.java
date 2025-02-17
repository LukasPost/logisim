/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

package logisim.file;

import java.io.File;
import java.io.IOException;
import java.lang.ref.WeakReference;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.WeakHashMap;

import logisim.tools.Library;
import logisim.util.StringUtil;

class LibraryManager {
	public static final LibraryManager instance = new LibraryManager();

	private static char desc_sep = '#';

	private abstract static class LibraryDescriptor {
		abstract boolean concernsFile(File query);

		abstract String toDescriptor(Loader loader);

		abstract void setBase(Loader loader, LoadedLibrary lib) throws LoadFailedException;
	}

	private static class LogisimProjectDescriptor extends LibraryDescriptor {
		private File file;

		LogisimProjectDescriptor(File file) {
			this.file = file;
		}

		@Override
		boolean concernsFile(File query) {
			return file.equals(query);
		}

		@Override
		String toDescriptor(Loader loader) {
			return "file#" + toRelative(loader, file);
		}

		@Override
		void setBase(Loader loader, LoadedLibrary lib) throws LoadFailedException {
			lib.setBase(loader.loadLogisimFile(file));
		}

		@Override
		public boolean equals(Object other) {
			return other instanceof LogisimProjectDescriptor o && file.equals(o.file);
		}

		@Override
		public int hashCode() {
			return file.hashCode();
		}
	}

	private static class JarDescriptor extends LibraryDescriptor {
		private File file;
		private String className;

		JarDescriptor(File file, String className) {
			this.file = file;
			this.className = className;
		}

		@Override
		boolean concernsFile(File query) {
			return file.equals(query);
		}

		@Override
		String toDescriptor(Loader loader) {
			return "jar#" + toRelative(loader, file) + desc_sep + className;
		}

		@Override
		void setBase(Loader loader, LoadedLibrary lib) throws LoadFailedException {
			lib.setBase(loader.loadJarFile(file, className));
		}

		@Override
		public boolean equals(Object other) {
			return other instanceof JarDescriptor o && file.equals(o.file) && className.equals(o.className);
		}

		@Override
		public int hashCode() {
			return file.hashCode() * 31 + className.hashCode();
		}
	}

	private HashMap<LibraryDescriptor, WeakReference<LoadedLibrary>> fileMap;
	private WeakHashMap<LoadedLibrary, LibraryDescriptor> invMap;

	private LibraryManager() {
		fileMap = new HashMap<>();
		invMap = new WeakHashMap<>();
		ProjectsDirty.initialize();
	}

	void setDirty(File file, boolean dirty) {
		LoadedLibrary lib = findKnown(file);
		if (lib != null) lib.setDirty(dirty);
	}

	Collection<LogisimFile> getLogisimLibraries() {
		ArrayList<LogisimFile> ret = new ArrayList<>();
		for (LoadedLibrary lib : invMap.keySet())
			if (lib.getBase() instanceof LogisimFile) ret.add((LogisimFile) lib.getBase());
		return ret;
	}

	public Library loadLibrary(Loader loader, String desc) {
		// It may already be loaded.
		// Otherwise we'll have to decode it.
		int sep = desc.indexOf(desc_sep);
		if (sep < 0) {
			loader.showError(StringUtil.format(Strings.get("fileDescriptorError"), desc));
			return null;
		}
		String type = desc.substring(0, sep);
		String name = desc.substring(sep + 1);

		if (type.isEmpty()) {
			Library ret = loader.getBuiltin().getLibrary(name);
			if (ret == null) {
				loader.showError(StringUtil.format(Strings.get("fileBuiltinMissingError"), name));
				return null;
			}
			return ret;
		} else if ("file".equals(type)) {
			File toRead = loader.getFileFor(name, Loader.LOGISIM_FILTER);
			return loadLogisimLibrary(loader, toRead);
		} else if ("jar".equals(type)) {
			int sepLoc = name.lastIndexOf(desc_sep);
			String fileName = name.substring(0, sepLoc);
			String className = name.substring(sepLoc + 1);
			File toRead = loader.getFileFor(fileName, Loader.JAR_FILTER);
			return loadJarLibrary(loader, toRead, className);
		} else {
			loader.showError(StringUtil.format(Strings.get("fileTypeError"), type, desc));
			return null;
		}
	}

	public LoadedLibrary loadLogisimLibrary(Loader loader, File toRead) {
		LoadedLibrary ret = findKnown(toRead);
		if (ret != null)
			return ret;

		try {
			ret = new LoadedLibrary(loader.loadLogisimFile(toRead));
		}
		catch (LoadFailedException e) {
			loader.showError(e.getMessage());
			return null;
		}

		LogisimProjectDescriptor desc = new LogisimProjectDescriptor(toRead);
		fileMap.put(desc, new WeakReference<>(ret));
		invMap.put(ret, desc);
		return ret;
	}

	public LoadedLibrary loadJarLibrary(Loader loader, File toRead, String className) {
		JarDescriptor jarDescriptor = new JarDescriptor(toRead, className);
		LoadedLibrary ret = findKnown(jarDescriptor);
		if (ret != null)
			return ret;

		try {
			ret = new LoadedLibrary(loader.loadJarFile(toRead, className));
		}
		catch (LoadFailedException e) {
			loader.showError(e.getMessage());
			return null;
		}

		fileMap.put(jarDescriptor, new WeakReference<>(ret));
		invMap.put(ret, jarDescriptor);
		return ret;
	}

	public void reload(Loader loader, LoadedLibrary lib) {
		LibraryDescriptor descriptor = invMap.get(lib);
		if (descriptor == null)
			loader.showError(StringUtil.format(Strings.get("unknownLibraryFileError"), lib.getDisplayName()));
		else try {
			descriptor.setBase(loader, lib);
		} catch (LoadFailedException e) {
			loader.showError(e.getMessage());
		}
	}

	public Library findReference(LogisimFile file, File query) {
		for (Library lib : file.getLibraries()) {
			if (!(lib instanceof LoadedLibrary loadedLib))
				continue;
			LibraryDescriptor desc = invMap.get(loadedLib);
			if (desc != null && desc.concernsFile(query)) return lib;
			if (loadedLib.getBase() instanceof LogisimFile loadedProj) {
				Library ret = findReference(loadedProj, query);
				if (ret != null)
					return lib;
			}
		}
		return null;
	}

	public void fileSaved(Loader loader, File dest, File oldFile, LogisimFile file) {
		LoadedLibrary old = findKnown(oldFile);
		if (old != null) old.setDirty(false);

		LoadedLibrary lib = findKnown(dest);
		if (lib != null) {
			LogisimFile clone = file.cloneLogisimFile(loader);
			clone.setName(file.getName());
			clone.setDirty(false);
			lib.setBase(clone);
		}
	}

	public String getDescriptor(Loader loader, Library lib) {
		if (loader.getBuiltin().getLibraries().contains(lib))
			return desc_sep + lib.getName();

		if (!(lib instanceof LoadedLibrary loaded))
			throw new LoaderException(StringUtil.format(Strings.get("fileDescriptorUnknownError"), lib.getDisplayName()));

		LibraryDescriptor desc = invMap.get(loaded);
		if (desc == null)
			throw new LoaderException(StringUtil.format(Strings.get("fileDescriptorUnknownError"), lib.getDisplayName()));

		return desc.toDescriptor(loader);
	}

	private LoadedLibrary findKnown(Object key) {
		if (!(key instanceof LibraryDescriptor))
			return null;

		WeakReference<LoadedLibrary> retLibRef = fileMap.get(key);
		if (retLibRef == null)
			return null;

		LoadedLibrary retLib = retLibRef.get();
		if (retLib == null) {
			fileMap.remove(key);
			return null;
		}
		return retLib;
	}

	private static String toRelative(Loader loader, File file) {
		File currentDirectory = loader.getCurrentDirectory();
		if (currentDirectory == null) try {
			return file.getCanonicalPath();
		} catch (IOException e) {
			return file.toString();
		}

		File fileDir = file.getParentFile();
		if (fileDir != null) if (currentDirectory.equals(fileDir)) return file.getName();
		else if (currentDirectory.equals(fileDir.getParentFile()))
			return fileDir.getName() + "/" + file.getName();
		else if (fileDir.equals(currentDirectory.getParentFile())) return "../" + file.getName();
		try {
			return file.getCanonicalPath();
		}
		catch (IOException e) {
			return file.toString();
		}
	}
}