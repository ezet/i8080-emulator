using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;

namespace eZet.i8080.Emulator {

    [Flags]
    public enum StatusFlag {
        None = 0x0,
        C = 0x1,
        Reset = 0x2,
        P = 0x4,
        //Zero = 0x8
        A = 0x10,
        //Zero = 0x20
        Z = 0x40,
        S = 0x80,
        All = 0xff,
    }

    public class StatusRegister {

        public StatusFlag Register { get; private set; }

        public StatusRegister() {
            reset();
        }

        public bool get(params StatusFlag[] flagList) {
            bool retval = true;
            foreach (StatusFlag flag in flagList) {
                retval = Register.HasFlag(flag) ? true : false; 
            }
            return retval;
        }

        public Byte get() {
            Byte b = (Word)Register;
            // TODO Implement this
            return b;
        }

        public void set(params StatusFlag[] flagList) {
            foreach (StatusFlag flag in flagList) {
                Register = (Register | flag);
            }
        }

        public void clear(params StatusFlag[] flagList) {
            foreach (StatusFlag flag in flagList) {
                Register = (Register & ~flag);
            }
        }

        public void clear() {
            Register = StatusFlag.None;
        }

        public void reset() {
            Register = StatusFlag.Reset;
        }

        public void toggle(params StatusFlag[] flagList) {
            foreach (StatusFlag flag in flagList) {
                Register = (Register ^ flag);
            }
        }

        public void toggle() {
            Register = (Register ^ StatusFlag.All);
        }

        public void put(Word source, StatusFlag mask) {
            Register = (Register & ~mask) | ((StatusFlag)source & mask); 
        }

        public void put(Word mask) {
            Register = (StatusFlag)mask;
        }

    }
}
