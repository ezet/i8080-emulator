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
                Console.WriteLine("Opcode not recognized: 0x{0:X} : {1} ", instructionRegister, e.Message);
            }
        }

        private void setFlags(params StatusFlag[] flagList) {
            setFlags(acc, flagList);
        }

        private void setFlags(DWord value, params StatusFlag[] flagList) {
            if (flagList.Contains(StatusFlag.C)) {
                if (alu.Carry) {
                    flags.set(StatusFlag.C);
                } else {
                    flags.clear(StatusFlag.C);
                }
            }

            if (flagList.Contains(StatusFlag.P)) {
                if (evenParity(value)) {
                    flags.set(StatusFlag.P);
                } else {
                    flags.clear(StatusFlag.P);
                }
            }

            if (flagList.Contains(StatusFlag.A)) {

            }

            if (flagList.Contains(StatusFlag.Z)) {
                if (value == 0)
                    flags.set(StatusFlag.Z);
                else
                    flags.clear(StatusFlag.Z);
            }

            if (flagList.Contains(StatusFlag.S)) {
                //flag.put(value, StatusFlag.S);
                if ((value & 0xff) != 0)
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

            // LDAX D
            opcode[0x1a] = () => {
                reg[SymRef.A] = mem[reg[SymRefD.DE]];
            };

            // DCX D
            opcode[0x1b] = () => {
                reg[SymRefD.DE] = alu.sub(reg[SymRefD.DE], 1);
            };

            // INR E
            opcode[0x1c] = () => {
                acc = reg[SymRef.E];
                acc = alu.add(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.E] = acc;
            };

            // DCR E
            opcode[0x1d] = () => {
                acc = reg[SymRef.E];
                acc = alu.sub(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.E] = acc;
            };

            // MVI E, d8
            opcode[0x1e] = () => {
                reg[SymRef.E] = mem[reg.Pc++];
            };

            // RAR
            opcode[0x1f] = () => {
                acc = reg[SymRef.A];
                flags.put(acc, Flag.C);
                acc = alu.rotateCarryRight(acc, 1, flags.get(Flag.C));
            };

            // RIM, 8085 only
            opcode[0x20] = nop;

            // LXI H, d16
            opcode[0x21] = () => {
                reg[SymRef.L] = mem[reg.Pc++];
                reg[SymRef.H] = mem[reg.Pc++];
            };

            // SHLD adr
            opcode[0x22] = () => {
                DWord adr = mem[reg.Pc++];
                mem[adr++] = reg[SymRef.L];
                mem[adr] = reg[SymRef.H];
            };

            // INX H
            opcode[0x23] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], 1);
            };

            // INR H
            opcode[0204] = () => {
                acc = reg[SymRef.H];
                acc = alu.add(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.H] = acc;
            };

            // DCR H
            opcode[0x05] = () => {
                acc = reg[SymRef.H];
                acc = alu.sub(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.H] = acc;
            };

            // MVI H, d8
            opcode[0x26] = () => {
                reg[SymRef.H] = mem[reg.Pc++];
            };

            // DAA
            opcode[0x27] = null;

            // NOP
            opcode[0x28] = nop;

            // DAD H
            opcode[0x29] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.HL]);
                setFlags(Flag.C);
            };

            // LHLD adr
            opcode[0x2a] = () => {
                DWord adr = mem[reg.Pc++];
                reg[SymRef.L] = mem[adr++];
                reg[SymRef.H] = mem[adr];
            };

            // DCX H
            opcode[0x2b] = () => {
                reg[SymRefD.HL] = alu.sub(reg[SymRefD.HL], 1);
            };

            // INR L
            opcode[0x2c] = () => {
                acc = reg[SymRef.L];
                acc = alu.add(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.L] = acc;
            };

            // DCR L
            opcode[0x2d] = () => {
                acc = reg[SymRef.L];
                acc = alu.sub(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.L] = acc;
            };

            // MVI L, d8
            opcode[0x2e] = () => {
                reg[SymRef.L] = mem[reg.Pc++];
            };

            // CMA
            opcode[0x2f] = () => {
                reg[SymRef.A] = (byte)~reg[SymRef.A];
            };

            // SIM, i8085
            opcode[0x30] = null;

            // LXI SP, d16
            opcode[0x31] = () => {
                DWord tmp = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
            };

            // STA adr
            opcode[0x32] = () => {
                mem[mem[reg.Pc++]] = reg[SymRef.A];
            };

            // INX SP
            opcode[0x33] = () => {
                reg.Sp = alu.add(reg.Sp, 1);
            };

            // INR M
            opcode[0x34] = () => {
               reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], 1);
               setFlags(reg[SymRefD.HL], Flag.Z, Flag.S, Flag.P, Flag.A);
            };

            // INR M
            opcode[0x35] = () => {
                reg[SymRefD.HL] = alu.sub(reg[SymRefD.HL], 1);
                setFlags(reg[SymRefD.HL], Flag.Z, Flag.S, Flag.P, Flag.A);
            };


            // MVI M, d8
            opcode[0x36] = () => {
                reg[SymRefD.HL] = mem[reg.Pc++];
            };

            // STC
            opcode[0x37] = () => {
                flags.set(Flag.C);
            };

            // NOP
            opcode[0x38] = nop;

            // DAD SP
            opcode[0x39] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg.Sp);
            };

            // LDA adr
            opcode[0x3a] = () => {
                DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                reg[SymRef.A] = mem[adr];
            };

            // DCX SP
            opcode[0x3b] = () => {
                reg.Sp = alu.sub(reg.Sp, 1);
            };

            // INR A
            opcode[0x3c] = () => {
                acc = reg[SymRef.A];
                acc = alu.add(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.A] = acc;
            };



            // DCR A
            opcode[0x3d] = () => {
                acc = reg[SymRef.A];
                acc = alu.sub(acc, 1);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.A] = acc;
            };

            // MVI A, d8
            opcode[0x3e] = () => {
                reg[SymRef.A] = mem[reg.Pc++];
            };

            // CMC
            opcode[0x3f] = () => {
                flags.toggle(Flag.C);
            };

            // MOV B, B
            opcode[0x40] = () => {
                reg[SymRef.B] = reg[SymRef.B];
            };

            // MOV B, C
            opcode[0x41] = () => {
                reg[SymRef.B] = reg[SymRef.C];
            };

            // MOV B, D
            opcode[0x42] = () => {
                reg[SymRef.B] = reg[SymRef.D];
            };

            // MOV B, E
            opcode[0x43] = () => {
                reg[SymRef.B] = reg[SymRef.E];
            };

            // MOV B, H
            opcode[0x44] = () => {
                reg[SymRef.B] = reg[SymRef.H];
            };

            // MOV B, L
            opcode[0x45] = () => {
                reg[SymRef.B] = reg[SymRef.L];
            };

            // MOV B, M
            opcode[0x46] = () => {
                reg[SymRef.B] = (Word)reg[SymRefD.HL];
            };

            // MOV B, A
            opcode[0x47] = () => {
                reg[SymRef.B] = reg[SymRef.A];
            };

            // MOV C, B
            opcode[0x48] = () => {
                reg[SymRef.C] = reg[SymRef.B];
            };

            // MOV C, C
            opcode[0x49] = () => {
                reg[SymRef.C] = reg[SymRef.C];
            };

            // MOV C, D
            opcode[0x4a] = () => {
                reg[SymRef.C] = reg[SymRef.D];
            };

            // MOV C, E
            opcode[0x4b] = () => {
                reg[SymRef.C] = reg[SymRef.E];
            };

            // MOV C, H
            opcode[0x4c] = () => {
                reg[SymRef.C] = reg[SymRef.H];
            };

            // MOV C, L
            opcode[0x4d] = () => {
                reg[SymRef.C] = reg[SymRef.L];
            };

            // MOV C, M
            opcode[0x4e] = () => {
                reg[SymRef.C] = (Word)reg[SymRefD.HL];
            };

            // MOV C, A
            opcode[0x4f] = () => {
                reg[SymRef.C] = reg[SymRef.A];
            };

            // MOV D, B
            opcode[0x50] = () => {
                reg[SymRef.D] = reg[SymRef.B];
            };

            // MOV D, C
            opcode[0x51] = () => {
                reg[SymRef.D] = reg[SymRef.C];
            };

            // MOV D, D
            opcode[0x52] = () => {
                reg[SymRef.D] = reg[SymRef.D];
            };

            // MOV D, E
            opcode[0x53] = () => {
                reg[SymRef.D] = reg[SymRef.E];
            };

            // MOV D, H
            opcode[0x54] = () => {
                reg[SymRef.D] = reg[SymRef.H];
            };

            // MOV D, L
            opcode[0x55] = () => {
                reg[SymRef.D] = reg[SymRef.L];
            };

            // MOV D, M
            opcode[0x56] = () => {
                reg[SymRef.D] = (Word)reg[SymRefD.HL];
            };

            // MOV D, A
            opcode[0x57] = () => {
                reg[SymRef.D] = reg[SymRef.A];
            };

            // MOV E, B
            opcode[0x58] = () => {
                reg[SymRef.E] = reg[SymRef.B];
            };

            // MOV E, C
            opcode[0x59] = () => {
                reg[SymRef.E] = reg[SymRef.C];
            };

            // MOV E, D
            opcode[0x5a] = () => {
                reg[SymRef.E] = reg[SymRef.D];
            };

            // MOV E, E
            opcode[0x5b] = () => {
                reg[SymRef.E] = reg[SymRef.E];
            };

            // MOV E, H
            opcode[0x5c] = () => {
                reg[SymRef.E] = reg[SymRef.H];
            };

            // MOV E, L
            opcode[0x5d] = () => {
                reg[SymRef.E] = reg[SymRef.L];
            };

            // MOV E, M
            opcode[0x5e] = () => {
                reg[SymRef.E] = (Word)reg[SymRefD.HL];
            };

            // MOV E, A
            opcode[0x5f] = () => {
                reg[SymRef.E] = reg[SymRef.A];
            };

            // MOV H, B
            opcode[0x60] = () => {
                reg[SymRef.H] = reg[SymRef.B];
            };

            // MOV H, C
            opcode[0x61] = () => {
                reg[SymRef.H] = reg[SymRef.C];
            };

            // MOV H, D
            opcode[0x62] = () => {
                reg[SymRef.H] = reg[SymRef.D];
            };

            // MOV H, E
            opcode[0x63] = () => {
                reg[SymRef.H] = reg[SymRef.E];
            };

            // MOV H, H
            opcode[0x64] = () => {
                reg[SymRef.H] = reg[SymRef.H];
            };

            // MOV H, L
            opcode[0x65] = () => {
                reg[SymRef.H] = reg[SymRef.L];
            };

            // MOV H, M
            opcode[0x66] = () => {
                reg[SymRef.H] = (Word)reg[SymRefD.HL];
            };

            // MOV H, A
            opcode[0x67] = () => {
                reg[SymRef.H] = reg[SymRef.A];
            };

            // MOV L, B
            opcode[0x68] = () => {
                reg[SymRef.L] = reg[SymRef.B];
            };

            // MOV L, C
            opcode[0x69] = () => {
                reg[SymRef.L] = reg[SymRef.C];
            };

            // MOV L, D
            opcode[0x6a] = () => {
                reg[SymRef.L] = reg[SymRef.D];
            };

            // MOV L, E
            opcode[0x6b] = () => {
                reg[SymRef.L] = reg[SymRef.E];
            };

            // MOV L, H
            opcode[0x6c] = () => {
                reg[SymRef.L] = reg[SymRef.H];
            };

            // MOV L, L
            opcode[0x6d] = () => {
                reg[SymRef.L] = reg[SymRef.L];
            };

            // MOV L, M
            opcode[0x6e] = () => {
                reg[SymRef.L] = (Word)reg[SymRefD.HL];
            };

            // MOV L, A
            opcode[0x6f] = () => {
                reg[SymRef.L] = reg[SymRef.A];
            };

            // MOV M, B
            opcode[0x70] = () => {
                reg[SymRefD.M] = reg[SymRef.B];
            };

            // MOV M, C
            opcode[0x71] = () => {
                reg[SymRefD.M] = reg[SymRef.C];
            };

            // MOV M, D
            opcode[0x72] = () => {
                reg[SymRefD.M] = reg[SymRef.D];
            };

            // MOV M, E
            opcode[0x73] = () => {
                reg[SymRefD.M] = reg[SymRef.E];
            };

            // MOV M, H
            opcode[0x74] = () => {
                reg[SymRefD.M] = reg[SymRef.H];
            };

            // MOV M, L
            opcode[0x75] = () => {
                reg[SymRefD.M] = reg[SymRef.L];
            };

            // HLT
            opcode[0x76] = () => {
                halt = true;
            };

            // MOV M, A
            opcode[0x77] = () => {
                reg[SymRefD.M] = reg[SymRef.A];
            };

            // MOV A, B
            opcode[0x78] = () => {
                reg[SymRef.A] = reg[SymRef.B];
            };

            // MOV A, C
            opcode[0x79] = () => {
                reg[SymRef.A] = reg[SymRef.C];
            };

            // MOV A, D
            opcode[0x7a] = () => {
                reg[SymRef.A] = reg[SymRef.D];
            };

            // MOV A, E
            opcode[0x7b] = () => {
                reg[SymRef.A] = reg[SymRef.E];
            };

            // MOV A, H
            opcode[0x7c] = () => {
                reg[SymRef.A] = reg[SymRef.H];
            };

            // MOV A, L
            opcode[0x7d] = () => {
                reg[SymRef.A] = reg[SymRef.L];
            };

            // MOV A, M
            opcode[0x7e] = () => {
                reg[SymRef.A] = (Word)reg[SymRefD.HL];
            };

            // MOV A, A
            opcode[0x7f] = () => {
                reg[SymRef.A] = reg[SymRef.A];
            };

            // ADD B
            opcode[0x80] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.B]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD C
            opcode[0x81] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.C]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD D
            opcode[0x82] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.D]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD E
            opcode[0x83] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.E]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD H
            opcode[0x84] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.H]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD L
            opcode[0x85] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.L]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD M
            opcode[0x86] = () => {
                acc = alu.add(reg[SymRef.A], (Word)reg[SymRefD.HL]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD A
            opcode[0x87] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.A]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC B
            opcode[0x88] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], reg[SymRef.B], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC C
            opcode[0x89] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], reg[SymRef.C], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC D
            opcode[0x8a] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], reg[SymRef.D], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC E
            opcode[0x8b] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], reg[SymRef.E], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC H
            opcode[0x8c] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], reg[SymRef.H], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC L
            opcode[0x8d] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], reg[SymRef.L], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC M
            opcode[0x8e] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], (Word)reg[SymRefD.HL], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC A
            opcode[0x8f] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.add(reg[SymRef.A], reg[SymRef.A], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB B
            opcode[0x90] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB C
            opcode[0x91] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB D
            opcode[0x92] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB E
            opcode[0x93] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB H
            opcode[0x94] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB L
            opcode[0x95] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB M
            opcode[0x96] = () => {
                acc = alu.sub(reg[SymRef.A], (Word)reg[SymRefD.HL]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB A
            opcode[0x97] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB B
            opcode[0x98] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB C
            opcode[0x99] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB D
            opcode[0x9a] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB E
            opcode[0x9b] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB H
            opcode[0x9c] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB L
            opcode[0x9d] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB M
            opcode[0x9e] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], (Word)reg[SymRefD.HL], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB A
            opcode[0x9f] = () => {
                Word carry = flags.get(Flag.C) ? (Byte)0 : (Byte)1;
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA B
            opcode[0xa0] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.B]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA C
            opcode[0xa1] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.C]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA D
            opcode[0xa2] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.D]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA E
            opcode[0xa3] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.E]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA H
            opcode[0xa4] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.H]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA L
            opcode[0xa5] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.L]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA M
            opcode[0xa6] = () => {
                acc = alu.and(reg[SymRef.A], (Word)reg[SymRefD.HL]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA A
            opcode[0xa7] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.A]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA B
            opcode[0xa8] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.B]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA C
            opcode[0xa9] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.C]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA D
            opcode[0xaa] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.D]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA E
            opcode[0xab] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.E]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA H
            opcode[0xac] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.H]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA L
            opcode[0xad] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.L]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA M
            opcode[0xae] = () => {
                acc = alu.xor(reg[SymRef.A], (Word)reg[SymRefD.HL]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA A
            opcode[0xaf] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.A]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA B
            opcode[0xb0] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.B]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA C
            opcode[0xb1] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.C]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA D
            opcode[0xb2] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.D]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA E
            opcode[0xb3] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.E]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA H
            opcode[0xb4] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.H]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA L
            opcode[0xb5] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.L]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA M
            opcode[0xb6] = () => {
                acc = alu.or(reg[SymRef.A], (Word)reg[SymRefD.HL]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA A
            opcode[0xb7] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.A]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // CMP B
            opcode[0xb8] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP C
            opcode[0xb9] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP D
            opcode[0xba] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP E
            opcode[0xbb] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP H
            opcode[0xbc] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP L
            opcode[0xbd] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP M
            opcode[0xbe] = () => {
                acc = alu.sub(reg[SymRef.A], (Word)reg[SymRefD.HL]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP A
            opcode[0xbf] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // RNZ
            opcode[0xc0] = () => {
                if (!flags.get(Flag.Z)) {
                    opcode[0xc9].Invoke();
                }
            };

            // POP B
            opcode[0xc1] = () => {
                reg[SymRef.C] = mem[reg.Sp--];
                reg[SymRef.B] = mem[reg.Sp--];
            };

            // JNZ adr
            opcode[0xc2] = () => {
                if (!flags.get(Flag.Z)) {
                    DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                    reg.Pc = adr;
                }
            };

            // JMP adr
            opcode[0xc3] = () => {
                DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                reg.Pc = adr;
            };

            // CNZ adr
            opcode[0xc4] = () => {
                if (!flags.get(Flag.Z)) {
                    opcode[0xcd].Invoke();
                } else {
                    reg.Pc += 2;
                }
            };

            // PUSH B
            opcode[0xc5] = () => {
                mem[++reg.Sp] = reg[SymRef.B];
                mem[++reg.Sp] = reg[SymRef.C];
            };

            // ADI d8
            opcode[0xc6] = () => {
                acc = alu.add(reg[SymRef.A], mem[reg.Pc++]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 0
            opcode[0xc7] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x00;
            };

            // RZ
            opcode[0xc8] = () => {
                if (flags.get(Flag.Z)) {
                    opcode[0xc9].Invoke(); // ret
                }
            };

            // RET
            opcode[0xc9] = () => {
                DWord adr = (DWord)(mem[reg.Sp--] | mem[reg.Sp--] << 8);
                reg.Pc = adr;
            };

            // JZ
            opcode[0xca] = () => {
                if (flags.get(Flag.Z)) {
                    DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                    reg.Pc = adr;
                }
            };

            // NOP
            opcode[0xcb] = nop;

            // CZ adr
            opcode[0xcc] = () => {
                if (flags.get(Flag.Z)) {
                    opcode[0xcd].Invoke(); // call
                }
            };

            // CALL adr
            opcode[0xcd] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                reg.Pc = adr;
            };

            // ACI d8
            opcode[0xce] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], mem[reg.Pc++], carry);
            };

            // RST 1
            opcode[0xcf] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x08;
            };

            // RNC
            opcode[0xd0] = () => {
                if (!flags.get(Flag.C)) {
                    opcode[0xc9].Invoke(); // ret
                }
            };

            // POP B
            opcode[0xd1] = () => {
                reg[SymRef.E] = mem[reg.Sp--];
                reg[SymRef.D] = mem[reg.Sp--];
            };

            // JNC adr
            opcode[0xd2] = () => {
                if (!flags.get(Flag.C)) {
                    DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                    reg.Pc = adr;
                }
            };

            // OUT d8
            opcode[0xd3] = () => {
                reg.Pc++;
                throw new NotImplementedException("OUT d8, 0xd3");
            };

            // CNC adr
            opcode[0xd4] = () => {
                if (!flags.get(Flag.C)) {
                    opcode[0xcd].Invoke(); // call
                }
            };

            // PUSH D
            opcode[0xd5] = () => {
                mem[++reg.Sp] = reg[SymRef.D];
                mem[++reg.Sp] = reg[SymRef.E];
            };

            // SUI d8
            opcode[0xd6] = () => {
                acc = alu.sub(reg[SymRef.A], mem[reg.Pc++]);
            };

            // RST 2
            opcode[0xd7] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x10;
            };

            // RC
            opcode[0xd8] = () => {
                if (flags.get(Flag.C)) {
                    opcode[0xc9].Invoke(); // ret
                }
            };

            // NOP
            opcode[0xd9] = nop;

            // JC adr
            opcode[0xda] = () => {
                if (flags.get(Flag.C)) {
                    DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                    reg.Pc = adr;
                }
            };

            // IN d8
            opcode[0xdb] = () => {
                reg.Pc++;
                throw new NotImplementedException("IN d8, 0xdb");
            };

            // CC adr
            opcode[0xdc] = () => {
                if (flags.get(Flag.C)) {
                    opcode[0xcd].Invoke(); // call
                }
            };

            // NOP
            opcode[0xdd] = nop;

            // SBI d8
            opcode[0xde] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], mem[reg.Pc++], carry);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 3
            opcode[0xdf] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x18;
            };

            // RPO
            opcode[0xe0] = () => {
                if (!flags.get(Flag.P)) {
                    opcode[0xc9].Invoke(); // ret
                }
            };

            // POP H
            opcode[0xe1] = () => {
                reg[SymRef.L] = mem[reg.Sp--];
                reg[SymRef.H] = mem[reg.Sp--];
            };

            // JPO adr
            opcode[0xe2] = () => {
                if (!flags.get(Flag.P)) {
                    DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                    reg.Pc = adr;
                }
            };

            // XTHL
            opcode[0xe3] = () => {
                Word sp1, sp2;
                sp1 = mem[reg.Sp--];
                sp2 = mem[reg.Sp--];
                mem[++reg.Sp] = reg[SymRef.L];
                mem[++reg.Sp] = reg[SymRef.H];
                reg[SymRef.L] = sp1;
                reg[SymRef.H] = sp2;
            };

            // CPO adr
            opcode[0xe4] = () => {
                if (!flags.get(Flag.P)) {
                    opcode[0xcd].Invoke(); // call adr
                }
            };

            // PUSH H
            opcode[0xe5] = () => {
                mem[++reg.Sp] = reg[SymRef.H];
                mem[++reg.Sp] = reg[SymRef.L];
            };

            // ANI d8
            opcode[0xe6] = () => {
                acc = alu.and(reg[SymRef.A], mem[reg.Pc++]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 4
            opcode[0xe7] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x20;
            };

            // RPE
           opcode[0xe8] = () => {
               if (flags.get(Flag.P)) {
                   opcode[0xc9].Invoke(); // ret
               }
           };

            // PCHL
            opcode[0xe9] = () => {
                DWord adr = (DWord)(reg[SymRef.L] | reg[SymRef.H] << 8);
                reg.Pc = adr;
            };

            // JPE adr
            opcode[0xea] = () => {
                if (flags.get(Flag.P)) {
                    DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                    reg.Pc = adr;
                }
            };

            // XDHG
            opcode[0xeb] = () => {
                Word e, d;
                e = reg[SymRef.E];
                d = reg[SymRef.D];
                reg[SymRef.E] = reg[SymRef.L];
                reg[SymRef.D] = reg[SymRef.H];
                reg[SymRef.L] = e;
                reg[SymRef.H] = d;
            };

            // CPE adr
            opcode[0xec] = () => {
                if (flags.get(Flag.P)) {
                    opcode[0xcd].Invoke(); // call adr
                } else {
                    reg.Pc += 2;
                }
            };

            // NOP
            opcode[0xed] = nop;

            // XRI d8
            opcode[0xee] = () => {
                acc = alu.xor(reg[SymRef.A], mem[reg.Pc++]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 5
            opcode[0xef] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x28;
            };

            // RP
            opcode[0xf0] = () => {
                if (!flags.get(Flag.S)) {
                    opcode[0xc9].Invoke(); // ret
                }
            };

            // POP PSW
            opcode[0xf1] = () => {
                flags.put(mem[reg.Sp--]);
                reg[SymRef.A] = mem[reg.Sp--];
                reg.Sp = reg.Sp;
            };

            // JP adr
            opcode[0xf2] = () => {
                DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                if (!flags.get(Flag.S)) {
                    reg.Pc = adr;
                }
            };

            // DI
            opcode[0xf3] = () => {
                throw new NotImplementedException("DI, 0xf3");
            };

            // CP adr
            opcode[0xf4] = () => {
                if (!flags.get(Flag.S)) {
                    opcode[0xcd].Invoke(); // call adr
                } else {
                    reg.Pc += 2;
                }
            };

            // PUSH PSW
            opcode[0xf5] = () => {
                mem[++reg.Sp] = flags.get();
                mem[++reg.Sp] = reg[SymRef.A];
            };

            // ORI d8
            opcode[0xf6] = () => {
                acc = alu.or(reg[SymRef.A], mem[reg.Pc++]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 5
            opcode[0xf7] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x30;
            };

            // RM
            opcode[0xf8] = () => {
                if (flags.get(Flag.S)) {
                    opcode[0xc9].Invoke(); // ret
                }
            };

            //SPHL
            opcode[0xf9] = () => {
                reg.Sp = reg[SymRefD.HL];
            };

            // JM adr
            opcode[0xfa] = () => {
                DWord adr = (DWord)(mem[reg.Pc++] | mem[reg.Pc++] << 8);
                if (flags.get(Flag.S)) {
                    reg.Pc = adr;
                }
            };

            // EI
            opcode[0xfb] = () => {
                throw new NotImplementedException("EI Enable Interrupts, 0xfb");
            };

            // CM adr
            opcode[0xfc] = () => {
                if (flags.get(Flag.S)) {
                    opcode[0xcd].Invoke(); // call adr
                } else {
                    reg.Pc += 2;
                }
            };

            // NOP
            opcode[0xfd] = nop;


            // CPI d8
            opcode[0xfe] = () => {
                acc = alu.sub(reg[SymRef.A], mem[reg.Pc++]);
                setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 7
            opcode[0xef] = () => {
                mem[++reg.Sp] = (Word)reg.Pc;
                mem[++reg.Sp] = (Word)(reg.Pc >> 8);
                reg.Pc = 0x38;
            };
        }
    }
}
