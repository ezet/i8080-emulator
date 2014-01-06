using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;

namespace eZet.i8080.Emulator {

    [Flags]
    public enum StatusFlag {
        None = 0x00,
        C = 0x1,
        Reset = 0x2,
        P = 0x4,
        Null = 0x8,
        A = 0x10,
        Null = 0x20,
        Z = 0x40,
        S = 0x80,
        All = 0xff,
    }

    public struct StatusRegister {

        public StatusFlag Register {get; private set;}

        public bool get(StatusFlag flag) {
            return Register.HasFlag(flag);
        }

        public void set(StatusFlag flag) {
            Register = (Register | flag);
        }

        public void clear(StatusFlag flag) {
            Register = (Register & ~flag);
        }

        public void clear() {
            Register = StatusFlag.Reset;
        }

        public void reset() {
            Register = StatusFlag.Reset;
        }

        public void toggle(StatusFlag flag) {
            Register = (Register ^ flag);
        }

        public void toggle() {
            Register = (Register ^ StatusFlag.All);
        }

        public void put(Word mask, StatusFlag flag) {
            Register = (((StatusFlag)mask & flag) | Register);
        }

        public void put(Word mask) {
            Register = (StatusFlag)mask;
        }

 





    }
}
