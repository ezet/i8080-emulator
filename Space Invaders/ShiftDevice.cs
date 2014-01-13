using eZet.i8080.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;
using System.Diagnostics;

namespace eZet.i8080.Games.SpaceInvaders {
    public class ShiftDevice : IInputDevice, IOutputDevice {
        private int shiftOffset;
        private DWord shiftData;

        public ShiftDevice() {
            shiftOffset = 0;
            shiftData = 0;

        }

        public Word read(DWord adr) {
            return (Word)(shiftData >> (8 - shiftOffset));
        }

        public void write(DWord adr, Word data) {
            if (adr == 2) {
                shiftOffset = data & 0x7;
            } else if (adr == 4) {
                shiftData = (DWord)(shiftData >> 8 | data << 8);
            }
        }
    }


}
