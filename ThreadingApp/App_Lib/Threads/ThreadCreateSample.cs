using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace SOLIDApp.App_Lib.Threads {

    public class ThreadCreateSample {

        private static DisplayInterface Display;
        private static int SleepTime = 1000;
        private static int ThreadsNo = 3;
        private static int LoopsNo = 5;

        public ThreadCreateSample(DisplayInterface display) {
            ThreadCreateSample.Display = display;
        }

        public void RunSample() {
            Display.ShowProgress(0);
            Display.ShowMessage("main thread", "Thread Sample Beginning");

            Thread.Sleep(SleepTime);

            BackgroundTask backTask = new BackgroundTask();

            ArrayList threadList = new ArrayList();
            // Create the thread object, passing in the BackgroundTask.Run method
            // via a ThreadStart delegate. This does not start the thread.
            Display.ShowProgress(20);

            for (int i = 0; i < ThreadsNo; i++) {
                Thread thread = new Thread(new ParameterizedThreadStart(backTask.Run));
                // Start the thread
                thread.Start((object)(i + 1));
                threadList.Add(thread);
            }

            Display.ShowMessage("main thread", "waiting for background task threads to join main thread");
            Display.ShowProgress(40);
            Thread.Sleep(SleepTime);

            // Wait until background thread finishes.
            foreach (Thread thread in threadList) {
                thread.Join();
            }

            Display.ShowMessage("main thread", "background task threads has joined main thread !!!");
            Display.ShowProgress(80);
            Thread.Sleep(SleepTime);

            Display.ShowMessage("main thread", "Thread Sample Finished");
            Display.ShowProgress(100);
            Thread.Sleep(SleepTime);
        }

        private class BackgroundTask {

            public BackgroundTask() {
            }

            // This method that will be called when the thread is started
            public void Run(object obj) {
                int no = (int)obj;

                for (int i = 0; i < ThreadCreateSample.LoopsNo; i++) {
                    Thread.Sleep(ThreadCreateSample.SleepTime + (no * 500));
                    Display.ShowMessage("back thread " + no, "background task loop " + (i + 1));
                }
            }
        }
    }
}