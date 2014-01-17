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
using System.Diagnostics;
using System.Threading;



namespace eZet.i8080.Emulator {

    public class Cpu {

        public bool Debug { get; set; }

        public bool Throttle { get; set; }

        public Register Reg { get; private set; }

        public StatusRegister Flags { get; private set; }

        public Alu Alu { get; private set; }

        public InstructionHandler Decoder { get; private set; }

        public Word instructionRegister { get; private set; }

        public int Cycles { get; private set; }

        public int instCount { get; private set; }

        internal bool IntE { get; set; }

        internal bool Halt { get; set; }

        public IDebug Debugger { get; set; }

        private volatile Word interruptVector;

        private volatile bool Irq;

        private DWord locationCounter;

        private object IrqLock = new object();

        public int Hz { get; set; }

        public int ThrottleFrequency { get; set; }

        public Bus Bus { get; private set; }

        public Cpu(Bus bus) {
            Hz = 2000000;
            this.Bus = bus;
            Reg = new Register();
            Flags = new StatusRegister();
            Alu = new Alu();
            Decoder = new InstructionHandler(this);
            Bus.InterruptEvent += Bus_InterruptEvent;
            ThrottleFrequency = 100;
        }


        internal void run() {
            initialize();
            int throttleCycle = 0;
            int cyclesPerThrottle = Hz / ThrottleFrequency;
            TimeSpan throttleTime = TimeSpan.FromMilliseconds(1000 / ThrottleFrequency);
            TimeSpan begin = TimeSpan.Zero;
            while (!Halt) {
                if (Debug) {
                    Debugger.executing(Reg.Pc);
                }
                if (Throttle) {
                    begin = Process.GetCurrentProcess().TotalProcessorTime;
                }
                fetchNextInstruction();
                var c = Decoder.executeInstruction(instructionRegister);
                Cycles += c;
                throttleCycle += c;
                if (Throttle && throttleCycle > cyclesPerThrottle) {
                    TimeSpan end = Process.GetCurrentProcess().TotalProcessorTime;
                    var wait = throttleTime - (end - begin);
                    throttleCycle = 0;
                    Thread.Sleep(wait);
                }

                // hack
                Reg[SymRef.F] = Flags.get();
                ++instCount;

                lock (IrqLock) {
                    if (IntE && Irq) {
                        IntE = false;
                        Decoder.executeInstruction(interruptVector);
                        Irq = false;
                    }
                }
            }
        }

        public void Bus_InterruptEvent(object sender, BusEventArgs e) {
            lock (IrqLock) {
                if (IntE) {
                    Irq = true;
                    interruptVector = e.Data;
                }
            }
        }


        internal Word DeviceRead(Word port) {
            return Bus.DeviceIn(port);
        }

        internal void deviceWrite(Word port, Word data) {
            Bus.DeviceOut(port, data);
        }

        internal DWord loadPcAddress() {
            return (DWord)(loadPc() | loadPc() << 8);
        }

        internal Word loadPc() {
            Bus.Load(Reg.Pc++);
            return Bus.Data;
        }

        internal Word load(DWord adr) {
            Bus.Load(adr);
            return Bus.Data;
        }

        internal void store(DWord adr, Word data) {
            Bus.Store(adr, data);
        }

        internal void ret() {
            DWord adr = popAddress();
            jmp(adr);
        }

        internal void jmp(DWord adr) {
            if (adr == 0) {
                Console.WriteLine("Jmp 0");
            }
            Reg.Pc = adr;
        }

        internal void call(DWord address) {
            push(Reg.Pc);
            Reg.Pc = address;
            debugPrint(address);
        }

        internal Word pop() {
            Bus.Load(Reg.Sp++);
            return Bus.Data;
        }

        internal DWord popAddress() {
            Word lo = pop();
            Word hi = pop();
            DWord adr = (DWord)(lo | hi << 8);
            return adr;
        }

        internal void push(Word data) {
            Bus.Store(--Reg.Sp, data);
        }

        internal void push(DWord data) {
            push((Word)(data >> 8));
            push((Word)data);
        }

        internal Word nop() { return 4; }

        internal void setFlags(params StatusFlag[] flagList) {
            setFlags(Decoder.acc, flagList);
        }

        internal void setFlags(DWord value, params StatusFlag[] flagList) {
            if (flagList.Contains(StatusFlag.C)) {
                if (Alu.Carry) {
                    Flags.set(StatusFlag.C);
                } else {
                    Flags.clear(StatusFlag.C);
                }
            }

            if (flagList.Contains(StatusFlag.P)) {
                if (evenParity(value)) {
                    Flags.set(StatusFlag.P);
                } else {
                    Flags.clear(StatusFlag.P);
                }
            }

            if (flagList.Contains(StatusFlag.A)) {
                if (Alu.AuxCarry) {
                    Flags.set(StatusFlag.A);
                } else {
                    Flags.clear(StatusFlag.A);
                }
            }

            if (flagList.Contains(StatusFlag.Z)) {
                if (value == 0)
                    Flags.set(StatusFlag.Z);
                else
                    Flags.clear(StatusFlag.Z);
            }

            if (flagList.Contains(StatusFlag.S)) {
                //flag.put(value, StatusFlag.S);
                if ((value & 0x80) != 0)
                    Flags.set(StatusFlag.S);
                else
                    Flags.clear(StatusFlag.S);
            }
        }

        private bool evenParity(int v) {
            v ^= v >> 16;
            v ^= v >> 8;
            v ^= v >> 4;
            v &= 0xf;
            return ((0x6996 >> v) & 1) == 0;
        }

        private void initialize() {
            Flags.reset();
            Alu.reset();
            Halt = false;
            Cycles = 0;
            instCount = 0;
        }
        private void fetchNextInstruction() {
            instructionRegister = loadPc();
        }

        private void debugPrint(DWord address) {
            if (Debug && address == 0x5) {
                if (Reg[SymRef.C] == 9) {
                    DWord adr = (DWord)(Reg[SymRefD.DE]);
                    string str = "";
                    while (load(adr) != '$') {
                        str += (char)load(adr++);
                    }
                    Console.WriteLine(str);
                } else if (Reg[SymRef.C] == 2) {
                    Console.Write(Reg[SymRefD.HL]);
                }
            }
        }


    }


}
