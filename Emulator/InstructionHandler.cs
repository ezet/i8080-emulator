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

        public delegate void Instruction();

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

        public void executeInstruction(Word opcode) {
            try {
                opcodes[opcode].Invoke();
            } catch (NullReferenceException e) {
                Console.Write(e.ToString());
                Environment.Exit(1);
            }
        }


        private void initOpcodeMap() {

            // NOP
            opcodes[0x00] = cpu.nop;

            // LXI B, d16
            opcodes[0x01] = () => {
                reg[SymRef.C] = cpu.loadPc();
                reg[SymRef.B] = cpu.loadPc();
            };

            // STAX B
            opcodes[0x02] = () => {
                cpu.store(reg[SymRefD.BC], reg[SymRef.A]);
            };

            // INX B
            opcodes[0x03] = () => {
                reg[SymRefD.BC] = alu.add(reg[SymRefD.BC], 1);
            };

            // INR B
            opcodes[0x04] = () => {
                acc = reg[SymRef.B];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.B] = acc;
            };

            // DCR B
            opcodes[0x05] = () => {
                acc = reg[SymRef.B];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.B] = acc;
            };

            // MVI B, d8
            opcodes[0x06] = () => {
                reg[SymRef.B] = cpu.loadPc();
            };

            // RLC
            opcodes[0x07] = () => {
                acc = reg[SymRef.A];
                acc = alu.rotateLeft(acc, 1);
                flags.put(acc, Flag.C);
                reg[SymRef.A] = acc;
            };

            // NOP
            opcodes[0x08] = cpu.nop;

            // DAD B
            opcodes[0x09] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.BC]);
                cpu.setFlags(Flag.C);
            };

            // LDAX B
            opcodes[0x0a] = () => {
                reg[SymRef.A] = cpu.load(reg[SymRefD.BC]);
            };

            // DCX B
            opcodes[0x0b] = () => {
                reg[SymRefD.BC] = alu.sub(reg[SymRefD.BC], 1);
            };

            // INR C
            opcodes[0x0c] = () => {
                acc = reg[SymRef.C];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.C] = acc;
            };

            // DCR C
            opcodes[0x0d] = () => {
                acc = reg[SymRef.C];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.C] = acc;
            };

            // MVI C, d8
            opcodes[0x0e] = () => {
                reg[SymRef.C] = cpu.loadPc();
            };

            // RRC
            opcodes[0x0f] = () => {
                acc = reg[SymRef.A];
                flags.put(acc, Flag.C);
                acc = alu.rotateRight(acc, 1);
                reg[SymRef.A] = acc;
            };

            // NOP
            opcodes[0x10] = cpu.nop;

            // LXI D, d16
            opcodes[0x11] = () => {
                reg[SymRef.E] = cpu.loadPc();
                reg[SymRef.D] = cpu.loadPc();
            };

            // STAX D
            opcodes[0x12] = () => {
                cpu.store(reg[SymRefD.DE], reg[SymRef.A]);
            };

            //INX D
            opcodes[0x13] = () => {
                reg[SymRefD.DE] = alu.add(reg[SymRefD.DE], 1);
            };

            // INR D
            opcodes[0x14] = () => {
                acc = reg[SymRef.D];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.D] = acc;
            };

            // DCR D
            opcodes[0x15] = () => {
                acc = reg[SymRef.D];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.D] = acc;
            };

            // MVI D, d8
            opcodes[0x16] = () => {
                reg[SymRef.D] = cpu.loadPc();
            };

            // RAL
            opcodes[0x17] = () => {
                acc = reg[SymRef.A];
                acc = alu.rotateCarryLeft(acc, 1, flags.get(Flag.C));
                cpu.setFlags(Flag.C);
                reg[SymRef.A] = acc;
            };

            // NOP
            opcodes[0x18] = cpu.nop;

            // DAD D
            opcodes[0x19] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.DE]);
                cpu.setFlags(Flag.C);
            };

            // LDAX D
            opcodes[0x1a] = () => {
                reg[SymRef.A] = cpu.load(reg[SymRefD.DE]);
            };

            // DCX D
            opcodes[0x1b] = () => {
                reg[SymRefD.DE] = alu.sub(reg[SymRefD.DE], 1);
            };

            // INR E
            opcodes[0x1c] = () => {
                acc = reg[SymRef.E];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.E] = acc;
            };

            // DCR E
            opcodes[0x1d] = () => {
                acc = reg[SymRef.E];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.E] = acc;
            };

            // MVI E, d8
            opcodes[0x1e] = () => {
                reg[SymRef.E] = cpu.loadPc();
            };

            // RAR
            opcodes[0x1f] = () => {
                acc = reg[SymRef.A];
                flags.put(acc, Flag.C);
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.rotateCarryRight(acc, 1, carry);
                reg[SymRef.A] = acc;
            };

            // RIM, 8085 only
            opcodes[0x20] = cpu.nop;

            // LXI H, d16
            opcodes[0x21] = () => {
                reg[SymRef.L] = cpu.loadPc();
                reg[SymRef.H] = cpu.loadPc();
            };

            // SHLD adr
            opcodes[0x22] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.store(adr, reg[SymRef.L]);
                cpu.store(++adr,reg[SymRef.H]);
            };

            // INX H
            opcodes[0x23] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], 1);
            };

            // INR H
            opcodes[0x24] = () => {
                acc = reg[SymRef.H];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.H] = acc;
            };

            // DCR H
            opcodes[0x25] = () => {
                acc = reg[SymRef.H];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.H] = acc;
            };

            // MVI H, d8
            opcodes[0x26] = () => {
                reg[SymRef.H] = cpu.loadPc();
            };

            // DAA
            opcodes[0x27] = null;

            // NOP
            opcodes[0x28] = cpu.nop;

            // DAD H
            opcodes[0x29] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg[SymRefD.HL]);
                cpu.setFlags(Flag.C);
            };

            // LHLD adr
            opcodes[0x2a] = () => {
                DWord adr = cpu.loadPcAddress();
                reg[SymRef.L] = cpu.load(adr);
                reg[SymRef.H] = cpu.load(++adr);
            };

            // DCX H
            opcodes[0x2b] = () => {
                reg[SymRefD.HL] = alu.sub(reg[SymRefD.HL], 1);
            };

            // INR L
            opcodes[0x2c] = () => {
                acc = reg[SymRef.L];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.L] = acc;
            };

            // DCR L
            opcodes[0x2d] = () => {
                acc = reg[SymRef.L];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.L] = acc;
            };

            // MVI L, d8
            opcodes[0x2e] = () => {
                reg[SymRef.L] = cpu.loadPc();
            };

            // CMA
            opcodes[0x2f] = () => {
                reg[SymRef.A] = (byte)~reg[SymRef.A];
            };

            // SIM, i8085
            opcodes[0x30] = null;

            // LXI SP, d16
            opcodes[0x31] = () => {
                DWord adr = cpu.loadPcAddress();
                reg.Sp = adr;
            };

            // STA adr
            opcodes[0x32] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.store(adr, reg[SymRef.A]);
            };

            // INX SP
            opcodes[0x33] = () => {
                reg.Sp = alu.add(reg.Sp, 1);
            };

            // INR M
            opcodes[0x34] = () => {
                acc = alu.add(cpu.load(reg[SymRefD.M]), 1);
                cpu.store(reg[SymRefD.M], acc);
                cpu.setFlags(cpu.load(reg[SymRefD.M]), Flag.Z, Flag.S, Flag.P, Flag.A);
                // TODO setflags DWord
            };

            // DCR M
            opcodes[0x35] = () => {
                acc = alu.sub(cpu.load(reg[SymRefD.M]), 1);
                cpu.store(reg[SymRefD.M], acc);
                cpu.setFlags(cpu.load(reg[SymRefD.M]), Flag.Z, Flag.S, Flag.P, Flag.A);
                // TODO setflags DWord
            };


            // MVI M, d8
            opcodes[0x36] = () => {
                cpu.store(reg[SymRefD.M], cpu.loadPc());
            };

            // STC
            opcodes[0x37] = () => {
                flags.set(Flag.C);
            };

            // NOP
            opcodes[0x38] = cpu.nop;

            // DAD SP
            opcodes[0x39] = () => {
                reg[SymRefD.HL] = alu.add(reg[SymRefD.HL], reg.Sp);
            };

            // LDA adr
            opcodes[0x3a] = () => {
                DWord adr = cpu.loadPcAddress();
                reg[SymRef.A] = cpu.load(adr);
            };

            // DCX SP
            opcodes[0x3b] = () => {
                reg.Sp = alu.sub(reg.Sp, 1);
            };

            // INR A
            opcodes[0x3c] = () => {
                acc = reg[SymRef.A];
                acc = alu.add(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.A] = acc;
            };



            // DCR A
            opcodes[0x3d] = () => {
                acc = reg[SymRef.A];
                acc = alu.sub(acc, 1);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.A);
                reg[SymRef.A] = acc;
            };

            // MVI A, d8
            opcodes[0x3e] = () => {
                reg[SymRef.A] = cpu.loadPc();
            };

            // CMC
            opcodes[0x3f] = () => {
                flags.toggle(Flag.C);
            };

            // MOV B, B
            opcodes[0x40] = () => {
                reg[SymRef.B] = reg[SymRef.B];
            };

            // MOV B, C
            opcodes[0x41] = () => {
                reg[SymRef.B] = reg[SymRef.C];
            };

            // MOV B, D
            opcodes[0x42] = () => {
                reg[SymRef.B] = reg[SymRef.D];
            };

            // MOV B, E
            opcodes[0x43] = () => {
                reg[SymRef.B] = reg[SymRef.E];
            };

            // MOV B, H
            opcodes[0x44] = () => {
                reg[SymRef.B] = reg[SymRef.H];
            };

            // MOV B, L
            opcodes[0x45] = () => {
                reg[SymRef.B] = reg[SymRef.L];
            };

            // MOV B, M
            opcodes[0x46] = () => {
                reg[SymRef.B] = cpu.load(reg[SymRefD.M]);
            };

            // MOV B, A
            opcodes[0x47] = () => {
                reg[SymRef.B] = reg[SymRef.A];
            };

            // MOV C, B
            opcodes[0x48] = () => {
                reg[SymRef.C] = reg[SymRef.B];
            };

            // MOV C, C
            opcodes[0x49] = () => {
                reg[SymRef.C] = reg[SymRef.C];
            };

            // MOV C, D
            opcodes[0x4a] = () => {
                reg[SymRef.C] = reg[SymRef.D];
            };

            // MOV C, E
            opcodes[0x4b] = () => {
                reg[SymRef.C] = reg[SymRef.E];
            };

            // MOV C, H
            opcodes[0x4c] = () => {
                reg[SymRef.C] = reg[SymRef.H];
            };

            // MOV C, L
            opcodes[0x4d] = () => {
                reg[SymRef.C] = reg[SymRef.L];
            };

            // MOV C, M
            opcodes[0x4e] = () => {
                reg[SymRef.C] = cpu.load(reg[SymRefD.M]);
            };

            // MOV C, A
            opcodes[0x4f] = () => {
                reg[SymRef.C] = reg[SymRef.A];
            };

            // MOV D, B
            opcodes[0x50] = () => {
                reg[SymRef.D] = reg[SymRef.B];
            };

            // MOV D, C
            opcodes[0x51] = () => {
                reg[SymRef.D] = reg[SymRef.C];
            };

            // MOV D, D
            opcodes[0x52] = () => {
                reg[SymRef.D] = reg[SymRef.D];
            };

            // MOV D, E
            opcodes[0x53] = () => {
                reg[SymRef.D] = reg[SymRef.E];
            };

            // MOV D, H
            opcodes[0x54] = () => {
                reg[SymRef.D] = reg[SymRef.H];
            };

            // MOV D, L
            opcodes[0x55] = () => {
                reg[SymRef.D] = reg[SymRef.L];
            };

            // MOV D, M
            opcodes[0x56] = () => {
                reg[SymRef.D] = cpu.load(reg[SymRefD.M]);
            };

            // MOV D, A
            opcodes[0x57] = () => {
                reg[SymRef.D] = reg[SymRef.A];
            };

            // MOV E, B
            opcodes[0x58] = () => {
                reg[SymRef.E] = reg[SymRef.B];
            };

            // MOV E, C
            opcodes[0x59] = () => {
                reg[SymRef.E] = reg[SymRef.C];
            };

            // MOV E, D
            opcodes[0x5a] = () => {
                reg[SymRef.E] = reg[SymRef.D];
            };

            // MOV E, E
            opcodes[0x5b] = () => {
                reg[SymRef.E] = reg[SymRef.E];
            };

            // MOV E, H
            opcodes[0x5c] = () => {
                reg[SymRef.E] = reg[SymRef.H];
            };

            // MOV E, L
            opcodes[0x5d] = () => {
                reg[SymRef.E] = reg[SymRef.L];
            };

            // MOV E, M
            opcodes[0x5e] = () => {
                reg[SymRef.E] = cpu.load(reg[SymRefD.M]);
            };

            // MOV E, A
            opcodes[0x5f] = () => {
                reg[SymRef.E] = reg[SymRef.A];
            };

            // MOV H, B
            opcodes[0x60] = () => {
                reg[SymRef.H] = reg[SymRef.B];
            };

            // MOV H, C
            opcodes[0x61] = () => {
                reg[SymRef.H] = reg[SymRef.C];
            };

            // MOV H, D
            opcodes[0x62] = () => {
                reg[SymRef.H] = reg[SymRef.D];
            };

            // MOV H, E
            opcodes[0x63] = () => {
                reg[SymRef.H] = reg[SymRef.E];
            };

            // MOV H, H
            opcodes[0x64] = () => {
                reg[SymRef.H] = reg[SymRef.H];
            };

            // MOV H, L
            opcodes[0x65] = () => {
                reg[SymRef.H] = reg[SymRef.L];
            };

            // MOV H, M
            opcodes[0x66] = () => {
                reg[SymRef.H] = cpu.load(reg[SymRefD.M]);
            };

            // MOV H, A
            opcodes[0x67] = () => {
                reg[SymRef.H] = reg[SymRef.A];
            };

            // MOV L, B
            opcodes[0x68] = () => {
                reg[SymRef.L] = reg[SymRef.B];
            };

            // MOV L, C
            opcodes[0x69] = () => {
                reg[SymRef.L] = reg[SymRef.C];
            };

            // MOV L, D
            opcodes[0x6a] = () => {
                reg[SymRef.L] = reg[SymRef.D];
            };

            // MOV L, E
            opcodes[0x6b] = () => {
                reg[SymRef.L] = reg[SymRef.E];
            };

            // MOV L, H
            opcodes[0x6c] = () => {
                reg[SymRef.L] = reg[SymRef.H];
            };

            // MOV L, L
            opcodes[0x6d] = () => {
                reg[SymRef.L] = reg[SymRef.L];
            };

            // MOV L, M
            opcodes[0x6e] = () => {
                reg[SymRef.L] = cpu.load(reg[SymRefD.M]);
            };

            // MOV L, A
            opcodes[0x6f] = () => {
                reg[SymRef.L] = reg[SymRef.A];
            };

            // MOV M, B
            opcodes[0x70] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.B]);
            };

            // MOV M, C
            opcodes[0x71] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.C]);
            };

            // MOV M, D
            opcodes[0x72] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.D]);
            };

            // MOV M, E
            opcodes[0x73] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.E]);
            };

            // MOV M, H
            opcodes[0x74] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.H]);
            };

            // MOV M, L
            opcodes[0x75] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.L]);
            };

            // HLT
            opcodes[0x76] = () => {
                cpu.Halt = true;
            };

            // MOV M, A
            opcodes[0x77] = () => {
                cpu.store(reg[SymRefD.M], reg[SymRef.A]);
            };

            // MOV A, B
            opcodes[0x78] = () => {
                reg[SymRef.A] = reg[SymRef.B];
            };

            // MOV A, C
            opcodes[0x79] = () => {
                reg[SymRef.A] = reg[SymRef.C];
            };

            // MOV A, D
            opcodes[0x7a] = () => {
                reg[SymRef.A] = reg[SymRef.D];
            };

            // MOV A, E
            opcodes[0x7b] = () => {
                reg[SymRef.A] = reg[SymRef.E];
            };

            // MOV A, H
            opcodes[0x7c] = () => {
                reg[SymRef.A] = reg[SymRef.H];
            };

            // MOV A, L
            opcodes[0x7d] = () => {
                reg[SymRef.A] = reg[SymRef.L];
            };

            // MOV A, M
            opcodes[0x7e] = () => {
                reg[SymRef.A] = cpu.load(reg[SymRefD.M]);
            };

            // MOV A, A
            opcodes[0x7f] = () => {
                reg[SymRef.A] = reg[SymRef.A];
            };

            // ADD B
            opcodes[0x80] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD C
            opcodes[0x81] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD D
            opcodes[0x82] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD E
            opcodes[0x83] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD H
            opcodes[0x84] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD L
            opcodes[0x85] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD M
            opcodes[0x86] = () => {
                acc = alu.add(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADD A
            opcodes[0x87] = () => {
                acc = alu.add(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC B
            opcodes[0x88] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.B], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC C
            opcodes[0x89] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.C], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC D
            opcodes[0x8a] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.D], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC E
            opcodes[0x8b] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.E], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC H
            opcodes[0x8c] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.H], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC L
            opcodes[0x8d] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.L], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC M
            opcodes[0x8e] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], cpu.load(reg[SymRefD.M]), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ADC A
            opcodes[0x8f] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], reg[SymRef.A], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB B
            opcodes[0x90] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB C
            opcodes[0x91] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB D
            opcodes[0x92] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB E
            opcodes[0x93] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB H
            opcodes[0x94] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB L
            opcodes[0x95] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB M
            opcodes[0x96] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SUB A
            opcodes[0x97] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB B
            opcodes[0x98] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB C
            opcodes[0x99] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB D
            opcodes[0x9a] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB E
            opcodes[0x9b] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB H
            opcodes[0x9c] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB L
            opcodes[0x9d] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB M
            opcodes[0x9e] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], cpu.load(reg[SymRefD.M]), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // SBB A
            opcodes[0x9f] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A], carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA B
            opcodes[0xa0] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA C
            opcodes[0xa1] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA D
            opcodes[0xa2] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA E
            opcodes[0xa3] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA H
            opcodes[0xa4] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA L
            opcodes[0xa5] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA M
            opcodes[0xa6] = () => {
                acc = alu.and(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ANA A
            opcodes[0xa7] = () => {
                acc = alu.and(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA B
            opcodes[0xa8] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA C
            opcodes[0xa9] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA D
            opcodes[0xaa] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA E
            opcodes[0xab] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA H
            opcodes[0xac] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA L
            opcodes[0xad] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA M
            opcodes[0xae] = () => {
                acc = alu.xor(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // XRA A
            opcodes[0xaf] = () => {
                acc = alu.xor(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA B
            opcodes[0xb0] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA C
            opcodes[0xb1] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA D
            opcodes[0xb2] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA E
            opcodes[0xb3] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA H
            opcodes[0xb4] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA L
            opcodes[0xb5] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA M
            opcodes[0xb6] = () => {
                acc = alu.or(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // ORA A
            opcodes[0xb7] = () => {
                acc = alu.or(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // CMP B
            opcodes[0xb8] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.B]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP C
            opcodes[0xb9] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.C]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP D
            opcodes[0xba] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.D]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP E
            opcodes[0xbb] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.E]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP H
            opcodes[0xbc] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.H]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP L
            opcodes[0xbd] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.L]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP M
            opcodes[0xbe] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.load(reg[SymRefD.M]));
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // CMP A
            opcodes[0xbf] = () => {
                acc = alu.sub(reg[SymRef.A], reg[SymRef.A]);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // RNZ
            opcodes[0xc0] = () => {
                if (!flags.get(Flag.Z)) {
                    cpu.ret();
                }
            };

            // POP B
            opcodes[0xc1] = () => {
                reg[SymRef.C] = cpu.pop();
                reg[SymRef.B] = cpu.pop();
            };

            // JNZ adr
            opcodes[0xc2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.Z)) {
                    cpu.jmp(adr);
                }
            };

            // JMP adr
            opcodes[0xc3] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.jmp(adr);
            };

            // CNZ adr
            opcodes[0xc4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.Z)) {
                    cpu.call(adr);
                }
            };

            // PUSH B
            opcodes[0xc5] = () => {
                cpu.push(reg[SymRef.B]);
                cpu.push(reg[SymRef.C]);
            };

            // ADI d8
            opcodes[0xc6] = () => {
                acc = alu.add(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 0
            opcodes[0xc7] = () => {
                cpu.call(0x00);
            };

            // RZ
            opcodes[0xc8] = () => {
                if (flags.get(Flag.Z)) {
                    cpu.ret();
                }
            };

            // RET
            opcodes[0xc9] = () => {
                cpu.ret();
            };

            // JZ
            opcodes[0xca] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.Z)) {
                    cpu.jmp(adr);
                }
            };

            // NOP
            opcodes[0xcb] = cpu.nop;

            // CZ adr
            opcodes[0xcc] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.Z)) {
                    cpu.call(adr);
                }
            };

            // CALL adr
            opcodes[0xcd] = () => {
                DWord adr = cpu.loadPcAddress();
                cpu.call(adr);
            };

            // ACI d8
            opcodes[0xce] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.add(reg[SymRef.A], cpu.loadPc(), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;

            };

            // RST 1
            opcodes[0xcf] = () => {
                cpu.call(0x08);
            };

            // RNC
            opcodes[0xd0] = () => {
                if (!flags.get(Flag.C)) {
                    cpu.ret();
                }
            };

            // POP D
            opcodes[0xd1] = () => {
                reg[SymRef.E] = cpu.pop();
                reg[SymRef.D] = cpu.pop();
            };

            // JNC adr
            opcodes[0xd2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.C)) {
                    cpu.jmp(adr);
                }
            };

            // OUT d8
            opcodes[0xd3] = () => {
                Word port = cpu.loadPc();
                cpu.deviceWrite(port, reg[SymRef.A]);
            };

            // CNC adr
            opcodes[0xd4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.C)) {
                    cpu.call(adr);
                }
            };

            // PUSH D
            opcodes[0xd5] = () => {
                cpu.push(reg[SymRef.D]);
                cpu.push(reg[SymRef.E]);
            };

            // SUI d8
            opcodes[0xd6] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 2
            opcodes[0xd7] = () => {
                cpu.call(0x10);
            };

            // RC
            opcodes[0xd8] = () => {
                if (flags.get(Flag.C)) {
                    cpu.ret();
                }
            };

            // NOP
            opcodes[0xd9] = cpu.nop;

            // JC adr
            opcodes[0xda] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.C)) {
                    cpu.jmp(adr);
                }
            };

            // IN d8
            opcodes[0xdb] = () => {
                Word port = cpu.loadPc();
                reg[SymRef.A] = cpu.DeviceRead(port);
            };

            // CC adr
            opcodes[0xdc] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.C)) {
                    cpu.call(adr);
                }
            };

            // NOP
            opcodes[0xdd] = cpu.nop;

            // SBI d8
            opcodes[0xde] = () => {
                Word carry = (Word)(flags.get(Flag.C) ? 1 : 0);
                acc = alu.sub(reg[SymRef.A], cpu.loadPc(), carry);
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 3
            opcodes[0xdf] = () => {
                cpu.call(0x18);
            };

            // RPO
            opcodes[0xe0] = () => {
                if (!flags.get(Flag.P)) {
                    cpu.ret();
                }
            };

            // POP H
            opcodes[0xe1] = () => {
                reg[SymRef.L] = cpu.pop();
                reg[SymRef.H] = cpu.pop();
            };

            // JPO adr
            opcodes[0xe2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.P)) {
                    cpu.jmp(adr);
                }
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
            };

            // CPO adr
            opcodes[0xe4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.P)) {
                    cpu.call(adr);
                }
            };

            // PUSH H
            opcodes[0xe5] = () => {
                cpu.push(reg[SymRef.H]);
                cpu.push(reg[SymRef.L]);
            };

            // ANI d8
            opcodes[0xe6] = () => {
                acc = alu.and(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 4
            opcodes[0xe7] = () => {
                cpu.call(0x20);
            };

            // RPE
            opcodes[0xe8] = () => {
                if (flags.get(Flag.P)) {
                    cpu.ret();
                }
            };

            // PCHL
            opcodes[0xe9] = () => {
                DWord adr = (DWord)(reg[SymRef.L] | reg[SymRef.H] << 8);
                cpu.jmp(adr);
            };

            // JPE adr
            opcodes[0xea] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.P)) {
                    cpu.jmp(adr);
                }
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
            };

            // CPE adr
            opcodes[0xec] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.P)) {
                    cpu.call(adr);
                }
            };

            // NOP
            opcodes[0xed] = cpu.nop;

            // XRI d8
            opcodes[0xee] = () => {
                acc = alu.xor(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 5
            opcodes[0xef] = () => {
                cpu.call(0x28);
            };

            // RP
            opcodes[0xf0] = () => {
                if (!flags.get(Flag.S)) {
                    cpu.ret();
                }
            };

            // POP PSW
            opcodes[0xf1] = () => {
                flags.put(cpu.pop());
                reg[SymRef.A] = cpu.pop();
            };

            // JP adr
            opcodes[0xf2] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.S)) {
                    cpu.jmp(adr);
                }
            };

            // DI
            opcodes[0xf3] = () => {
                cpu.IntE = false;
            };

            // CP adr
            opcodes[0xf4] = () => {
                DWord adr = cpu.loadPcAddress();
                if (!flags.get(Flag.S)) {
                    cpu.call(adr);
                }
            };

            // PUSH PSW
            opcodes[0xf5] = () => {
                cpu.push(reg[SymRef.A]);
                cpu.push(flags.get());
            };

            // ORI d8
            opcodes[0xf6] = () => {
                acc = alu.or(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
                reg[SymRef.A] = acc;
            };

            // RST 5
            opcodes[0xf7] = () => {
                cpu.call(0x30);
            };

            // RM
            opcodes[0xf8] = () => {
                if (flags.get(Flag.S)) {
                    cpu.ret();
                }
            };

            //SPHL
            opcodes[0xf9] = () => {
                reg.Sp = reg[SymRefD.HL];
            };

            // JM adr
            opcodes[0xfa] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.S)) {
                    cpu.jmp(adr);
                }
            };

            // EI
            opcodes[0xfb] = () => {
                cpu.IntE = true;
            };

            // CM adr
            opcodes[0xfc] = () => {
                DWord adr = cpu.loadPcAddress();
                if (flags.get(Flag.S)) {
                    cpu.call(adr);
                }
            };

            // NOP
            opcodes[0xfd] = cpu.nop;


            // CPI d8
            opcodes[0xfe] = () => {
                acc = alu.sub(reg[SymRef.A], cpu.loadPc());
                cpu.setFlags(Flag.Z, Flag.S, Flag.P, Flag.C, Flag.A);
            };

            // RST 7
            opcodes[0xef] = () => {
                cpu.call(0x38);
            };
        }

    }
}
