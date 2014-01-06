using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    class Memory {

        private Word[] ram;

        public Memory() {
            ram = new Word[64000];
        }

        public Word this[DWord index] {
            get {
                return ram[index];
            }

            set {
                ram[index] = value;
            }

        }

    }
}
