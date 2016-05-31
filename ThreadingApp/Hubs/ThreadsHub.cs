using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using SOLIDApp.App_Lib.Threads;

namespace SOLIDApp.Hubs {

    public class ThreadsHub : Hub, DisplayInterface {

        public void RunThreadCreateSample() {
            ThreadCreateSample threadSample = new ThreadCreateSample(this);
            threadSample.RunSample();
        }

        public void RunMonitorSample() {
            MonitorSample threadSample = new MonitorSample(this);
            threadSample.RunSample();
        }

        public void RunThreadPoolSample() {
            ThreadPoolSample threadSample = new ThreadPoolSample(this);
            threadSample.RunSample();
        }

        public void RunMutexSample() {
            MutexSample threadSample = new MutexSample(this);
            threadSample.RunSample();
        }

        // DisplayInterface implementation
        //
        public void ShowMessage(string obj, string message) {
            Clients.Caller.showMessage(obj, message);
        }

        public void ShowProgress(int progress) {
            Clients.Caller.showProgress(progress);
        }
    }
}