using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    public class IoController {

        private IInputDevice[] inputDevices;

        private IOutputDevice[] outputDevices;

        public Bus Bus { get; private set; }

        public IoController(Bus bus) {
            this.Bus = bus;
            inputDevices = new IInputDevice[255];
            outputDevices = new IOutputDevice[255];
            Bus.InEvent += Bus_InEvent;
            Bus.OutEvent += Bus_OutEvent;
        }

        public void Interrupt(DWord id, Word data) {
            Bus.Interrupt(id, data);
        }

        public void Bus_InEvent(object sender, BusEventArgs e) {
            Bus.Data = inputDevices[(Word)e.Adr].read((Word)e.Adr);
        }

        public void Bus_OutEvent(object sender, BusEventArgs e) {
            outputDevices[(Word)e.Adr].write((Word)e.Adr, e.Data);
        }

        internal void addInput(IInputDevice device, DWord port) {
            inputDevices[(Word)port] = device;
        }

        internal void removeInput(DWord port) {
            inputDevices[port] = null;
        }

        internal void removeInput(IInputDevice device) {
            removeDevice<IInputDevice>(inputDevices, device);
        }

        internal void addOutput(IOutputDevice device, DWord port) {
            outputDevices[(Word)port] = device;
        }

        internal void removeOutput(DWord port) {
            outputDevices[(Word)port] = null;
        }

        internal void removeOutput(IOutputDevice device) {
            removeDevice<IOutputDevice>(outputDevices, device);
        }

        private void removeDevice<T>(T[] array, T device) {
            for (int i = 0; i < array.Length; ++i) {
                if (array[i].Equals(device)) {
                    array[i] = default(T);
                }
            }
        }
    }
}
