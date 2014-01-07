using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using Word = System.Byte;
using DWord = System.UInt16;
using Flag = eZet.i8080.Emulator.StatusFlag;
using System.IO;



namespace eZet.i8080.Emulator {

    public class Cpu {

        public delegate void InstructionHandler();

        private StatusRegister flags;

        private Register reg;

        private Memory mem;

        private Alu alu;

        private Word instructionRegister;

        private Word acc;

        private bool halt;

        private int cycles;

        //private Word tmp;

        private InstructionHandler[] opcode = new InstructionHandler[256];

        public Cpu() {
            reg = new Register();
            mem = new Memory();
            flags = new StatusRegister();
            alu = new Alu();
            initOpcodeMap();
        }

        public void execute() {
            initialize();
            reg.Pc = mem.CodeStart;
            while (reg.Pc < 64000 && !halt && cycles < 10) {
                fetchNextInstruction();
                executeInstruction();
                ++cycles;
            }
        }

        public void loadProgram(Stream input) {
            using (var ms = new MemoryStream()) {
                input.CopyTo(ms);
                mem.storeProgramCode(ms);
            }
        }

        private void initialize() {
            flags.reset();
            alu.reset();
            halt = false;
            cycles = 0;
        }

        private void fetchNextInstruction() {
            instructionRegister = mem[reg.Pc++];
        }

        private void executeInstruction() {
            try {
                opcode[instructionRegister].Invoke();
                Console.WriteLine("Executed: 0x{0:X}", instructionRegister);
            } catch (NullReferenceException e) {
                Console.WriteLine("Opcode not recognized: 0x{0:X} ", instructionRegister);
            }
        }

        private void setFlags(params StatusFlag[] flagList) {
            if (flagList.Contains(StatusFlag.C)) {
                if (alu.Carry) {
                    flags.set(StatusFlag.C);
                } else{ 
                    flags.clear(StatusFlag.C);
                }
            }

            if (flagList.Contains(StatusFlag.P)) {
                if (evenParity(acc)) {
                    flags.set(StatusFlag.P);
                } else {
                    flags.clear(StatusFlag.P);
                }
            }

            if (flagList.Contains(StatusFlag.A)) {

            }

            if (flagList.Contains(StatusFlag.Z)) {
                if (acc == 0)
                    flags.set(StatusFlag.Z);
                else
                    flags.clear(StatusFlag.Z);
            }

            if (flagList.Contains(StatusFlag.S)) {
                //flag.put(value, StatusFlag.S);
                if ((acc & 0xff) != 0)
                    flags.set(StatusFlag.S);
                else
                    flags.clear(StatusFlag.S);
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

        private void initOpcodeMap() {

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
                reg[SymRefD.BC] = alu.add(reg[SymRefD.BC], 1);
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
                flags.put(acc, Flag.C);
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
                reg[SymRef.A] = mem[reg[SymRefD.BC]];
            };

            // DCX B
            opcode[0x0b] = () => {
                reg[SymRefD.BC] = alu.sub(reg[SymRefD.BC], 1);
            };

            // INR C
            opcode[0x0c] = () => {
                acc = reg[SymRef.C];
                acc = alu.add(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.C] = acc;
            };

            // DCR C
            opcode[0x0d] = () => {
                acc = reg[SymRef.C];
                acc = alu.sub(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.C] = acc;
            };

            // MVI C, d8
            opcode[0x0e] = () => {
                reg[SymRef.C] = mem[reg.Pc++];
            };

            // RRC
            opcode[0x0f] = () => {
                acc = reg[SymRef.A];
                flags.put(acc, Flag.C);
                acc = alu.rotateRight(acc, 1);
            };

            // NOP
            opcode[0x10] = nop;

            // LXI D, d16
            opcode[0x11] = () => {
                reg[SymRef.E] = mem[reg.Pc++];
                reg[SymRef.D] = mem[reg.Pc++];
            };

            // STAX D
            opcode[0x12] = () => {
                mem[reg[SymRefD.DE]] = reg[SymRef.A];
            };

            //INX D
            opcode[0x13] = () => {
                reg[SymRefD.DE] = alu.add(reg[SymRefD.DE], 1);
            };

            // INR D
            opcode[0x14] = () => {
                acc = reg[SymRef.D];
                acc = alu.add(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.D] = acc;
            };

        // DCR D
            opcode[0x15] = () => {
                acc = reg[SymRef.D];
                acc = alu.sub(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.D] = acc;
            };

            // MVI D, d8
            opcode[0x16] = () => {
                reg[SymRef.D] = mem[reg.Pc++];
            };

            // RAL
            opcode[0x17] = () => {
                acc = reg[SymRef.A];
                flags.put((Word)(acc >> 7), Flag.C);
                acc = alu.rotateCarryLeft(acc, 1, flags.get(Flag.C));
            };

            // NOP
            opcode[0x18] = nop;

            // DAD D
            opcode[0x19] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.DE]);
                setFlags(Flag.C);
            };




        }



    }
}
