using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOLIDApp.App_Lib.Threads {

    public interface DisplayInterface {
        void ShowMessage(string obj, string message);
        void ShowProgress(int progress);
    }
}