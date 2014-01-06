using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZet.i8080.Emulator {
    class Program {
        static void Main(string[] args) {
            Cpu cpu = new Cpu();
            Console.Write(cpu.evenParity(4));
        }
    }
}
