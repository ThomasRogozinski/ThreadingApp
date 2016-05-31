using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace SOLIDApp.App_Lib.Threads {

    public class MonitorSample {

        private static DisplayInterface Display;
        private static int SleepTime = 1000;
        private static int Steps = 5;

        public MonitorSample(DisplayInterface display) {
            MonitorSample.Display = display;
        }

        public void RunSample() {
            Display.ShowProgress(0);

            Product product = new Product();

            Producer producer = new Producer(product, Steps);
            Consumer consumer = new Consumer(product, Steps);  

            Thread producerThread = new Thread(new ThreadStart(producer.ThreadRun));
            Thread consumerThread = new Thread(new ThreadStart(consumer.ThreadRun));

            try {
                producerThread.Start();
                consumerThread.Start();

                // join both threads with no timeout
                producerThread.Join();   
                consumerThread.Join();
                // threads producerThread and consumerThread have finished at this point.
            } catch (ThreadStateException e) {
                Display.ShowMessage("main thread", e.Message);  
            } catch (ThreadInterruptedException e) {
                Display.ShowMessage("main thread", e.Message); 
            }
        }

        private class Producer {
            Product product;      
            int quantity = 1;

            public Producer(Product product, int quantity) {
                this.product = product;
                this.quantity = quantity;  
            }
            public void ThreadRun() {
                for (int looper = 1; looper <= quantity; looper++) {
                    product.Produce(looper);
                }
            }
        }

        private class Consumer {
            Product product;       
            int quantity = 1;

            public Consumer(Product product, int quantity) {
                this.product = product;        
                this.quantity = quantity;  
            }
            public void ThreadRun() {
                int retval;
                for (int looper = 1; looper <= quantity; looper++) {
                    Thread.Sleep(SleepTime);
                    retval = product.Consume();
                    Display.ShowProgress(looper * 100 / Steps);
                }
            }
        }

        private class Product {

            int contents;            // Product contents
            bool readyFlag = false;  // State flag

            public int Consume() {
                lock (this)  {  // Enter synchronization block
                    if (!readyFlag) {   // Wait until Product is ready for consuming
                        try {
                            // Waits for the Monitor.Pulse in Produce
                            Display.ShowMessage("consume", "waiting for pulse");
                            Monitor.Wait(this);
                        } catch (SynchronizationLockException e) {
                            Display.ShowMessage("consume", e.Message);
                        } catch (ThreadInterruptedException e) {
                            Display.ShowMessage("consume", e.Message);
                        }
                    }
                    Display.ShowMessage("consume", String.Format("Consumed: {0}", contents));
                    readyFlag = false;      // consuming is done.
                    Monitor.Pulse(this);    // Pulse notifies waiting thread that Product workItemData is changed.
                }  
                return contents;
            }

            public void Produce(int n) {
                lock (this) {   // Enter synchronization block
                    if (readyFlag) {      // Wait until Product is ready for producing.
                        try {
                            // Waits for the Monitor.Pulse in Consume
                            Display.ShowMessage("produce", "waiting for pulse"); 
                            Monitor.Wait(this);   
                        } catch (SynchronizationLockException e) {
                            Display.ShowMessage("produce", e.Message); 
                        } catch (ThreadInterruptedException e) {
                            Display.ShowMessage("produce", e.Message); 
                        }
                    }
                    contents = n;
                    Display.ShowMessage("produce", String.Format("Produced: {0}", contents));  // Display text of exception
                    readyFlag = true;       // producing is done
                    Monitor.Pulse(this);    // Pulse notifies waiting thread that Product workItemData is changed
                } 
            }
        }
    }
}
