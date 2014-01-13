using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    public class BusEventArgs : EventArgs {

        public Word Data { get; private set; }

        public DWord Adr { get; private set; }

        public BusEventArgs(DWord adr) {
            this.Adr = adr;
        }

        public BusEventArgs(DWord adr, Word data) {
            this.Adr = adr;
            this.Data = data;
        }



    }
}