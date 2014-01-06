using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {

    public enum SymRef { A, F, B, C, D, E, H, L}

    public enum SymRefD { AF, BC, DE, HL}


    public class Register {

        private unsafe struct FixedBuffer {
            public fixed Word buffer[8];
        }

        private FixedBuffer mainRegister = default(FixedBuffer);

        private unsafe DWord* dWordPtr;

        public DWord Pc { get; set; }
        public DWord Sp { get; set; }


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
