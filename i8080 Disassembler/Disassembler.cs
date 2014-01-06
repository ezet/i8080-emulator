using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZet.i8080.Disassembler {
    class Disassembler {
        public delegate String OpcodeHandler(BinaryReader reader);

        private OpcodeHandler[] opcodeMap;

        static void Main(string[] args) {
            FileStream fs = File.OpenRead("../../invaders.h");

            Disassembler diss = new Disassembler();
            List<string> list = diss.disassemble(fs);


        }

        public Disassembler() {
            initMap();
        }

        public List<string> disassemble(Stream input) {
            BinaryReader r = new BinaryReader(input);
            List<string> list = new List<string>();
            int pc = 0;
            while (r.BaseStream.Position < r.BaseStream.Length && pc < 20) {
                byte opcode = r.ReadByte();
                string asm = opcodeMap[opcode].Invoke(r);
                Console.WriteLine(pc++ + ": " + asm);
                list.Add(asm);
            }
            return list;

        }

        private void initMap() {

            opcodeMap = new OpcodeHandler[256];
            opcodeMap[0x00] = r => "NOP";

            // Load Immediate register pair BC
            opcodeMap[0x01] = r => "LXI B, #$" + r.ReadInt16();
            opcodeMap[0x02] = r => "STAX B";
            opcodeMap[0x03] = r => "INX B";
            opcodeMap[0x04] = r => "INR B";
            opcodeMap[0x05] = r => "DCR B";
            opcodeMap[0x06] = r => "MVI B, " + r.ReadByte();
            opcodeMap[0x07] = r => "RLC";

            opcodeMap[0x08] = r => opcodeMap[0x08].Invoke(r);
            opcodeMap[0x09] = r => "DAD B";
            opcodeMap[0x0a] = r => "LDAX B";
            opcodeMap[0x0b] = r => "DCX B";
            opcodeMap[0x0c] = r => "INR C";
            opcodeMap[0x0d] = r => "DCR C";
            opcodeMap[0x0e] = r => "MVI C, " + r.ReadByte();
            opcodeMap[0x0f] = r => "RRC";

            opcodeMap[0x10] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0x11] = r => "LXI D, " + r.ReadInt16();
            opcodeMap[0x12] = r => "STAX D";
            opcodeMap[0x13] = r => "INX D";
            opcodeMap[0x14] = r => "INR D";
            opcodeMap[0x15] = r => "DCR D";
            opcodeMap[0x16] = r => "MVI D, " + r.ReadByte();
            opcodeMap[0x17] = r => "RAL";

            opcodeMap[0x18] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0x19] = r => "DAD D";
            opcodeMap[0x1a] = r => "LDAX D";
            opcodeMap[0x1b] = r => "DCX D";
            opcodeMap[0x1c] = r => "INR E";
            opcodeMap[0x1d] = r => "DCR E";
            opcodeMap[0x1e] = r => "MVI E, " + r.ReadByte();
            opcodeMap[0x1f] = r => "RAR";

            opcodeMap[0x20] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0x21] = r => "LXI H, " + r.ReadInt16();
            opcodeMap[0x22] = r => "SHLD " + r.ReadInt16();
            opcodeMap[0x23] = r => "INX H";
            opcodeMap[0x24] = r => "INR H";
            opcodeMap[0x25] = r => "DCR H";
            opcodeMap[0x26] = r => "MVI H, " + r.ReadByte();
            opcodeMap[0x27] = r => "DAA";

            opcodeMap[0x28] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0x29] = r => "DAD H";
            opcodeMap[0x2a] = r => "LHLD " + r.ReadInt16();
            opcodeMap[0x2b] = r => "DCX H";
            opcodeMap[0x2c] = r => "INR L";
            opcodeMap[0x2d] = r => "DCR L";
            opcodeMap[0x2e] = r => "MVI L, " + r.ReadByte();
            opcodeMap[0x2f] = r => "CMA";

            opcodeMap[0x30] = r => "SIM";
            opcodeMap[0x31] = r => "LXI SP, " + r.ReadInt16();
            opcodeMap[0x32] = r => "STA " + r.ReadInt16();
            opcodeMap[0x33] = r => "INX SP";
            opcodeMap[0x34] = r => "INR M";
            opcodeMap[0x35] = r => "DCR M";
            opcodeMap[0x36] = r => "MVI M, " + r.ReadByte();
            opcodeMap[0x37] = r => "STC";

            opcodeMap[0x38] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0x39] = r => "DAD SP";
            opcodeMap[0x3a] = r => "LDA " + r.ReadInt16();
            opcodeMap[0x3b] = r => "DCX SP";
            opcodeMap[0x3c] = r => "INR A";
            opcodeMap[0x3d] = r => "DCR A";
            opcodeMap[0x3e] = r => "MVI A, " + r.ReadByte();
            opcodeMap[0x3f] = r => "CMC";

            opcodeMap[0x40] = r => "MOV B, B";
            opcodeMap[0x41] = r => "MOV B, C";
            opcodeMap[0x42] = r => "MOV B, D";
            opcodeMap[0x43] = r => "MOV B, E";
            opcodeMap[0x44] = r => "MOV B, H";
            opcodeMap[0x45] = r => "MOV B, L";
            opcodeMap[0x46] = r => "MOV B, M";
            opcodeMap[0x47] = r => "MOV B, A";

            opcodeMap[0x48] = r => "MOV C, B";
            opcodeMap[0x49] = r => "MOV C, C";
            opcodeMap[0x4a] = r => "MOV C, D";
            opcodeMap[0x4b] = r => "MOV C, E";
            opcodeMap[0x4c] = r => "MOV C, H";
            opcodeMap[0x4d] = r => "MOV C, L";
            opcodeMap[0x4e] = r => "MOV C, M";
            opcodeMap[0x4f] = r => "MOV C, A";

            opcodeMap[0x50] = r => "MOV D, B";
            opcodeMap[0x51] = r => "MOV D, C";
            opcodeMap[0x52] = r => "MOV D, D";
            opcodeMap[0x53] = r => "MOV D, E";
            opcodeMap[0x54] = r => "MOV D, H";
            opcodeMap[0x55] = r => "MOV D, L";
            opcodeMap[0x56] = r => "MOV D, M";
            opcodeMap[0x57] = r => "MOV D, A";

            opcodeMap[0x58] = r => "MOV E, B";
            opcodeMap[0x59] = r => "MOV E, C";
            opcodeMap[0x5a] = r => "MOV E, D";
            opcodeMap[0x5b] = r => "MOV E, E";
            opcodeMap[0x5c] = r => "MOV E, H";
            opcodeMap[0x5d] = r => "MOV E, L";
            opcodeMap[0x5e] = r => "MOV E, M";
            opcodeMap[0x5f] = r => "MOV E, A";

            opcodeMap[0x60] = r => "MOV H, B";
            opcodeMap[0x61] = r => "MOV H, C";
            opcodeMap[0x62] = r => "MOV H, D";
            opcodeMap[0x63] = r => "MOV H, E";
            opcodeMap[0x64] = r => "MOV H, H";
            opcodeMap[0x65] = r => "MOV H, L";
            opcodeMap[0x66] = r => "MOV H, M";
            opcodeMap[0x67] = r => "MOV H, A";
            
            opcodeMap[0x68] = r => "MOV L, B";
            opcodeMap[0x69] = r => "MOV L, C";
            opcodeMap[0x6a] = r => "MOV L, D";
            opcodeMap[0x6b] = r => "MOV L, E";
            opcodeMap[0x6c] = r => "MOV L, H";
            opcodeMap[0x6d] = r => "MOV L, L";
            opcodeMap[0x6e] = r => "MOV L, M";
            opcodeMap[0x6f] = r => "MOV L, A";

            opcodeMap[0x70] = r => "MOV M, B";
            opcodeMap[0x71] = r => "MOV M, C";
            opcodeMap[0x72] = r => "MOV M, D";
            opcodeMap[0x73] = r => "MOV M, E";
            opcodeMap[0x74] = r => "MOV M, H";
            opcodeMap[0x75] = r => "MOV M, L";
            opcodeMap[0x76] = r => "HLT";
            opcodeMap[0x77] = r => "MOV M, A";

            opcodeMap[0x78] = r => "MOV A, B";
            opcodeMap[0x79] = r => "MOV A, C";
            opcodeMap[0x7a] = r => "MOV A, D";
            opcodeMap[0x7b] = r => "MOV A, E";
            opcodeMap[0x7c] = r => "MOV A, H";
            opcodeMap[0x7d] = r => "MOV A, L";
            opcodeMap[0x7e] = r => "MOV A, M";
            opcodeMap[0x7f] = r => "MOV A, A";

            opcodeMap[0x80] = r => "ADD B";
            opcodeMap[0x81] = r => "ADD C";
            opcodeMap[0x82] = r => "ADD D";
            opcodeMap[0x83] = r => "ADD E";
            opcodeMap[0x84] = r => "ADD H";
            opcodeMap[0x85] = r => "ADD L";
            opcodeMap[0x86] = r => "ADD M";
            opcodeMap[0x87] = r => "ADD A";

            opcodeMap[0x88] = r => "ADC B";
            opcodeMap[0x89] = r => "ADC C";
            opcodeMap[0x8a] = r => "ADC D";
            opcodeMap[0x8b] = r => "ADC E";
            opcodeMap[0x8c] = r => "ADC H";
            opcodeMap[0x8d] = r => "ADC L";
            opcodeMap[0x8e] = r => "ADC M";
            opcodeMap[0x8f] = r => "ADC A";

            opcodeMap[0x90] = r => "SUB B";
            opcodeMap[0x91] = r => "SUB C";
            opcodeMap[0x92] = r => "SUB D";
            opcodeMap[0x93] = r => "SUB E";
            opcodeMap[0x94] = r => "SUB H";
            opcodeMap[0x95] = r => "SUB L";
            opcodeMap[0x96] = r => "SUB M";
            opcodeMap[0x97] = r => "SUB A";
            
            opcodeMap[0x98] = r => "SBB B";
            opcodeMap[0x99] = r => "SBB C";
            opcodeMap[0x9a] = r => "SBB D";
            opcodeMap[0x9b] = r => "SBB E";
            opcodeMap[0x9c] = r => "SBB H";
            opcodeMap[0x9d] = r => "SBB L";
            opcodeMap[0x9e] = r => "SBB M";
            opcodeMap[0x9f] = r => "SBB A";

            opcodeMap[0xa0] = r => "ANA B";
            opcodeMap[0xa1] = r => "ANA C";
            opcodeMap[0xa2] = r => "ANA D";
            opcodeMap[0xa3] = r => "ANA E";
            opcodeMap[0xa4] = r => "ANA H";
            opcodeMap[0xa5] = r => "ANA L";
            opcodeMap[0xa6] = r => "ANA M";
            opcodeMap[0xa7] = r => "ANA A";

            opcodeMap[0xa8] = r => "XRA B";
            opcodeMap[0xa9] = r => "XRA C";
            opcodeMap[0xaa] = r => "XRA D";
            opcodeMap[0xab] = r => "XRA E";
            opcodeMap[0xac] = r => "XRA H";
            opcodeMap[0xad] = r => "XRA L";
            opcodeMap[0xae] = r => "XRA M";
            opcodeMap[0xaf] = r => "XRA A";

            opcodeMap[0xb0] = r => "ORA B";
            opcodeMap[0xb1] = r => "ORA C";
            opcodeMap[0xb2] = r => "ORA D";
            opcodeMap[0xb3] = r => "ORA E";
            opcodeMap[0xb4] = r => "ORA H";
            opcodeMap[0xb5] = r => "ORA L";
            opcodeMap[0xb6] = r => "ORA M";
            opcodeMap[0xb7] = r => "ORA A";

            opcodeMap[0xb8] = r => "CMP B";
            opcodeMap[0xb9] = r => "CMP C";
            opcodeMap[0xba] = r => "CMP D";
            opcodeMap[0xbb] = r => "CMP E";
            opcodeMap[0xbc] = r => "CMP H";
            opcodeMap[0xbd] = r => "CMP L";
            opcodeMap[0xbe] = r => "CMP M";
            opcodeMap[0xbf] = r => "CMP A";

            opcodeMap[0xc0] = r => "RNZ";
            opcodeMap[0xc1] = r => "POP B";
            opcodeMap[0xc2] = r => "JNZ " + r.ReadInt16();
            opcodeMap[0xc3] = r => "JMP " + r.ReadInt16();
            opcodeMap[0xc4] = r => "CNZ " + r.ReadInt16();
            opcodeMap[0xc5] = r => "PUSH B";
            opcodeMap[0xc6] = r => "ADI " + r.ReadByte();
            opcodeMap[0xc7] = r => "RST 0";

            opcodeMap[0xc8] = r => "RZ";
            opcodeMap[0xc9] = r => "RET";
            opcodeMap[0xca] = r => "JZ " + r.ReadInt16();
            opcodeMap[0xcb] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0xcc] = r => "CZ " + r.ReadInt16();
            opcodeMap[0xcd] = r => "CALL " + r.ReadInt16();
            opcodeMap[0xce] = r => "ACI " + r.ReadByte();
            opcodeMap[0xcf] = r => "RST 1";

            opcodeMap[0xd0] = r => "RNC";
            opcodeMap[0xd1] = r => "POP D";
            opcodeMap[0xd2] = r => "JNC " + r.ReadInt16();
            opcodeMap[0xd3] = r => "OUT " + r.ReadByte();
            opcodeMap[0xd4] = r => "CNC " + r.ReadInt16();
            opcodeMap[0xd5] = r => "PUSH D";
            opcodeMap[0xd6] = r => "SUI "  + r.ReadByte();
            opcodeMap[0xd7] = r => "RST 2";
            opcodeMap[0xd8] = r => "RC";

            opcodeMap[0xd9] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0xda] = r => "JC " + r.ReadInt16();
            opcodeMap[0xdb] = r => "IN " + r.ReadByte();
            opcodeMap[0xdc] = r => "CC " + r.ReadInt16();
            opcodeMap[0xdd] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0xde] = r => "SBI " + r.ReadByte();
            opcodeMap[0xdf] = r => "RST 3";

            opcodeMap[0xe0] = r => "RPO";
            opcodeMap[0xe1] = r => "POP H";
            opcodeMap[0xe2] = r => "JPO " + r.ReadInt16();
            opcodeMap[0xe3] = r => "XTHL";
            opcodeMap[0xe4] = r => "CPO " + r.ReadInt16();
            opcodeMap[0xe5] = r => "PUSH H";
            opcodeMap[0xe6] = r => "ANI " + r.ReadByte();
            opcodeMap[0xe7] = r => "RST 4";

            opcodeMap[0xe8] = r => "RPE";
            opcodeMap[0xe9] = r => "PCHL";
            opcodeMap[0xea] = r => "JPE " + r.ReadInt16();
            opcodeMap[0xeb] = r => "XCHG";
            opcodeMap[0xec] = r => "CPE " + r.ReadInt16();
            opcodeMap[0xed] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0xee] = r => "XRI " + r.ReadByte();
            opcodeMap[0xef] = r => "RST 5";

            opcodeMap[0xf0] = r => "RP";
            opcodeMap[0xf1] = r => "POP PSW";
            opcodeMap[0xf2] = r => "JP " + r.ReadInt16();
            opcodeMap[0xf3] = r => "DI";
            opcodeMap[0xf4] = r => "CP " + r.ReadInt16();
            opcodeMap[0xf5] = r => "PUSH PSW";
            opcodeMap[0xf6] = r => "ORI " + r.ReadByte();
            opcodeMap[0xf7] = r => "RST 6";

            opcodeMap[0xf8] = r => "RM";
            opcodeMap[0xf9] = r => "SPHL";
            opcodeMap[0xfa] = r => "JM " + r.ReadInt16();
            opcodeMap[0xfb] = r => "EI";
            opcodeMap[0xfc] = r => "CM " + r.ReadInt16();
            opcodeMap[0xfd] = r => opcodeMap[0x00].Invoke(r);
            opcodeMap[0xfe] = r => "CPI " + r.ReadByte();
            opcodeMap[0xff] = r => "RST 7";

        }

        private String asmBuilder(string opcode, params byte[] operands) {
            StringBuilder sb = new StringBuilder(opcode);
            return sb.ToString();
        }
    }
}
