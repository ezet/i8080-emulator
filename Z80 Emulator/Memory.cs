using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;
using System.IO;

namespace eZet.i8080.Emulator {
    class Memory {

        private Word[] ram;

        public DWord CodeStart { get; private set; }
        public DWord Capacity { get; private set; }

        public Memory() {
            Capacity = 64 * 1024 - 1;
            ram = new Word[Capacity];
            CodeStart = 0x2000;

        }

        public Word this[DWord index] {
            get {
                return ram[index];
            }

            set {
                ram[index] = value;
            }

        }

        public void storeProgramCode(MemoryStream stream) {
            stream.ToArray().CopyTo(ram, CodeStart);
        }

    }
}
