// Assignment 4
// Pete Myers
// OIT, Spring 2017

using System;
using FileSystemCSharp;


namespace SimpleShell
{
    public interface Session
    {
        int UserID { get; }
        string Username { get; }
        Terminal Terminal { get; }
        Shell Shell { get; }
        Directory HomeDirectory { get; }

        FileSystem FileSystem { get; }
        void Run();
        void Logout();
    }

    public interface SessionManager
    {
        Session NewSession(Terminal terminal);
    }
}
