using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    public class Bus {


        public event EventHandler<BusEventArgs> InterruptEvent;

        public event EventHandler<BusEventArgs> LoadEvent;

        public event EventHandler<BusEventArgs> StoreEvent;

        public event EventHandler<BusEventArgs> InEvent;
        
        public event EventHandler<BusEventArgs> OutEvent;

        public event EventHandler<BusEventArgs> VideoEvent;

        private object locker = new object();

        public Word Data { get; set; }

        public DWord Adr { get; set; }

  
        public void Interrupt(DWord adr, Word data) {
            onInterrupt(new BusEventArgs(adr, data));
        }

        public Word Load(DWord adr) {
            lock (locker) {
                onLoad(new BusEventArgs(adr));
                return Data;
            }
        }

        public void Load() {
            throw new NotImplementedException();
        }

        public void Store(DWord adr, Word data) {
            onStore(new BusEventArgs(adr, data));
        }

        public void Store() {
            throw new NotImplementedException();
        }

        public Word DeviceIn(DWord port) {
            lock (locker) {
                onRead(new BusEventArgs(port));
                return Data;
            }
        }

        public Word DeviceIn() {
            throw new NotImplementedException();
        }

        public void DeviceOut(DWord port, Word data) {
            onWrite(new BusEventArgs(port, data));
        }

        public void DeviceOut() {
            throw new NotImplementedException();
        }

        protected virtual void onInterrupt(BusEventArgs e) {
            EventHandler<BusEventArgs> handler = InterruptEvent;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void onLoad(BusEventArgs e) {
            EventHandler<BusEventArgs> handler = LoadEvent;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void onStore(BusEventArgs e) {
            EventHandler<BusEventArgs> handler = StoreEvent;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void onWrite(BusEventArgs e) {
            EventHandler<BusEventArgs> handler = OutEvent;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void onRead(BusEventArgs e) {
            EventHandler<BusEventArgs> handler = InEvent;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void onVblank(BusEventArgs e) {
            EventHandler<BusEventArgs> handler = VideoEvent;
            if (handler != null) {
                handler(this, e);
            }
        }
    }
}
