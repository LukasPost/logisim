// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.util
{

	public class ZipClassLoader : ClassLoader
	{
		// This code was posted on a forum by "leukbr" on March 30, 2001.
		// http://forums.sun.com/thread.jspa?threadID=360060&forumID=31
		// I've modified it substantially to include a thread that keeps the file
		// open for OPEN_TIME milliseconds so time isn't wasted continually
		// opening and closing the file.
		private const int OPEN_TIME = 5000;
		private const int DEBUG = 0;
		// 0 = no debug messages
		// 1 = open/close ZIP file only
		// 2 = also each resource request
		// 3 = all messages while retrieving resource

		private const int REQUEST_FIND = 0;
		private const int REQUEST_LOAD = 1;

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unused") private static class Request
		private class Request
		{
			internal int action;
			internal string resource;
			internal bool responseSent;
			internal object response;

			internal Request(int action, string resource)
			{
				this.action = action;
				this.resource = resource;
				this.responseSent = false;
			}

			public override string ToString()
			{
				string act = action == REQUEST_LOAD ? "load" : action == REQUEST_FIND ? "find" : "act" + action;
				return act + ":" + resource;
			}

			internal virtual object Response
			{
				set
				{
					lock (this)
					{
						response = value;
						responseSent = true;
						Monitor.PulseAll(this);
					}
				}
				get
				{
					lock (this)
					{
						while (!responseSent)
						{
							try
							{
								Monitor.Wait(this, TimeSpan.FromMilliseconds(1000));
							}
							catch (InterruptedException)
							{
							}
						}
						return response;
					}
				}
			}

			internal virtual void ensureDone()
			{
				bool aborted = false;
				lock (this)
				{
					if (!responseSent)
					{
						aborted = true;
						responseSent = true;
						response = null;
						Monitor.PulseAll(this);
					}
				}
				if (aborted && DEBUG >= 1)
				{
					Console.Error.WriteLine("request not handled successfully"); // OK
				}
			}

		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @SuppressWarnings("unused") private class WorkThread extends Thread
		private class WorkThread : Thread
		{
			private readonly ZipClassLoader outerInstance;

			public WorkThread(ZipClassLoader outerInstance)
			{
				this.outerInstance = outerInstance;
			}

			internal LinkedList<Request> requests = new LinkedList<Request>();
			internal ZipFile zipFile = null;

			public override void run()
			{
				try
				{
					while (true)
					{
						Request request = waitForNextRequest();
						if (request == null)
						{
							return;
						}

						if (DEBUG >= 2)
						{
							Console.Error.WriteLine("processing " + request); // OK
						}
						try
						{
							switch (request.action)
							{
							case REQUEST_LOAD:
								performLoad(request);
								break;
							case REQUEST_FIND:
								performFind(request);
								break;
							}
						}
						finally
						{
							request.ensureDone();
						}
						if (DEBUG >= 2)
						{
							Console.Error.WriteLine("processed: " + request.Response); // OK
						}
					}
				}
				catch (Exception t)
				{
					if (DEBUG >= 3)
					{
						Console.Error.Write("uncaught: ");
						Console.WriteLine(t.ToString());
						Console.Write(t.StackTrace);
					} // OK
				}
				finally
				{
					if (zipFile != null)
					{
						try
						{
							zipFile.close();
							zipFile = null;
							if (DEBUG >= 1)
							{
								Console.Error.WriteLine("  ZIP closed"); // OK
							}
						}
						catch (IOException)
						{
							if (DEBUG >= 1)
							{
								Console.Error.WriteLine("Error closing ZIP file"); // OK
							}
						}
					}
				}
			}

			internal virtual Request waitForNextRequest()
			{
				lock (outerInstance.bgLock)
				{
					long start = DateTimeHelper.CurrentUnixTimeMillis();
					while (requests.Count == 0)
					{
						long elapse = DateTimeHelper.CurrentUnixTimeMillis() - start;
						if (elapse >= OPEN_TIME)
						{
							outerInstance.bgThread = null;
							return null;
						}
						try
						{
							Monitor.Wait(outerInstance.bgLock, TimeSpan.FromMilliseconds(OPEN_TIME));
						}
						catch (InterruptedException)
						{
						}
					}
					return requests.RemoveFirst();
				}
			}

			internal virtual void performFind(Request req)
			{
				ensureZipOpen();
				object ret = null;
				try
				{
					if (zipFile != null)
					{
						if (DEBUG >= 3)
						{
							Console.Error.WriteLine("  retrieve ZIP entry"); // OK
						}
						string res = req.resource;
						ZipEntry zipEntry = zipFile.getEntry(res);
						if (zipEntry != null)
						{
							string url = "jar:" + outerInstance.zipPath.toURI() + "!/" + res;
							ret = (new URI(url)).toURL();
							if (DEBUG >= 3)
							{
								Console.Error.WriteLine("  found: " + url); // OK
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (DEBUG >= 3)
					{
						Console.Error.WriteLine("  error retrieving data"); // OK
					}
					Console.WriteLine(ex.ToString());
					Console.Write(ex.StackTrace);
				}
				req.Response = ret;
			}

			internal virtual void performLoad(Request req)
			{
				BufferedInputStream bis = null;
				ensureZipOpen();
				object ret = null;
				try
				{
					if (zipFile != null)
					{
						if (DEBUG >= 3)
						{
							Console.Error.WriteLine("  retrieve ZIP entry"); // OK
						}
						ZipEntry zipEntry = zipFile.getEntry(req.resource);
						if (zipEntry != null)
						{
							if (DEBUG >= 3)
							{
								Console.Error.WriteLine("  load file"); // OK
							}
							sbyte[] result = new sbyte[(int) zipEntry.getSize()];
							bis = new BufferedInputStream(zipFile.getInputStream(zipEntry));
							try
							{
								bis.read(result, 0, result.Length);
								ret = result;
							}
							catch (IOException)
							{
								if (DEBUG >= 3)
								{
									Console.Error.WriteLine("  error loading file"); // OK
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (DEBUG >= 3)
					{
						Console.Error.WriteLine("  error retrieving data"); // OK
					}
					Console.WriteLine(ex.ToString());
					Console.Write(ex.StackTrace);
				}
				finally
				{
					if (bis != null)
					{
						try
						{
							if (DEBUG >= 3)
							{
								Console.Error.WriteLine("  close file"); // OK
							}
							bis.close();
						}
						catch (IOException)
						{
							if (DEBUG >= 3)
							{
								Console.Error.WriteLine("  error closing data"); // OK
							}
						}
					}
				}
				req.Response = ret;
			}

			internal virtual void ensureZipOpen()
			{
				if (zipFile == null)
				{
					try
					{
						if (DEBUG >= 3)
						{
							Console.Error.WriteLine("  open ZIP file"); // OK
						}
						zipFile = new ZipFile(outerInstance.zipPath);
						if (DEBUG >= 1)
						{
							Console.Error.WriteLine("  ZIP opened"); // OK
						}
					}
					catch (IOException)
					{
						if (DEBUG >= 1)
						{
							Console.Error.WriteLine("  error opening ZIP file"); // OK
						}
					}
				}
			}
		}

		private File zipPath;
		private Dictionary<string, object> classes = new Dictionary<string, object>();
		private object bgLock = new object();
		private WorkThread bgThread = null;

		public ZipClassLoader(string zipFileName) : this(new File(zipFileName))
		{
		}

		public ZipClassLoader(File zipFile)
		{
			zipPath = zipFile;
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unused") public java.net.URL findResource(String resourceName)
		public override URL findResource(string resourceName)
		{
			if (DEBUG >= 3)
			{
				Console.Error.WriteLine("findResource " + resourceName); // OK
			}
			object ret = request(REQUEST_FIND, resourceName);
			if (ret is URL)
			{
				return (URL) ret;
			}
			else
			{
				return base.findResource(resourceName);
			}
		}

// JAVA TO C# CONVERTER TASK: Most Java annotations will not have direct .NET equivalent attributes:
// ORIGINAL LINE: @Override @SuppressWarnings("unused") public Class findClass(String className) throws ClassNotFoundException
// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
		public override Type findClass(string className)
		{
			bool found = false;
			object result = null;

			// check whether we have loaded this class before
			lock (classes)
			{
				found = classes.ContainsKey(className);
				if (found)
				{
					result = classes[className];
				}
			}

			// try loading it from the ZIP file if we haven't
			if (!found)
			{
				string resourceName = className.Replace('.', '/') + ".class";
				result = request(REQUEST_LOAD, resourceName);

				if (result is sbyte[])
				{
					if (DEBUG >= 3)
					{
						Console.Error.WriteLine("  define class"); // OK
					}
					sbyte[] data = (sbyte[]) result;
					result = defineClass(className, data, 0, data.Length);
					if (result != null)
					{
						if (DEBUG >= 3)
						{
							Console.Error.WriteLine("  class defined"); // OK
						}
					}
					else
					{
						if (DEBUG >= 3)
						{
							Console.Error.WriteLine("  format error"); // OK
						}
						result = new ClassFormatError(className);
					}
				}

				lock (classes)
				{
					classes[className] = result;
				}
			}

			if (result is Type)
			{
				return (Type) result;
			}
			else if (result is ClassNotFoundException)
			{
				throw (ClassNotFoundException) result;
			}
			else if (result is Exception)
			{
				throw (Error) result;
			}
			else
			{
				return base.findClass(className);
			}
		}

		private object request(int action, string resourceName)
		{
			Request request;
			lock (bgLock)
			{
				if (bgThread == null)
				{ // start the thread if it isn't working
					bgThread = new WorkThread(this);
					bgThread.Start();
				}
				request = new Request(action, resourceName);
				bgThread.requests.AddLast(request);
				Monitor.PulseAll(bgLock);
			}
			return request.Response;
		}
	}

}
