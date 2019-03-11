// Assignment 4
// Pete Myers
// OIT, Spring 2017

using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleShell
{
    public class SimpleSecurity : SecuritySystem
    {
        private class User
        {
            public int UserID;
            public string UserName;
            public string Password;
            public string HomeDirectory;
            public string Shell;
        }

        private int nextUserID;
        private Dictionary<int, User> usersById;        // UserID -> User

        public SimpleSecurity()
        {
            nextUserID = 1;
            usersById = new Dictionary<int, User>();
        }

        private User UserByName(string username)
        {
            return usersById.Values.FirstOrDefault(u => u.UserName == username);
        }

        public int AddUser(string username)
        {
            if (UserByName(username) != null)
                throw new Exception("Username " + username + " already exists!");

            int userID = nextUserID++;

            User user = new User {
                UserID = userID,
                UserName = username,
                HomeDirectory = "/users/" + username,
                Shell = "pshell",
                Password = null
                
            };

            usersById[userID] = user;

            return userID;
        }

        public bool NeedsPassword(string username)
        {
            User user = UserByName(username);
            if (user == null)
                throw new Exception("User " + username + " doesn't exist!");
               
            return user.Password == null;
        }

        public void SetPassword(string username, string password)
        {
            User user = UserByName(username);
            if (user == null)
                throw new Exception("User " + username + " doesn't exist!");

            user.Password = password;
        }

        public int Authenticate(string username, string password)
        {
            User user = UserByName(username);
            if (user == null)
                throw new Exception("User " + username + " doesn't exist!");

            if (user.Password != password)
                throw new Exception("Invalid username/password combo!");

            return user.UserID;
        }

        public string UserName(int userID)
        {
            if (!usersById.ContainsKey(userID))
                throw new Exception("UserId " + userID.ToString() + " doesn't exist!");


            return usersById[userID].UserName;
        }

        public string UserHomeDirectory(int userID)
        {
            if (!usersById.ContainsKey(userID))
                throw new Exception("UserId " + userID.ToString() + " doesn't exist!");


            return usersById[userID].HomeDirectory;
        }

        public string UserPreferredShell(int userID)
        {
            if (!usersById.ContainsKey(userID))
                throw new Exception("UserId " + userID.ToString() + " doesn't exist!");


            return usersById[userID].Shell;
        }
    }
}
