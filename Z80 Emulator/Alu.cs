using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    class Alu {

        private int tmp;
        public bool Carry { get; private set; }

        public Alu() {
        }

        public Word add(Word lhs, Word rhs) {
            tmp = lhs + rhs;
            checkCarry(Word.MinValue, Word.MaxValue);
            return (Word)tmp;
        }

        public Word sub(Word lhs, Word rhs) {
            tmp = lhs - rhs;
            checkCarry(Word.MinValue, Word.MaxValue);
            return (Word)tmp; ;
        }

        public DWord add(DWord lhs, DWord rhs) {
            tmp = lhs + rhs;
            checkCarry(DWord.MinValue, DWord.MaxValue);
            return (DWord)tmp;
        }

        public DWord sub(DWord lhs, DWord rhs) {
            tmp = lhs - rhs;
            checkCarry(DWord.MinValue, DWord.MaxValue);
            return (DWord)tmp;
        }

        public Word shiftLeft(Word value, int count) {
            tmp = value << count;
            return (Word)tmp;
        }
        public Word shiftRight(Word value, int count) {
            tmp = value >> count;
            return (Word)tmp;
        }


        public Word rotateCarryLeft(Word value, int count, bool carry) {
            // TODO implement.
            throw new NotImplementedException("rotateCarryLeft");
            return (Word)((value << count) | (value >> (9 - count)));
        }

        public Word rotateCarryRight(Word value, int count) {
            // TODO implement
            throw new NotImplementedException("rotateCarryRight");
            return (Word)((value >> count) | (value << (8 - count)));
        }

        public Word rotateLeft(Word value, int count) {
            return (Word)((value << count) | (value >> (8 - count)));
        }

        public Word rotateRight(Word value, int count) {
            return (Word)((value >> count) | (value << (8 - count)));
        }

        private void checkCarry(int min, int max) {
            if (tmp < min || tmp > max) {
                Carry = true;
            } else {
                Carry = false;
            }
        }

    }
}
