using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZet.i8080.Emulator {
    class Program {
        static void Main(string[] args) {
            Cpu cpu = new Cpu();
            using (var fs = File.OpenRead("../../../invaders rom/invaders.h")) {
                cpu.loadProgram(fs);
                cpu.execute();
            }

        }
    }
}
