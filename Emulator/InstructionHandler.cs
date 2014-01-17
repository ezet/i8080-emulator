using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;
using Flag = eZet.i8080.Emulator.StatusFlag;

namespace eZet.i8080.Emulator {
    public class InstructionHandler {

        public delegate Word Instruction();

        private Cpu cpu;

        private Register reg;

        private StatusRegister flags;

        private Alu alu;

        public Word acc { get; set; }

        private Instruction[] opcodes = new Instruction[256];

        public InstructionHandler(Cpu cpu) {
            this.cpu = cpu;
            reg = cpu.Reg;
            flags = cpu.Flags;
            alu = cpu.Alu;
            initOpcodeMap();
        }

        public Word executeInstruction(Word opcode) {
            try {
                return opcodes[opcode].Invoke();
            } catch (NullReferenceException e) {
                throw new NotImplementedException(string.Format("Unknown opcode: 0x{0:X2}", opcode), e);
            }

        }


        private void initOpcodeMap() {

            // NOP
            opcodes[0x00] = cpu.nop;

            // LXI B, d16
            opcodes[0x01] = () => {
                reg[SymRef.C] = cpu.loadPc();
                reg[SymRef.B] = cpu.loadPc();
                return 10;
            };

            // STAX B
            opcodes[0x02] = () => {
                cpu.store(reg[SymRefD.BC], reg[SymRef.A]);
                return 7;
            };

            // INX B
            opcodes[0x03] = () => {
                reg[SymRefD.BC] = alu.add(reg[SymRefD.BC], 1);
                return 5;
            };

            // INR B
            opcodes[0x04] = () => {
                acc = reg[SymRef.B];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.B] = acc;
                return 5;
            };

            // DCR B
            opcodes[0x05] = () => {
                acc = reg[SymRef.B];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.B] = acc;
                return 5;
            };

            // MVI B, d8
            opcodes[0x06] = () => {
                reg[SymRef.B] = cpu.loadPc();
                return 7;
            };

            // RLC
            opcodes[0x07] = () => {
                acc = reg[SymRef.A];
                acc = alu.rotateLeft(acc, 1);
                flags.put(acc, Flag.C);
                reg[SymRef.A] = acc;
                return 4;
            };

            // NOP
            opcodes[0x08] = cpu.nop;

