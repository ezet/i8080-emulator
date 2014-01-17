using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    public class MemoryController {

        public Bus Bus { get; private set; }

        public Word[] Ram { get; private set; }

        public int Capacity { get; private set; }

        public DWord SystemBase { get; private set; }

        public DWord SystemEnd { get; private set; }

        public DWord ProgramBase { get; private set; }

        public DWord ProgramEnd { get; private set; }

        public DWord VramBase { get; private set; }

        public DWord VramEnd { get; private set; }

        public MemoryController(Bus bus) {
            this.Bus = bus;
            Capacity = DWord.MaxValue + 1;
            Ram = new Word[Capacity];
            VramBase = 0x2400;
            VramEnd = 0x4000;
            Bus.LoadEvent += Bus_LoadEvent;
            Bus.StoreEvent += Bus_StoreEvent;
        }

        public void Bus_LoadEvent(object sender, BusEventArgs e) {
            var bus = sender as Bus;
            bus.Data = Ram[e.Adr];
        }

        public void Bus_StoreEvent(object sender, BusEventArgs e) {
            Ram[e.Adr] = e.Data;
        }

        public Word[] getVram() {
            var tmp = Ram.Skip<Word>(VramBase).Take<Word>(VramEnd - VramBase).ToArray();
            return tmp;
        }

        public Word this[DWord index] {
            get {
                return Ram[index];
            }
            set {
                Ram[index] = value;
            }
        }

        public void StoreProgram(MemoryStream stream, DWord programBase) {
            this.ProgramBase = programBase;
            stream.ToArray().CopyTo(Ram, ProgramBase);
            ProgramEnd = (DWord)stream.Length;
        }

    }
}
