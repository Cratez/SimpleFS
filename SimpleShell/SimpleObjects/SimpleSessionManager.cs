using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSystemCSharp;

namespace SimpleShell
{
    public class SimpleSessionManager : SessionManager
    {
        private class SimpleSession : Session
        {
            private int userID;
            private SecuritySystem security;
            private FileSystem filesystem;
            private Directory homeDir;
            private Shell shell;
            private Terminal terminal;


            public SimpleSession(SecuritySystem security, FileSystem filesystem, ShellFactory shells, Terminal terminal, int userID)
            {
                this.security = security;
                this.filesystem = filesystem;
                this.terminal = terminal;
                this.userID = userID;

                //get users home dir
                string homepath = security.UserHomeDirectory(userID);
                this.homeDir = (Directory)filesystem.Find(homepath);
                if (this.homeDir == null)
                    throw new Exception("Failed to home users home directory: " + homepath);

                //get shell
                string shellname = security.UserPreferredShell(userID);
                shell = shells.CreateShell(shellname,this);
                if(shell == null)
                    throw new Exception("Failed to find user's preferred shell");
                //TODO  get the shell from a factory
            }

            public int UserID => userID;
            public string Username => security.UserName(userID);
            public Terminal Terminal => terminal;
            public Shell Shell => shell;
            public Directory HomeDirectory => homeDir;

            public FileSystem FileSystem => filesystem;

            public void Run()
            {
                shell.Run(terminal);
            }
            public void Logout()
            {
                //TODO
            }
        }

        //
        //
        //
        private SecuritySystem security;
        private FileSystem filesystem;
        private ShellFactory shells;

        public SimpleSessionManager(SecuritySystem security, FileSystem filesystem, ShellFactory shells)
        {
            this.security = security;
            this.filesystem = filesystem;
            this.shells = shells;
        }


        public Session NewSession(Terminal terminal)
        {
            const int MAX_TRIES = 3;

            // ask the user to login      
            for (int attempts = 0; attempts < MAX_TRIES; attempts++)
            {
                try
                {
                    //prompt for username
                    terminal.Write("Username: ");
                    terminal.Echo = true;
                    string username = terminal.ReadLine();

                    //TODO: determine if we need to prompt for password
                    if (security.NeedsPassword(username))
                    {
                        //prompt for password
                        terminal.Write("Set Password: ");
                        terminal.Echo = false;
                        string newpassword = terminal.ReadLine();
                        terminal.WriteLine();

                        //set password
                        security.SetPassword(username, newpassword);

                        attempts = -1; //give them three attempts
                        continue;
                    }

                    //prompt for password
                    terminal.Write("Password: ");
                    terminal.Echo = false;
                    string password = terminal.ReadLine();
                    terminal.WriteLine();
                    security.SetPassword(username, password);


                    //authentic user
                    int userId = security.Authenticate(username, password);

                    terminal.WriteLine("Welcome! " + security.UserName(userId));


                    return new SimpleSession(security, this.filesystem, this.shells, terminal, userId);
                }
                catch (Exception ex)
                {
                    terminal.WriteLine("Nope!");
                } 
            }


            terminal.WriteLine("Failed login attempts!");
            return null;
        }
    }
}
