// Assignment 4
// Pete Myers
// OIT, Spring 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSystemCSharp;

namespace SimpleShell
{
    public class SimpleShell : Shell
    {
        private Session session;
        private Directory pwd;

        public SimpleShell(Session session)
        {
            this.session = session;
            this.pwd = session.HomeDirectory;
        }

        public void Run(Terminal terminal)
        {
            // NOTE: takes over the current thread, returns only when shell exits
            // expects terminal to already be connected

            terminal.Echo = true;

            while (true)
            {
                //print command prompt
                terminal.Write($"{pwd.Name}> ");

                //get command line
                string cmdline = terminal.ReadLine();
                if (cmdline.Length == 0)
                {
                    continue;
                }
                   

                // identify command
                string[] parts = cmdline.Split(' ');
                if(parts != null && parts.Length > 0)
                {
                    string cmd = parts[0];
                    if(cmd == "exit")
                    {
                        terminal.WriteLine("Bye!");                   
                        break;
                    }
                    else if(cmd == "pwd")
                    {
                        terminal.WriteLine(pwd.FullPathName);
                    }
                    else if(cmd == "ls")
                    {                       
                        foreach(FSEntry entry in pwd.GetSubDirectories())
                        {
                            terminal.WriteLine(entry.Name);
                        }
                        foreach(FSEntry entry in pwd.GetFiles())
                        {
                            terminal.WriteLine(entry.Name);
                        }
                    }
                    else if(cmd == "cd")
                    {
                        if (parts.Length == 2)
                        {
                            string dest = parts[1];
                            if (!dest.StartsWith("/"))
                                dest = pwd.FullPathName + "/" + dest;

                            FSEntry found = session.FileSystem.Find(dest);
                            if(found != null)
                            {
                                if (found.IsDirectory)
                                {
                                    pwd = (Directory)found;
                                }
                                else
                                {
                                    terminal.WriteLine("Error: " + dest + " is not a directory!");
                                }
                            }
                        }
                        else
                        {
                            terminal.WriteLine("Usage: cd <name>");
                        }
                    }
                    else if(cmd == "mkdir")
                    {
                        if(parts.Length == 2)
                        {
                            //validate commandline args...

                            //make dir
                            pwd.CreateDirectory(parts[1]);
                        }
                    }
                    else
                    {
                        terminal.WriteLine("Command not found!");
                    }
                }
                //run command
            }
        }
    }
}
