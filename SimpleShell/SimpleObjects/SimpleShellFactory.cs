using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleShell
{
    class SimpleShellFactory : ShellFactory
    {
        public Shell CreateShell(string name, Session session)
        {
            if(name == "pshell")
            {
                return new SimpleShell(session);
            }

            return null;
        }
    }
}
