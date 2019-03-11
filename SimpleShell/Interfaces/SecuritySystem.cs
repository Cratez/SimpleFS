// Assignment 4
// Pete Myers
// OIT, Spring 2017

using System;
using System.Collections.Generic;

namespace SimpleShell
{
    public interface SecuritySystem
    {
        int AddUser(string user);
        bool NeedsPassword(string user);
        void SetPassword(string user, string password);
        int Authenticate(string user, string password);

        //user info
        string UserName(int userID);
        string UserHomeDirectory(int userID);
        string UserPreferredShell(int userID);
    }
}
