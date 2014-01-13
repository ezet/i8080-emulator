using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    public class VideoController {

        public event EventHandler VblankEvent;

        public IVideoDevice Device { get; private set; }

        public Bus Bus { get; private set; }

        public VideoController(Bus bus) {
            this.Bus = bus;
            Bus.VideoEvent += Bus_VideoEvent;
        }

        public void Bus_VideoEvent(object sender, BusEventArgs e) {
            Device.vblank();
        }

        public void attach(IVideoDevice device) {
            this.Device = device;
        }

    }
}
