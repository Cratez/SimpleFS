// Assignment 4
// Pete Myers
// OIT, Spring 2017

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SimpleShell
{
    public class Terminal
    {
        // NOTE: performs line discipline over driver
        private TerminalDriver driver;
        private LineQueue completedLineQueue;
        private Handler handler;

        public Terminal(TerminalDriver driver)
        {
            this.completedLineQueue = new LineQueue();
            this.handler = new Handler(driver, completedLineQueue);

            this.driver = driver;             
            this.driver.InstallInterruptHandler(handler); //todo dont hand driver to it?          
        }

        public void Connect()
        {
            driver.Connect();
        }

        public void Disconnect()
        {
            driver.Disconnect();
        }

        public bool Echo { get { return handler.Echo; } set { handler.Echo = value; } }

        public string ReadLine()
        {
            // NOTE: blocks until a line of text is available
            // the linequeue blocks until there is at least one completed line
            return completedLineQueue.Remove();
        }

        public void Write(string line)
        {
            //send each char
            foreach (char c in line)
            {
                driver.SendChar(c);
            }
        }

        public void WriteLine(string line = "")
        {
            Write(line);
            driver.SendNewLine();
        }


        private class LineQueue
        {
            private Queue<string> theQueue;
            private Mutex mutex;
            private ManualResetEvent hasItemsEvent;

            public LineQueue()
            {
                this.theQueue = new Queue<string>();
                this.mutex = new Mutex();
                this.hasItemsEvent = new ManualResetEvent(false);
            }

            public void Insert(string s)
            {
                //wait for lock
                mutex.WaitOne();

                //add item, and set event
                theQueue.Enqueue(s);
                hasItemsEvent.Set();

                //release lock
                mutex.ReleaseMutex();
            }

            public string Remove()
            {
                //wait for locks
                WaitHandle.WaitAll(new WaitHandle[] { hasItemsEvent, mutex });

                //get result
                string result = theQueue.Dequeue();

                if (theQueue.Count == 0)
                    hasItemsEvent.Reset();
                // release lock
                mutex.ReleaseMutex();

                return result;
            }

            public int Count
            {
                get
                {
                    mutex.WaitOne();
                    int count = theQueue.Count;
                    mutex.ReleaseMutex();
                    return count;
                }
            }
        }

        class Handler : TerminalInterruptHandler
        {
            private TerminalDriver driver;
            private List<char> partialLineQueue;
            private LineQueue completedLineQueue;

            public bool Echo { get; set; } = false;

            public Handler(TerminalDriver driver, LineQueue queue)
            {
                this.driver = driver;
                this.completedLineQueue = queue;
                this.partialLineQueue = new List<char>();
                
            }
            public void HandleInterrupt(TerminalInterrupt interrupt)
            {
                switch (interrupt)
                {
                    case TerminalInterrupt.CHAR:
                        // queue up the characters
                        char c = driver.RecvChar();
                        if (Echo)
                            driver.SendChar(c);
                        partialLineQueue.Add(c);
                        break;

                    case TerminalInterrupt.ENTER:
                        if (Echo)
                            driver.SendNewLine();
                        // get all the characters from the queue, and turn them into a completed line
                        completedLineQueue.Insert(new string(partialLineQueue.ToArray()));        
                        partialLineQueue.Clear();        
                        break;

                    case TerminalInterrupt.BACK:
                        // throw away LAST character, not first
                        if (partialLineQueue.Count > 0)
                        {
                            if (Echo)
                            {
                                // wipe last character
                                driver.SendChar('\b');
                                driver.SendChar(' ');
                                driver.SendChar('\b');
                            }
                            partialLineQueue.RemoveAt(partialLineQueue.Count - 1);
                        }
                        break;
                }
                //stuff will happen here
            }
        }
    }
}
