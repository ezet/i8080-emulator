using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;


namespace eZet.i8080.Emulator {

    public class Cpu {

        private StatusRegister flag;

        private Register reg;

        private Memory mem;

        private Word instructionRegister;



        public Cpu() {
            reg = new Register();
            mem = new Memory();

        }

        public void run() {

        }

        private void fetchNextInstruction() {

        }



    }
}
