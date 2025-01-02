// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System.IO;
using System.Text;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.gui.log
{

	using Value = logisim.data.Value;

	internal class LogThread : Thread, ModelListener
	{
		// file will be flushed with at least this frequency
		private const int FLUSH_FREQUENCY = 500;

		// file will be closed after waiting this many milliseconds between writes
		private const int IDLE_UNTIL_CLOSE = 10000;

		private Model model;
		private bool canceled = false;
		private object @lock = new object();
		private PrintWriter writer = null;
		private bool headerDirty = true;
		private long lastWrite = 0;

		public LogThread(Model model)
		{
			this.model = model;
			model.addModelListener(this);
		}

		public override void run()
		{
			while (!canceled)
			{
				lock (@lock)
				{
					if (writer != null)
					{
						if (DateTimeHelper.CurrentUnixTimeMillis() - lastWrite > IDLE_UNTIL_CLOSE)
						{
							writer.close();
							writer = null;
						}
						else
						{
							writer.flush();
						}
					}
				}
				try
				{
					Thread.Sleep(FLUSH_FREQUENCY);
				}
				catch (InterruptedException)
				{
				}
			}
			lock (@lock)
			{
				if (writer != null)
				{
					writer.close();
					writer = null;
				}
			}
		}

		public virtual void cancel()
		{
			lock (@lock)
			{
				canceled = true;
				if (writer != null)
				{
					writer.close();
					writer = null;
				}
			}
		}

		public virtual void selectionChanged(ModelEvent @event)
		{
			headerDirty = true;
		}

		public virtual void entryAdded(ModelEvent @event, Value[] values)
		{
			lock (@lock)
			{
				if (FileEnabled)
				{
					addEntry(values);
				}
			}
		}

		public virtual void filePropertyChanged(ModelEvent @event)
		{
			lock (@lock)
			{
				if (FileEnabled)
				{
					if (writer == null)
					{
						Selection sel = model.Selection;
						Value[] values = new Value[sel.size()];
						bool found = false;
						for (int i = 0; i < values.Length; i++)
						{
							values[i] = model.getValueLog(sel.get(i)).Last;
							if (values[i] != null)
							{
								found = true;
							}
						}
						if (found)
						{
							addEntry(values);
						}
					}
				}
				else
				{
					if (writer != null)
					{
						writer.close();
						writer = null;
					}
				}
			}
		}

		private bool FileEnabled
		{
			get
			{
				return !canceled && model.Selected && model.FileEnabled && model.File != null;
			}
		}

		// Should hold lock and have verified that isFileEnabled() before
		// entering this method.
		private void addEntry(Value[] values)
		{
			if (writer == null)
			{
				try
				{
					writer = new PrintWriter(new StreamWriter(model.File, true));
				}
				catch (IOException)
				{
					model.File = null;
					return;
				}
			}
			Selection sel = model.Selection;
			if (headerDirty)
			{
				if (model.FileHeader)
				{
					StringBuilder buf = new StringBuilder();
					for (int i = 0; i < sel.size(); i++)
					{
						if (i > 0)
						{
							buf.Append("\t");
						}
						buf.Append(sel.get(i).ToString());
					}
					writer.println(buf.ToString());
				}
				headerDirty = false;
			}
			StringBuilder buf = new StringBuilder();
			for (int i = 0; i < values.Length; i++)
			{
				if (i > 0)
				{
					buf.Append("\t");
				}
				if (values[i] != null)
				{
					int radix = sel.get(i).getRadix();
					buf.Append(values[i].toDisplayString(radix));
				}
			}
			writer.println(buf.ToString());
			lastWrite = DateTimeHelper.CurrentUnixTimeMillis();
		}
	}

}
