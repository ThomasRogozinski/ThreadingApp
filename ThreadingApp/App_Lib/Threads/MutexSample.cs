using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace SOLIDApp.App_Lib.Threads {

    public class MutexSample {

        private static DisplayInterface Display;
        private static int SleepTime = 1000;

        private Mutex mutex1 = new Mutex(true);
        private Mutex mutex2 = new Mutex(true);
        // ManualResetEvent can be used instead of AutoResetEvent for the purpose of below sample
        private AutoResetEvent event1 = new AutoResetEvent(false);
        private AutoResetEvent event2 = new AutoResetEvent(false);
        private AutoResetEvent event3 = new AutoResetEvent(false);
        private AutoResetEvent event4 = new AutoResetEvent(false);

        public MutexSample(DisplayInterface display) {
            MutexSample.Display = display;

            // create mutexes initialOwned
            mutex1 = new Mutex(true);
            mutex2 = new Mutex(true);
            // create events
            event1 = new AutoResetEvent(false);
            event2 = new AutoResetEvent(false);
            event3 = new AutoResetEvent(false);
            event4 = new AutoResetEvent(false);
        }

        public void RunSample() {
            Display.ShowProgress(0);

            Display.ShowMessage("Main", "Mutex Sample Started ...");

            AutoResetEvent[] events = new AutoResetEvent[4];
            events[0] = event1;
            events[1] = event2;
            events[2] = event3;
            events[3] = event4;    

            Thread thread1 = new Thread(new ThreadStart(Start1));
            Thread thread2 = new Thread(new ThreadStart(Start2));
            Thread thread3 = new Thread(new ThreadStart(Start3));
            Thread thread4 = new Thread(new ThreadStart(Start4));
            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            Thread.Sleep(SleepTime * 2);
            Display.ShowMessage("Main", "mutex1 Released");
            mutex1.ReleaseMutex();  // thread2 and thread3 will end and signal

            Thread.Sleep(SleepTime * 4);
            Display.ShowMessage("Main", "mutex2 Released");
            mutex2.ReleaseMutex();  // t1 and t4 will end and signal

            // waiting until all four threads signal that they are done.
            WaitHandle.WaitAll(events);
            Display.ShowMessage("Main", "Mutex Sample Finished ...");
        }

        public void Start1() {
            Display.ShowMessage("Start1", "Started,  Mutex.WaitAll(mutex1, mutex2)");
            
            Mutex[] mutexes = new Mutex[2];
            // create and load an array of Mutex for WaitAll call
            mutexes[0] = mutex1;  
            mutexes[1] = mutex2;
            // waits until both mutexes are released
            Mutex.WaitAll(mutexes);  
            
            Thread.Sleep(SleepTime * 2);
            Display.ShowMessage("Start1", "Finished,  Mutex.WaitAll(mutex1, mutex2)");
            
            // release resources before leaving this thread
            mutex1.ReleaseMutex();
            mutex2.ReleaseMutex();
            // signal main thread this action is finished
            event1.Set();
        }

        public void Start2() {
            Display.ShowMessage("Start2", "Started,  mutex1.WaitOne( )");

            // waits until Mutex mutex1 is released
            mutex1.WaitOne();

            Thread.Sleep(SleepTime * 2);
            Display.ShowMessage("Start2", "Finished,  mutex1.WaitOne( )");

            // release resources before leaving this thread
            mutex1.ReleaseMutex();
            // signal main thread this action is finished
            event2.Set();   
        }

        public void Start3() {
            Display.ShowMessage("Start3", "Started,  Mutex.WaitAny(mutex1, mutex2)");
            Mutex[] mutexes = new Mutex[2];

            // Create and load an array of mutex for WaitAny call
            mutexes[0] = mutex1;  
            mutexes[1] = mutex2;
                
            // Waits until either mutex is released
            int acquiredMutex = Mutex.WaitAny(mutexes);

            Thread.Sleep(SleepTime * 2);
            Display.ShowMessage("Start3", "Finished,  Mutex.WaitAny(mutex1, mutex2)");

            // release acquired mutex before leaving this thread
            mutexes[acquiredMutex].ReleaseMutex();
            // signal main thread this action is finished
            event3.Set(); 
        }

        public void Start4() {
            Display.ShowMessage("Start4", "Started,  mutex2.WaitOne( )");
            
            // waits until mutex2 is released
            mutex2.WaitOne();   

            Thread.Sleep(SleepTime * 2);
            Display.ShowMessage("Start4", "Finished, mutex2.WaitOne( )");

            // release resources before leaving this thread
            mutex2.ReleaseMutex();
            // signal main thread this action is finished
            event4.Set();    
        }
    }
}

