using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {

    public enum SymRef { A, F, B, C, D, E, H, L, A2, F2, B2, C2, D2, E2, H2, L2, I = 25, R = 26 }

    public enum SymRefD { AF, BC, DE, HL, AF2, BC2, DE2, HL2, IX, IY, SP, PC }


    public class Register {

        private unsafe struct FixedBuffer {
            public fixed Word buffer[27];
        }

        private FixedBuffer mainRegister = default(FixedBuffer);

        private unsafe DWord* dWordPtr;


        public unsafe Register() {
            fixed (Word* ptr = mainRegister.buffer) {
                dWordPtr = (DWord*)ptr;
            }
        }

        public unsafe Word this[SymRef index] {
            get {
                fixed (Word* ptr = mainRegister.buffer) {
                    return ptr[(int)index];
                }
            }
            set {
                fixed (Word* ptr = mainRegister.buffer) {
                    ptr[(int)index] = value;
                }
            }
        }

        public unsafe DWord this[SymRefD index] {
            get {
                fixed (Word* ptr = mainRegister.buffer) {
                    return dWordPtr[(int)index];
                }
            }
            set {
                fixed (Word* ptr = mainRegister.buffer) {
                    dWordPtr[(int)index] = value;
                }
            }
        }

   
    }





}
