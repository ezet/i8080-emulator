using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {

    public enum SymRef { A, F, B, C, D, E, H, L }

    public enum SymRefD { PSW, BC = 2, DE = 4, HL = 6, M = 6 }


    public class Register {

        private Word[] buffer = new Word[8];

        public DWord Pc { get; set; }

        public DWord Sp { get; set; }

        public Register() {
        }

        public Word this[SymRef index] {
            get {
                return buffer[(int)index];
            }
            set {
                buffer[(int)index] = value;
            }
        }

        public DWord this[SymRefD index] {
            get {
                return (DWord)(buffer[(int)index] << 8 | buffer[(int)index + 1]);
            }
            set {
                buffer[(int)index] = (Word)(value >> 8);
                buffer[(int)index + 1] = (Word)value;
            }
        }
    }
}