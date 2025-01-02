// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// To produce customized conversions, purchase a Premium Edition license:
// https://www.tangiblesoftwaresolutions.com/product-details/java-to-csharp-converter.html
// ====================================================================================================

using System;
using System.Threading;

/* Copyright (c) 2010, Carl Burch. License information is located in the
 * logisim.Main source code and at www.cburch.com/logisim/. */

namespace logisim.tools.move
{
	using ReplacementMap = logisim.circuit.ReplacementMap;

	internal class ConnectorThread : Thread
	{
		private static ConnectorThread INSTANCE = new ConnectorThread();

		static ConnectorThread()
		{
			INSTANCE.Start();
		}

		public static void enqueueRequest(MoveRequest req, bool priority)
		{
			lock (INSTANCE.@lock)
			{
				if (!req.Equals(INSTANCE.processingRequest))
				{
					INSTANCE.nextRequest = req;
					INSTANCE.overrideRequest = priority;
					Monitor.PulseAll(INSTANCE.@lock);
				}
			}
		}

		public static bool OverrideRequested
		{
			get
			{
				return INSTANCE.overrideRequest;
			}
		}

		private object @lock;
		[NonSerialized]
		private bool overrideRequest;
		private MoveRequest nextRequest;
		private MoveRequest processingRequest;

		private ConnectorThread()
		{
			@lock = new object();
			overrideRequest = false;
			nextRequest = null;
		}

		public virtual bool AbortRequested
		{
			get
			{
				return overrideRequest;
			}
		}

		public override void run()
		{
			while (true)
			{
				MoveRequest req;
				bool wasOverride;
				lock (@lock)
				{
					processingRequest = null;
					while (nextRequest == null)
					{
						try
						{
							Monitor.Wait(@lock);
						}
						catch (InterruptedException)
						{
							Thread.CurrentThread.Interrupt();
							return;
						}
					}
					req = nextRequest;
					wasOverride = overrideRequest;
					nextRequest = null;
					overrideRequest = false;
					processingRequest = req;
				}

				try
				{
					MoveResult result = Connector.computeWires(req);
					if (result != null)
					{
						MoveGesture gesture = req.MoveGesture;
						gesture.notifyResult(req, result);
					}
				}
				catch (Exception t)
				{
					Console.WriteLine(t.ToString());
					Console.Write(t.StackTrace);
					if (wasOverride)
					{
						MoveResult result = new MoveResult(req, new ReplacementMap(), req.MoveGesture.Connections, 0);
						req.MoveGesture.notifyResult(req, result);
					}
				}
			}
		}
	}

}
