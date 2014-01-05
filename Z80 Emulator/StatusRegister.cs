using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZet.i8080.Emulator {

    [Flags]
    public enum StatusFlag {
        None = 0x00,
        C = 0x1,
        P = 0x4,
        A = 0x10,
        Z = 0x40,
        S = 0x80,
        All = 0xff,
    }

    public struct StatusRegister {

        private StatusFlag register;

        public bool get(StatusFlag flag) {
            return register.HasFlag(flag);
        }

        public void set(StatusFlag flag) {
            register = (register | flag);
        }

        public void clear(StatusFlag flag) {
            register = (register & ~flag);
        }

        public void clear() {
            register = (register & StatusFlag.None);
        }


        public void toggle(StatusFlag flag) {
            register = (register ^ flag);
        }

        public void toggle() {
            register = (register ^ StatusFlag.All);
        }

    }
}
