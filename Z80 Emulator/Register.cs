using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.Z80.Emulator {


    class Register {
        public enum WordRegisterName { A, B, C, D, E, H, L, I, R }

        public enum DWordRegisterName { AF, BC, DE, HL, AF2, BC2, DE2, HL2, IX, IY, SP, PC }

        public enum StatusFlag { S, Z, I, H, P, C }

        private int[] wordRegister = new int[9];

        private int[] dWordRegister = new int[12];

        private bool[] statusRegister = new bool[6];

        public Word this[WordRegisterName index] {
            get {
                return (Word)wordRegister[(int)index];
            }
            set {
                wordRegister[(int)index] = value;
            }
        }

        public DWord this[DWordRegisterName index] {
            get {
                return (DWord)dWordRegister[(int)index];
            }
            set {
                dWordRegister[(int)index] = value;
            }
        }

        public bool this[StatusFlag index] {
            get {
                return statusRegister[(int)index];
            }
            set {
                statusRegister[(int)index] = value;
            }
        }
    }





    }
}
