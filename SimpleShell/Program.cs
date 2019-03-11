// Assignment 4
// John Harrison
// OIT, Spring 2017

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FileSystemCSharp;


namespace SimpleShell
{
    class Program
    {
        static void Main(string[] args)
        {

            //TestTerminalDriver();
            //TestTerminal();
            //TestSecuritySystem();
            TestSessionManager();

            //pause
            Console.ReadKey();
        }

        static void TestSessionManager()
        {
            SlowDisk disk = new SlowDisk(42);
            disk.TurnOn();

            FileSystem filesystem = new SimpleFS();
            filesystem.Format(disk);
            filesystem.Mount(disk, "/");

            //make dir/files
            Directory root = filesystem.GetRootDirectory();
            Directory usersDir = root.CreateDirectory("users");
            Directory johnDir = usersDir.CreateDirectory("john");
            Directory subDir = johnDir.CreateDirectory("subdir");
            subDir.CreateFile("file1");
            subDir.CreateFile("file2");
            // add an /etc/passwd file that contains users for security system.


            SecuritySystem security = new SimpleSecurity();
            security.AddUser("john");
            security.SetPassword("john", "foo");

            
            ShellFactory shells = new SimpleShellFactory();
            SessionManager sessionManager = new SimpleSessionManager(security,filesystem,shells);
            TerminalDriver driver = new DotNetConsoleTerminal();
            Terminal term = new Terminal(driver);
            term.Connect();

            while (true)
            {
                Session session = sessionManager.NewSession(term);
                if (session == null)
                {
                    throw new Exception("Failed to get sessions!");
                }
                session.Run();

                // after session exits, logout
                session.Logout();
            }

            term.Disconnect();
        }

        static void TestSecuritySystem()
        {
            string username = "john";
            SecuritySystem security = new SimpleSecurity();
            security.AddUser(username);

            if (security.NeedsPassword(username))
            {
                security.SetPassword(username, "test");
            }
            int userId = security.Authenticate(username, "test");
            Console.WriteLine("UserID " + userId.ToString());
            Console.WriteLine("Username " + security.UserName(userId));
            Console.WriteLine("Home Directory " + security.UserHomeDirectory(userId));
            Console.WriteLine("Shell " + security.UserPreferredShell(userId));
        }

        static void TestTerminal()
        {
            TerminalDriver driver = new DotNetConsoleTerminal();
            Terminal term = new Terminal(driver);
            term.Connect();

            term.Write("Enter some text: ");
            term.Echo = true;
            string s1 = term.ReadLine();
            term.Write("You entered: " + s1);

            term.Disconnect();
        }

        static void TestTerminalDriver()
        {
            TerminalDriver driver = new DotNetConsoleTerminal();
            driver.InstallInterruptHandler(new TestHandler(driver));
            driver.Connect();

            driver.SendChar('f');
            driver.SendChar('o');
            driver.SendChar('o');
            driver.SendNewLine();

            Thread.Sleep(100000);
            driver.Disconnect();
            
        }

        class TestHandler : TerminalInterruptHandler
        {
            private TerminalDriver mDriver;
            public TestHandler(TerminalDriver driver)
            {
                mDriver = driver;
            }
            public void HandleInterrupt(TerminalInterrupt interrupt)
            {
                switch (interrupt)
                {
                    // TODO: test other interupts
                    case TerminalInterrupt.CHAR:
                        Trace.WriteLine("Received character: " + mDriver.RecvChar());
                        break;

                    case TerminalInterrupt.ENTER:
                        Trace.WriteLine("Received ENTER");
                        break;

                    case TerminalInterrupt.BACK:
                        Trace.WriteLine("Received BACK");
                        break;

                    case TerminalInterrupt.CONNECT:
                        Trace.WriteLine("Received CONNECT");
                        break;

                }
                //stuff will happen here
            }
        }
    }
}
