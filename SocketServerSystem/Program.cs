using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            //new TCP(4860);
            new UDP(4868);
            UDP.script.ServerStart();
            Console.Read();
        }
    }
}
