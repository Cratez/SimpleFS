// Assignment 4
// Pete Myers
// OIT, Spring 2017

using System;
using System.Threading;
using System.Collections.Generic;

namespace SimpleShell
{
    class DotNetConsoleTerminal : TerminalDriver
    {
        const char DEFAULT_CHAR_RETURN = ' ';

        private TerminalInterruptHandler mHandler;
        private Queue<char> mPartialLineQueue;
        private KeyMonitor mMonitor;

        public DotNetConsoleTerminal()
        {
            mHandler = null;
            mMonitor = null;
            mPartialLineQueue = null;
        }

        public void Connect()
        {
            // are we connected?
            if(mMonitor == null)
            { 
                // NOTE: blocks until connected
                mPartialLineQueue = new Queue<char>();
                mMonitor = new KeyMonitor();
                mMonitor.KeyPressed += MMonitor_KeyPressed;
            }
        }

        public void Disconnect()
        {
            if (mMonitor != null)
            {
                // NOTE: blocks until disconnected
                mMonitor.KeyPressed -= MMonitor_KeyPressed; // unsub
                mPartialLineQueue = null;                   // clear
                mMonitor = null;                            // clear
            }
        }

        public char RecvChar()
        {
            // NOTE: non-blocking, just returns last character received
            return mPartialLineQueue.Count > 0 ? mPartialLineQueue.Dequeue() : DEFAULT_CHAR_RETURN;
        }

        public void SendChar(char c)
        {
            Console.Write(c);
        }

        public void SendNewLine()
        {
            Console.WriteLine();
        }

        public void InstallInterruptHandler(TerminalInterruptHandler handler)
        {
            mHandler = handler;
        }

        private void MMonitor_KeyPressed(object sender, ConsoleKeyInfo e)
        {
            switch (e.Key)
            {
                // enter
                case ConsoleKey.Enter:
                    mHandler?.HandleInterrupt(TerminalInterrupt.ENTER);
                    break;
                
                // backspace
                case ConsoleKey.Backspace:
                    mHandler?.HandleInterrupt(TerminalInterrupt.BACK);
                    break;
                
                // special connect key
                case ConsoleKey.F1:
                    mHandler?.HandleInterrupt(TerminalInterrupt.CONNECT);
                    break;
                
                // default key. Most other characters
                default:
                    mPartialLineQueue.Enqueue(e.KeyChar);
                    mHandler?.HandleInterrupt(TerminalInterrupt.CHAR);
                    break;
            }
        }

        private class KeyMonitor
        {
            private Thread mThread;
            public event EventHandler<ConsoleKeyInfo> KeyPressed;

            public KeyMonitor()
            {
                mThread = new Thread(ThreadProc);
                mThread.IsBackground = true;
                mThread.Start(this);
            }

            private void ThreadProc(object param)
            {
                while (true)
                {
                    // Deal with other types of interupts
                    // Prevent echo automatically
                    var key = Console.ReadKey(true);
                    KeyPressed?.Invoke(this, key);
                }
                
            }
        }
    }
}
