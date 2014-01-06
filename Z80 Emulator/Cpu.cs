using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;
using System.Collections;



namespace eZet.i8080.Emulator {

    public class Cpu {

        public delegate void InstructionHandler();

        private StatusRegister flag;

        private Register reg;

        private Memory mem;

        private Word instructionRegister;

        private InstructionHandler[] opcode = new InstructionHandler[256];

        public Cpu() {
            reg = new Register();
            mem = new Memory();
            flag = new StatusRegister();

        }

        public void run() {

        }

        private void fetchNextInstruction() {
            instructionRegister = mem[reg.Pc++];
        }

        private void executeInstruction() {
            opcode[instructionRegister].Invoke();
        }

        public void setFlags(short value, params StatusFlag[] flags) {

            if (flags.Contains(StatusFlag.C)) {

            }

            if (flags.Contains(StatusFlag.P)) {
                if (evenParity(value)) {
                    flag.set(StatusFlag.P);
                } else {
                    flag.clear(StatusFlag.P);
                }
            }

            if (flags.Contains(StatusFlag.A)) {

            }

            if (flags.Contains(StatusFlag.Z)) {
                if (value == 0)
                    flag.set(StatusFlag.Z);
                else
                    flag.clear(StatusFlag.Z);
            }

            if (flags.Contains(StatusFlag.S)) {
                if (value < 0)
                    flag.set(StatusFlag.S);
                else
                    flag.clear(StatusFlag.S);
            }
        }

        public bool evenParity(int v) {
            v ^= v >> 16;
            v ^= v >> 8;
            v ^= v >> 4;
            v &= 0xf;
            return ((0x6996 >> v) & 1) == 0;
        }

        private void initInstructionMap() {

            // NOP
            opcode[0x00] = () => { };

            // LXI B, d16
            opcode[0x01] = () => {
                reg[SymRef.C] = mem[reg.Pc++];
                reg[SymRef.B] = mem[reg.Pc++];
            };

            // STAX B
            opcode[0x02] = () => {
                mem[reg[SymRefD.BC]] = reg[SymRef.A];
            };

            // INX B
            opcode[0x03] = () => {
                reg[SymRefD.BC]++;
            };

            // INR B
            opcode[0x04] = () => {
                reg[SymRef.B]++;
                setFlags(reg[SymRef.B]++);
                // TODO set flags
            };
        }


    }
}