            // DAD B
            opcodes[0x09] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.BC]);
                cpu.setFlags(Flag.C);
                return 10;
            };

            // LDAX B
            opcodes[0x0a] = () => {
                reg[SymRef.A] = cpu.load(reg[SymRefD.BC]);
                return 7;
            };

            // DCX B
            opcodes[0x0b] = () => {
                reg[SymRefD.BC] = alu.sub(reg[SymRefD.BC], 1);
                return 5;
            };

            // INR C
            opcodes[0x0c] = () => {
                acc = reg[SymRef.C];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.C] = acc;
                return 5;
            };

            // DCR C
            opcodes[0x0d] = () => {
                acc = reg[SymRef.C];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.C] = acc;
                return 5;
            };

            // MVI C, d8
            opcodes[0x0e] = () => {
                reg[SymRef.C] = cpu.loadPc();
                return 7;
            };

            // RRC
            opcodes[0x0f] = () => {
                acc = reg[SymRef.A];
                flags.put(acc, Flag.C);
                acc = alu.rotateRight(acc, 1);
                reg[SymRef.A] = acc;
                return 4;
            };

            // NOP
            opcodes[0x10] = cpu.nop;

            // LXI D, d16
            opcodes[0x11] = () => {
                reg[SymRef.E] = cpu.loadPc();
                reg[SymRef.D] = cpu.loadPc();
                return 10;
            };

            // STAX D
            opcodes[0x12] = () => {
                cpu.store(reg[SymRefD.DE], reg[SymRef.A]);
                return 7;
            };

            //INX D
            opcodes[0x13] = () => {
                reg[SymRefD.DE] = alu.add(reg[SymRefD.DE], 1);
                return 5;
            };

            // INR D
            opcodes[0x14] = () => {
                acc = reg[SymRef.D];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.D] = acc;
                return 5;
            };

            // DCR D
            opcodes[0x15] = () => {
                acc = reg[SymRef.D];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.D] = acc;
                return 5;
            };

            // MVI D, d8
            opcodes[0x16] = () => {
                reg[SymRef.D] = cpu.loadPc();
                return 7;
            };

            // RAL
            opcodes[0x17] = () => {
                acc = reg[SymRef.A];
                acc = alu.rotateCarryLeft(acc, 1, flags.get(Flag.C));
                cpu.setFlags(Flag.C);
                reg[SymRef.A] = acc;
                return 4;
            };

            // NOP
            opcodes[0x18] = cpu.nop;

            // DAD D
            opcodes[0x19] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.DE]);
                cpu.setFlags(Flag.C);
                return 10;
            };

            // LDAX D
            opcodes[0x1a] = () => {
                reg[SymRef.A] = cpu.load(reg[SymRefD.DE]);
                return 7;
            };

            // DCX D
            opcodes[0x1b] = () => {
                reg[SymRefD.DE] = alu.sub(reg[SymRefD.DE], 1);
                return 5;
            };

            // INR E
            opcodes[0x1c] = () => {
                acc = reg[SymRef.E];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.E] = acc;
                return 5;
            };

            // DCR E
            opcodes[0x1d] = () => {
                acc = reg[SymRef.E];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.E] = acc;
                return 5;
            };

            // MVI E, d8
            opcodes[0x1e] = () => {
                reg[SymRef.E] = cpu.loadPc();
                return 7;
            };

            // RAR
            opcodes[0x1f] = () => {
                acc = reg[SymRef.A];
                flags.put(acc, Flag.C);
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.rotateCarryRight(acc, 1, carry);
                reg[SymRef.A] = acc;
                return 4;
            };

            // RIM, 8085 only
            opcodes[0x20] = cpu.nop;

            // LXI H, d16
            opcodes[0x21] = () => {
                reg[SymRef.L] = cpu.loadPc();
                reg[SymRef.H] = cpu.loadPc();
                return 10;
            };

            // SHLD adr
            opcodes[0x22] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.store(adr, reg[SymRef.L]);
                cpu.store(++adr,reg[SymRef.H]);
                return 16;
            };

            // INX H
            opcodes[0x23] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], 1);
                return 5;
            };

            // INR H
            opcodes[0x24] = () => {
                acc = reg[SymRef.H];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.H] = acc;
                return 5;
            };

            // DCR H
            opcodes[0x25] = () => {
                acc = reg[SymRef.H];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.H] = acc;
                return 5;
            };

            // MVI H, d8
            opcodes[0x26] = () => {
                reg[SymRef.H] = cpu.loadPc();
                return 7;
            };

            // DAA
            opcodes[0x27] = () => {
                acc = reg[SymRef.A];
                acc &= 0x0f;
                if (acc > 9 || flags.get(Flag.A)) {
                    reg[SymRef.A] = alu.add(reg[SymRef.A], 6);
                }
                cpu.setFlags(Flag.A);
                acc = reg[SymRef.A];
                acc = (Word)(acc >> 4);
                if (acc > 9 || flags.get(Flag.C)) {
                    acc = alu.add(reg[SymRef.A], 96);
                    reg[SymRef.A] = acc;
                }
                cpu.setFlags(Flag.C, Flag.Z, Flag.S, Flag.P);
                return 4;
            };

            // NOP
            opcodes[0x28] = cpu.nop;

            // DAD H
            opcodes[0x29] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.HL]);
                cpu.setFlags(Flag.C);
                return 10;
            };

            // LHLD adr
            opcodes[0x2a] = () => {
                DWord adr = cpu.loadPcAddress();
                reg[SymRef.L] = cpu.load(adr);
                reg[SymRef.H] = cpu.load(++adr);
                return 16;
            };

            // DCX H
            opcodes[0x2b] = () => {
                reg[SymRefD.HL] = alu.sub(reg[SymRefD.HL], 1);
                return 5;
            };

            // INR L
            opcodes[0x2c] = () => {
                acc = reg[SymRef.L];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.L] = acc;
                return 5;
            };

            // DCR L
            opcodes[0x2d] = () => {
                acc = reg[SymRef.L];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.L] = acc;
                return 5;
            };

            // MVI L, d8
            opcodes[0x2e] = () => {
                reg[SymRef.L] = cpu.loadPc();
                return 7;
            };

            // CMA
            opcodes[0x2f] = () => {
                reg[SymRef.A] = (byte)~reg[SymRef.A];
                return 4;
            };

            // SIM, i8085
            opcodes[0x30] = cpu.nop;

            // LXI SP, d16
            opcodes[0x31] = () => {
                DWord adr = cpu.loadPcAddress();
                reg.Sp = adr;
                return 10;
            };

            // STA adr
            opcodes[0x32] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.store(adr, reg[SymRef.A]);
                return 13;
            };

            // INX SP
            opcodes[0x33] = () => {
                reg.Sp = alu.add(reg.Sp, 1);
                return 5;
            };

            // INR M
            opcodes[0x34] = () => {
                acc = alu.add(cpu.load(reg[SymRefD.M]), 1);
                cpu.store(reg[SymRefD.M], acc);
                cpu.setFlags(cpu.load(reg[SymRefD.M]), Flag.Z, Flag.S, Flag.P, Flag.A);
                // TODO setflags DWord
                return 10;
            };

            // DCR M
            opcodes[0x35] = () => {
                acc = alu.sub(cpu.load(reg[SymRefD.M]), 1);
                cpu.store(reg[SymRefD.M], acc);
                cpu.setFlags(cpu.load(reg[SymRefD.M]), Flag.Z, Flag.S, Flag.P, Flag.A);
                // TODO setflags DWord
                return 10;
            };


            // MVI M, d8
            opcodes[0x36] = () => {
                cpu.store(reg[SymRefD.M], cpu.loadPc());
                return 10;
            };

            // STC
            opcodes[0x37] = () => {
                flags.set(Flag.C);
                return 4;
            };

            // NOP
            opcodes[0x38] = cpu.nop;

            // DAD SP
            opcodes[0x39] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg.Sp);
                return 10;
            };

            // LDA adr
            opcodes[0x3a] = () => {
                DWord adr = cpu.loadPcAddress();
                reg[SymRef.A] = cpu.load(adr);
                return 16;
            };

            // DCX SP
            opcodes[0x3b] = () => {
                reg.Sp = alu.sub(reg.Sp, 1);
                return 5;
            };

            // INR A
            opcodes[0x3c] = () => {
                acc = reg[SymRef.A];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.A] = acc;
                return 5;
            };

            // DCR A
            opcodes[0x3d] = () => {
                acc = reg[SymRef.A];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.A] = acc;
                return 5;
            };

            // MVI A, d8
            opcodes[0x3e] = () => {
                reg[SymRef.A] = cpu.loadPc();
                return 7;
            };

            // CMC
            opcodes[0x3f] = () => {
                flags.toggle(Flag.C);
                return 4;
            };

            // MOV B, B
            opcodes[0x40] = () => {
                reg[SymRef.B] = reg[SymRef.B];
                return 5;
            };

            // MOV B, C
            opcodes[0x41] = () => {
                reg[SymRef.B] = reg[SymRef.C];
                return 5;
            };

            // MOV B, D
            opcodes[0x42] = () => {
                reg[SymRef.B] = reg[SymRef.D];
                return 5;
            };

            // MOV B, E
            opcodes[0x43] = () => {
                reg[SymRef.B] = reg[SymRef.E];
                return 5;
            };

            // MOV B, H
            opcodes[0x44] = () => {
                reg[SymRef.B] = reg[SymRef.H];
                return 5;
            };

            // MOV B, L
            opcodes[0x45] = () => {
                reg[SymRef.B] = reg[SymRef.L];
                return 5;
            };

            // MOV B, M
            opcodes[0x46] = () => {
                reg[SymRef.B] = cpu.load(reg[SymRefD.M]);
                return 7;
            };

            // MOV B, A
            opcodes[0x47] = () => {
                reg[SymRef.B] = reg[SymRef.A];
                return 5;
            };

            // MOV C, B
            opcodes[0x48] = () => {
                reg[SymRef.C] = reg[SymRef.B];
                return 5;
            };

            // MOV C, C
            opcodes[0x49] = () => {
                reg[SymRef.C] = reg[SymRef.C];
                return 5;
            };

            // MOV C, D
            opcodes[0x4a] = () => {
                reg[SymRef.C] = reg[SymRef.D];
                return 5;
            };

            // MOV C, E
            opcodes[0x4b] = () => {
                reg[SymRef.C] = reg[SymRef.E];
                return 5;
            };

            // MOV C, H
            opcodes[0x4c] = () => {
                reg[SymRef.C] = reg[SymRef.H];
                return 5;
            };

            // MOV C, L
            opcodes[0x4d] = () => {
                reg[SymRef.C] = reg[SymRef.L];
                return 5;
            };

            // MOV C, M
            opcodes[0x4e] = () => {
                reg[SymRef.C] = cpu.load(reg[SymRefD.M]);
                return 7;
            };

            // MOV C, A
            opcodes[0x4f] = () => {
                reg[SymRef.C] = reg[SymRef.A];
                return 5;
            };

            // MOV D, B
            opcodes[0x50] = () => {
                reg[SymRef.D] = reg[SymRef.B];
                return 5;
            };

            // MOV D, C
            opcodes[0x51] = () => {
                reg[SymRef.D] = reg[SymRef.C];
                return 5;
            };

            // MOV D, D
            opcodes[0x52] = () => {
                reg[SymRef.D] = reg[SymRef.D];
                return 5;
            };

            // MOV D, E
            opcodes[0x53] = () => {
                reg[SymRef.D] = reg[SymRef.E];
                return 5;
            };

            // MOV D, H
            opcodes[0x54] = () => {
                reg[SymRef.D] = reg[SymRef.H];
                return 5;
            };

            // MOV D, L
            opcodes[0x55] = () => {
                reg[SymRef.D] = reg[SymRef.L];
                return 5;
            };

            // MOV D, M
            opcodes[0x56] = () => {
                reg[SymRef.D] = cpu.load(reg[SymRefD.M]);
                return 7;
            };

            // MOV D, A
            opcodes[0x57] = () => {
                reg[SymRef.D] = reg[SymRef.A];
                return 5;
            };

            // MOV E, B
            opcodes[0x58] = () => {
                reg[SymRef.E] = reg[SymRef.B];
                return 5;
            };

            // MOV E, C
            opcodes[0x59] = () => {
                reg[SymRef.E] = reg[SymRef.C];
                return 5;
            };

            // MOV E, D
            opcodes[0x5a] = () => {
                reg[SymRef.E] = reg[SymRef.D];
                return 5;
            };

            // MOV E, E
            opcodes[0x5b] = () => {
                reg[SymRef.E] = reg[SymRef.E];
                return 5;
            };

            // MOV E, H
            opcodes[0x5c] = () => {
                reg[SymRef.E] = reg[SymRef.H];
                return 5;
            };

            // MOV E, L
            opcodes[0x5d] = () => {
                reg[SymRef.E] = reg[SymRef.L];
                return 5;
            };

            // MOV E, M
            opcodes[0x5e] = () => {
                reg[SymRef.E] = cpu.load(reg[SymRefD.M]);
                return 7;
            };

            // MOV E, A
            opcodes[0x5f] = () => {
                reg[SymRef.E] = reg[SymRef.A];
                return 5;
            };

            // MOV H, B
            opcodes[0x60] = () => {
                reg[SymRef.H] = reg[SymRef.B];
                return 5;
            };

            // MOV H, C
            opcodes[0x61] = () => {
                reg[SymRef.H] = reg[SymRef.C];
                return 5;
            };

            // MOV H, D
            opcodes[0x62] = () => {
                reg[SymRef.H] = reg[SymRef.D];
                return 5;
            };

            // MOV H, E
            opcodes[0x63] = () => {
                reg[SymRef.H] = reg[SymRef.E];
                return 5;
            };

            // MOV H, H
            opcodes[0x64] = () => {
                reg[SymRef.H] = reg[SymRef.H];
                return 5;
            };

            // MOV H, L
            opcodes[0x65] = () => {
                reg[SymRef.H] = reg[SymRef.L];
                return 5;
            };

            // MOV H, M
            opcodes[0x66] = () => {
                reg[SymRef.H] = cpu.load(reg[SymRefD.M]);
                return 7;
            };

            // MOV H, A
            opcodes[0x67] = () => {
                reg[SymRef.H] = reg[SymRef.A];
                return 5;
            };

            // MOV L, B
            opcodes[0x68] = () => {
                reg[SymRef.L] = reg[SymRef.B];
                return 5;
            };

            // MOV L, C
            opcodes[0x69] = () => {
                reg[SymRef.L] = reg[SymRef.C];
                return 5;
            };

            // MOV L, D
            opcodes[0x6a] = () => {
                reg[SymRef.L] = reg[SymRef.D];
                return 5;
            };

            // MOV L, E
            opcodes[0x6b] = () => {
                reg[SymRef.L] = reg[SymRef.E];
                return 5;
            };

            // MOV L, H
            opcodes[0x6c] = () => {
                reg[SymRef.L] = reg[SymRef.H];
                return 5;
            };

            // MOV L, L
            opcodes[0x6d] = () => {
                reg[SymRef.L] = reg[SymRef.L];
                return 5;
            };

            // MOV L, M
            opcodes[0x6e] = () => {
                reg[SymRef.L] = cpu.load(reg[SymRefD.M]);
                return 7;
            };

            // MOV L, A
            opcodes[0x6f] = () => {
                reg[SymRef.L] = reg[SymRef.A];
                return 5;
            };

            // MOV M, B
            opcodes[0x70] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.B]);
                return 7;
            };

            // MOV M, C
            opcodes[0x71] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.C]);
                return 7;

            };

            // MOV M, D
            opcodes[0x72] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.D]);
                return 7;

            };

            // MOV M, E
            opcodes[0x73] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.E]);
                return 7;

            };

            // MOV M, H
            opcodes[0x74] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.H]);
                return 7;

            };

            // MOV M, L
            opcodes[0x75] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.L]);
                return 7;

            };

            // HLT
            opcodes[0x76] = () => {
                cpu.Halt = true;
                return 7;

            };

            // MOV M, A
            opcodes[0x77] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.A]);
                return 5;
            };

            // MOV A, B
            opcodes[0x78] = () => {
                reg[SymRef.A] = reg[SymRef.B];
                return 5;
            };

            // MOV A, C
            opcodes[0x79] = () => {
                reg[SymRef.A] = reg[SymRef.C];
                return 5;
            };

            // MOV A, D
            opcodes[0x7a] = () => {
                reg[SymRef.A] = reg[SymRef.D];
                return 5;
            };

            // MOV A, E
            opcodes[0x7b] = () => {
                reg[SymRef.A] = reg[SymRef.E];
                return 5;
            };

            // MOV A, H
            opcodes[0x7c] = () => {
                reg[SymRef.A] = reg[SymRef.H];
                return 5;
            };

            // MOV A, L
            opcodes[0x7d] = () => {
                reg[SymRef.A] = reg[SymRef.L];
                return 5;
            };

            // MOV A, M
            opcodes[0x7e] = () => {
                reg[SymRef.A] = cpu.load(reg[SymRefD.M]);
                return 7;
            };

            // MOV A, A
            opcodes[0x7f] = () => {
                reg[SymRef.A] = reg[SymRef.A];
                return 5;
            };

            // ADD B
            opcodes[0x80] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADD C
            opcodes[0x81] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADD D
            opcodes[0x82] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADD E
            opcodes[0x83] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADD H
            opcodes[0x84] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADD L
            opcodes[0x85] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADD M
            opcodes[0x86] = () => {
                acc = alu.add(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // ADD A
            opcodes[0x87] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADC B
            opcodes[0x88] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.B], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADC C
            opcodes[0x89] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.C], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADC D
            opcodes[0x8a] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.D], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADC E
            opcodes[0x8b] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.E], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADC H
            opcodes[0x8c] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.H], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADC L
            opcodes[0x8d] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.L], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ADC M
            opcodes[0x8e] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], cpu.load(reg[SymRefD.M]), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // ADC A
            opcodes[0x8f] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.A], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SUB B
            opcodes[0x90] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SUB C
            opcodes[0x91] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SUB D
            opcodes[0x92] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SUB E
            opcodes[0x93] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SUB H
            opcodes[0x94] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SUB L
            opcodes[0x95] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SUB M
            opcodes[0x96] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // SUB A
            opcodes[0x97] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SBB B
            opcodes[0x98] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SBB C
            opcodes[0x99] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SBB D
            opcodes[0x9a] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SBB E
            opcodes[0x9b] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SBB H
            opcodes[0x9c] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SBB L
            opcodes[0x9d] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // SBB M
            opcodes[0x9e] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], cpu.load(reg[SymRefD.M]), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // SBB A
            opcodes[0x9f] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ANA B
            opcodes[0xa0] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ANA C
            opcodes[0xa1] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ANA D
            opcodes[0xa2] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ANA E
            opcodes[0xa3] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ANA H
            opcodes[0xa4] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ANA L
            opcodes[0xa5] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ANA M
            opcodes[0xa6] = () => {
                acc = alu.and(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // ANA A
            opcodes[0xa7] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // XRA B
            opcodes[0xa8] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // XRA C
            opcodes[0xa9] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // XRA D
            opcodes[0xaa] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // XRA E
            opcodes[0xab] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // XRA H
            opcodes[0xac] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // XRA L
            opcodes[0xad] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // XRA M
            opcodes[0xae] = () => {
                acc = alu.xor(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // XRA A
            opcodes[0xaf] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ORA B
            opcodes[0xb0] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ORA C
            opcodes[0xb1] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ORA D
            opcodes[0xb2] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ORA E
            opcodes[0xb3] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ORA H
            opcodes[0xb4] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ORA L
            opcodes[0xb5] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // ORA M
            opcodes[0xb6] = () => {
                acc = alu.or(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // ORA A
            opcodes[0xb7] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 4;
            };

            // CMP B
            opcodes[0xb8] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 4;
            };

            // CMP C
            opcodes[0xb9] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 4;
            };

            // CMP D
            opcodes[0xba] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 4;
            };

            // CMP E
            opcodes[0xbb] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 4;
            };

            // CMP H
            opcodes[0xbc] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 4;
            };

            // CMP L
            opcodes[0xbd] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 4;
            };

            // CMP M
            opcodes[0xbe] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 7;
            };

            // CMP A
            opcodes[0xbf] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 4;
            };

            // RNZ
            opcodes[0xc0] = () => {
                if (!flags.get(Flag.Z)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // POP B
            opcodes[0xc1] = () => {
                reg[SymRef.C] = cpu.pop();
                reg[SymRef.B] = cpu.pop();
                return 10;
            };

            // JNZ adr
            opcodes[0xc2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.Z)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // JMP adr
            opcodes[0xc3] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.jmp(adr);
                return 10;
            };

            // CNZ adr
            opcodes[0xc4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.Z)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // PUSH B
            opcodes[0xc5] = () => {
                cpu.push(reg[SymRef.B]);
                cpu.push(reg[SymRef.C]);
                return 11;
            };

            // ADI d8
            opcodes[0xc6] = () => {
                acc = alu.add(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // RST 0
            opcodes[0xc7] = () => {
                cpu.call(0x00);
                return 11;
            };

            // RZ
            opcodes[0xc8] = () => {
                if (flags.get(Flag.Z)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // RET
            opcodes[0xc9] = () => {
                cpu.ret();
                return 10;
            };

            // JZ
            opcodes[0xca] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.Z)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // NOP
            opcodes[0xcb] = cpu.nop;

            // CZ adr
            opcodes[0xcc] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.Z)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // CALL adr
            opcodes[0xcd] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.call(adr);
                return 17;
            };

            // ACI d8
            opcodes[0xce] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], cpu.loadPc(), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // RST 1
            opcodes[0xcf] = () => {
                cpu.call(0x08);
                return 11;
            };

            // RNC
            opcodes[0xd0] = () => {
                if (!flags.get(Flag.C)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // POP D
            opcodes[0xd1] = () => {
                reg[SymRef.E] = cpu.pop();
                reg[SymRef.D] = cpu.pop();
                return 10;
            };

            // JNC adr
            opcodes[0xd2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.C)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // OUT d8
            opcodes[0xd3] = () => {
                Word port = cpu.loadPc();
                cpu.deviceWrite(port, reg[SymRef.A]);
                return 8;
            };

            // CNC adr
            opcodes[0xd4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.C)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // PUSH D
            opcodes[0xd5] = () => {
                cpu.push(reg[SymRef.D]);
                cpu.push(reg[SymRef.E]);
                return 11;
            };

            // SUI d8
            opcodes[0xd6] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // RST 2
            opcodes[0xd7] = () => {
                cpu.call(0x10);
                return 11;
            };

            // RC
            opcodes[0xd8] = () => {
                if (flags.get(Flag.C)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // NOP
            opcodes[0xd9] = cpu.nop;

            // JC adr
            opcodes[0xda] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.C)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // IN d8
            opcodes[0xdb] = () => {
                Word port = cpu.loadPc();
                reg[SymRef.A] = cpu.DeviceRead(port);
                return 10;
            };

            // CC adr
            opcodes[0xdc] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.C)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // NOP
            opcodes[0xdd] = cpu.nop;

            // SBI d8
            opcodes[0xde] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], cpu.loadPc(), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // RST 3
            opcodes[0xdf] = () => {
                cpu.call(0x18);
                return 11;
            };

            // RPO
            opcodes[0xe0] = () => {
                if (!flags.get(Flag.P)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // POP H
            opcodes[0xe1] = () => {
                reg[SymRef.L] = cpu.pop();
                reg[SymRef.H] = cpu.pop();
                return 10;
            };

            // JPO adr
            opcodes[0xe2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.P)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // XTHL
            opcodes[0xe3] = () => {
                Word hi, lo;
                lo = cpu.pop();
                hi = cpu.pop();
                cpu.push(reg[SymRef.H]);
                cpu.push(reg[SymRef.L]);
                reg[SymRef.L] = lo;
                reg[SymRef.H] = hi;
                return 18;
            };

            // CPO adr
            opcodes[0xe4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.P)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // PUSH H
            opcodes[0xe5] = () => {
                cpu.push(reg[SymRef.H]);
                cpu.push(reg[SymRef.L]);
                return 11;
            };

            // ANI d8
            opcodes[0xe6] = () => {
                acc = alu.and(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // RST 4
            opcodes[0xe7] = () => {
                cpu.call(0x20);
                return 11;
            };

            // RPE
            opcodes[0xe8] = () => {
                if (flags.get(Flag.P)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // PCHL
            opcodes[0xe9] = () => {
                DWord adr = (DWord)(reg[SymRef.L] | reg[SymRef.H] << 8);
                cpu.jmp(adr);
                return 5;
            };

            // JPE adr
            opcodes[0xea] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.P)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // XDHG
            opcodes[0xeb] = () => {
                Word hi, lo;
                hi = reg[SymRef.D];
                lo = reg[SymRef.E];
                reg[SymRef.E] = reg[SymRef.L];
                reg[SymRef.D] = reg[SymRef.H];
                reg[SymRef.L] = lo;
                reg[SymRef.H] = hi;
                return 5;
            };

            // CPE adr
            opcodes[0xec] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.P)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // NOP
            opcodes[0xed] = cpu.nop;

            // XRI d8
            opcodes[0xee] = () => {
                acc = alu.xor(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // RST 5
            opcodes[0xef] = () => {
                cpu.call(0x28);
                return 11;
            };

            // RP
            opcodes[0xf0] = () => {
                if (!flags.get(Flag.S)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // POP PSW
            opcodes[0xf1] = () => {
                flags.put(cpu.pop());
                reg[SymRef.A] = cpu.pop();
                return 10;
            };

            // JP adr
            opcodes[0xf2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.S)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // DI
            opcodes[0xf3] = () => {
                cpu.IntE = false;
                return 4;
            };

            // CP adr
            opcodes[0xf4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.S)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // PUSH PSW
            opcodes[0xf5] = () => {
                cpu.push(reg[SymRef.A]);
                cpu.push(flags.get());
                return 11;
            };

            // ORI d8
            opcodes[0xf6] = () => {
                acc = alu.or(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
                return 7;
            };

            // RST 5
            opcodes[0xf7] = () => {
                cpu.call(0x30);
                return 11;
            };

            // RM
            opcodes[0xf8] = () => {
                if (flags.get(Flag.S)) {
                    cpu.ret();
                    return 11;
                }
                return 5;
            };

            // SPHL
            opcodes[0xf9] = () => {
                reg.Sp = reg[SymRefD.HL];
                return 5;
            };

            // JM adr
            opcodes[0xfa] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.S)) {
                    cpu.jmp(adr);
                }
                return 10;
            };

            // EI
            opcodes[0xfb] = () => {
                cpu.IntE = true;
                return 4;
            };

            // CM adr
            opcodes[0xfc] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.S)) {
                    cpu.call(adr);
                    return 17;
                }
                return 11;
            };

            // NOP
            opcodes[0xfd] = cpu.nop;


            // CPI d8
            opcodes[0xfe] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                return 7;
            };

            // RST 7
            opcodes[0xff] = () => {
                cpu.call(0x38);
                return 11;
            };
        }

    }
}
