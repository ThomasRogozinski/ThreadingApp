using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace SOLIDApp.App_Lib.Threads {

    public class ThreadPoolSample {

        private static DisplayInterface Display;
        private static int PoolSize = 10;   // Allow a total of 10 work items in the pool
        private static int SleepTime = 1000;

        public ThreadPoolSample(DisplayInterface display) {
            ThreadPoolSample.Display = display;
        }

        public void RunSample() {
            Display.ShowProgress(0);

            Display.ShowMessage("Main", "Thread Pool Sample:");
            bool operationSupported = false;

            // mark the event as unsignaled/blocking.
            ManualResetEvent mre = new ManualResetEvent(false);
            Display.ShowMessage("Main", String.Format("Queuing {0} items to Thread Pool", PoolSize));

            // create worker object
            WorkItem workItem = new WorkItem(PoolSize, mre); 
            
            Display.ShowMessage("Main", "Queue to Thread Pool 0");
            try {
                // queue the work items
                ThreadPool.QueueUserWorkItem(new WaitCallback(workItem.Run), new WorkItemData(0));
                operationSupported = true;
            } catch (NotSupportedException) {
                Display.ShowMessage("Main", "These API's may fail when Thread Pool is not OS supported.");
                operationSupported = false;
            }
            if (operationSupported) {
                for (int i = 1; i < PoolSize; i++) {
                    // queue the work items
                    Display.ShowMessage("Main", String.Format("Queue to Thread Pool {0}", i));
                    ThreadPool.QueueUserWorkItem(new WaitCallback(workItem.Run), new WorkItemData(i));
                }
                Display.ShowMessage("Main", "Waiting for Thread Pool to drain");

                // wait until event is fired/signalled
                mre.WaitOne(Timeout.Infinite, true);

                // the WaitOne won't return until the event has been signaled.
                Display.ShowMessage("Main", "Load across threads");
                foreach (object o in workItem.threadsTable.Keys)
                    Display.ShowMessage("Main", String.Format("{0} {1}", o, workItem.threadsTable[o]));
            }
        }

        private class WorkItemData {
            public int Data;

            public WorkItemData(int data) {
                this.Data = data;
            }
        }

        private class WorkItem {
            public Hashtable threadsTable;
            private ManualResetEvent mre;
            private int countRuns;
            private int poolSize;

            public WorkItem(int poolSize, ManualResetEvent mre) {
                // synchronize hashtable to use it across different threads
                threadsTable = Hashtable.Synchronized(new Hashtable(poolSize));
                countRuns = 0;
                this.poolSize = poolSize;
                this.mre = mre;
            }

            // Run is the method that will be called when the thread pool has an available thread for the work item
            public void Run(Object workItemData) {
                int curThreadId = Thread.CurrentThread.GetHashCode();

                if (!threadsTable.ContainsKey(Thread.CurrentThread.GetHashCode())) {
                    threadsTable.Add(Thread.CurrentThread.GetHashCode(), 0);
                }
                threadsTable[Thread.CurrentThread.GetHashCode()] = ((int)threadsTable[Thread.CurrentThread.GetHashCode()]) + 1;

                Display.ShowMessage("work thread", String.Format("Current Thread: {0}  Worker Data: {1}  Total Threads: {2}", curThreadId, ((WorkItemData)workItemData).Data, threadsTable.Count));

                // do some work
                Thread.Sleep(SleepTime * 2);

                // The Interlocked.Increment method allows thread-safe modification of variables accessible across multiple threads
                Interlocked.Increment(ref countRuns);
                if (countRuns == poolSize) {
                    Display.ShowMessage("work thread", "Signalling Event");
                    mre.Set();
                }
            }
        }
    }
}

