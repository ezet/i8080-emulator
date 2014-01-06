using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using Word = System.Byte;
using DWord = System.UInt16;
using Flag = eZet.i8080.Emulator.StatusFlag;



namespace eZet.i8080.Emulator {

    public class Cpu {

        public delegate void InstructionHandler();

        private StatusRegister flag;

        private Register reg;

        private Memory mem;

        private Alu alu;

        private Word instructionRegister;

        private Word acc;

        private Word tmp;

        private InstructionHandler[] opcode = new InstructionHandler[256];

        public Cpu() {
            reg = new Register();
            mem = new Memory();
            flag = new StatusRegister();
            alu = new Alu();

        }

        public void run() {

        }

        private void fetchNextInstruction() {
            instructionRegister = mem[reg.Pc++];
        }

        private void executeInstruction() {
            try {
                opcode[instructionRegister].Invoke();
            } catch (System.OverflowException e) {

            }
        }

        private void setFlags(params StatusFlag[] flags) {
            if (flags.Contains(StatusFlag.C)) {
                if (alu.Carry) {
                    flag.set(StatusFlag.C);
                } else {
                    flag.clear(StatusFlag.C);
                }
            }

            if (flags.Contains(StatusFlag.P)) {
                if (evenParity(acc)) {
                    flag.set(StatusFlag.P);
                } else {
                    flag.clear(StatusFlag.P);
                }
            }

            if (flags.Contains(StatusFlag.A)) {

            }

            if (flags.Contains(StatusFlag.Z)) {
                if (acc == 0)
                    flag.set(StatusFlag.Z);
                else
                    flag.clear(StatusFlag.Z);
            }

            if (flags.Contains(StatusFlag.S)) {
                //flag.put(value, StatusFlag.S);
                if ((acc & 0xff) != 0)
                    flag.set(StatusFlag.S);
                else
                    flag.clear(StatusFlag.S);
            }
        }

        private bool evenParity(int v) {
            v ^= v >> 16;
            v ^= v >> 8;
            v ^= v >> 4;
            v &= 0xf;
            return ((0x6996 >> v) & 1) == 0;
        }

        private void nop() { }

        private void initInstructionMap() {

            // NOP
            opcode[0x00] = nop;

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
                acc = reg[SymRef.B];
                acc = alu.add(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.B] = acc;
            };
            
            // DCR B
            opcode[0x05] = () => {
                acc = reg[SymRef.B];
                acc = alu.sub(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.B] = acc;
            };

            // MVI B, d8
            opcode[0x06] = () => {
                reg[SymRef.B] = mem[reg.Pc++];
            };

            // RLC
            opcode[0x07] = () => {
                acc = reg[SymRef.A];
                acc = alu.rotateLeft(acc, 1);
                flag.put(tmp, Flag.C);
            };
            
            // NOP
            opcode[0x08] = nop;

            // DAD B
            opcode[0x09] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.BC]);
                setFlags(Flag.C);
            };

            // LDAX B
            opcode[0x0a] = () => {
                reg[SymRef.A] = mem.[reg[SymRefD.BC]];
            };

            // DCX B
            opcode[0x0b] = () => {
                reg[SymRefD.BC] = alu.sub(reg[SymRefD.BC], 1);
            };





        }
     


    }
}
