// Assignment 4
// Pete Myers
// OIT, Spring 2017

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShell
{
    public interface Shell
    {
        void Run(Terminal terminal);
    }

    public interface ShellFactory
    {
        Shell CreateShell(string name, Session session);
    }
}
