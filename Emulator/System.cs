using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Emulator {
    public class System8080 {

        public Cpu Cpu { get; private set; }

        public Bus Bus { get; private set; }

        public IoController IoController { get; private set; }

        public MemoryController MemoryController { get; private set; }

        public VideoController VideoController { get; private set; }

        public IDebug debugger;

        public System8080() {
            Bus = new Bus();
            Cpu = new Cpu(Bus);
            IoController = new IoController(Bus);
            MemoryController = new MemoryController(Bus);
            VideoController = new VideoController(Bus);
        }

        public System8080(IDebug debugger) :this () {
            this.debugger = debugger;
            Cpu.Debug = true;
        }

        public void attachDebugger(IDebug debugger) {
            Cpu.Debug = true;
            Cpu.Debugger = debugger;
        }

        public void loadProgram(Stream input, DWord programBase) {
            if (input.GetType() == typeof(MemoryStream)) {
                MemoryController.StoreProgram((MemoryStream)input, programBase);
            } else {
                using (var ms = new MemoryStream()) {
                    input.CopyTo(ms);
                    MemoryController.StoreProgram(ms, programBase);
                }
            }
            Cpu.Reg.Pc = MemoryController.ProgramBase;
        }

        public void addInput(IInputDevice device, DWord port) {
            IoController.addInput(device, port);
        }

        public void removeInput(DWord port) {
            IoController.removeInput(port);
        }

        public void addOutput(IOutputDevice device, DWord port) {
            IoController.addOutput(device, port);
        }

        public void removeOutput(DWord port) {
            IoController.removeOutput(port);
        }

        public void AddVideo(IVideoDevice device) {
            VideoController.attach(device);
        }

        public void RemoveVideo() {

        }

        public void draw() {
            VideoController.Bus_VideoEvent(this, new BusEventArgs(0));
        }

        public void boot() {
            Cpu.run();
        }

        public Byte[] getVram() {
            return MemoryController.getVram();
        }

        public void RunCpuDiag() {
            var ms = new MemoryStream();
            using (var fs = File.OpenRead("../../../diag/cpudiag.bin")) {
                fs.CopyTo(ms);
            }

            loadProgram(ms, 0x100);

            // Halt
            MemoryController[0x06] = 0x76;
            // skip DAA
            MemoryController[0x59c] = 0xc3;
            MemoryController[0x59d] = 0xc2;
            MemoryController[0x59e] = 0x05;

            boot();
        }
    }
}
