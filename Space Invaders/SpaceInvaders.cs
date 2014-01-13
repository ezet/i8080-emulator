using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using eZet.i8080.Diagnostics;
using eZet.i8080.Emulator;
using Debug = System.Diagnostics.Debug;
using System.Drawing.Imaging;

namespace eZet.i8080.Games.SpaceInvaders {
    public partial class SpaceInvaders : Form, IVideoDevice {


        private System8080 system;

        private Debugger debug;

        private BackgroundWorker bw;


        public unsafe SpaceInvaders() {
            InitializeComponent();
            initializeCpu();
        }

        public void vblank() {
            if (this.pictureBox1.InvokeRequired) {
                this.Invoke(new MethodInvoker(refresh));
            } else {
                refresh();
            }
        }

        private unsafe void refresh() {
            Bitmap bmap;
            fixed (byte* p = &system.MemoryController.Ram[system.MemoryController.VramBase]) {
                bmap = new Bitmap(256, 224, 32, PixelFormat.Format1bppIndexed, (IntPtr)p);
            }
            bmap.RotateFlip(RotateFlipType.Rotate270FlipY);
            pictureBox1.Image = null;
            pictureBox1.Image = bmap;
        }

        private void initializeCpu() {
            system = new System8080();
            debug = new Debugger(system);

            system.addOutput(new DebugDevice(6), 6);
            system.addOutput(new DebugDevice(3), 3);
            system.addOutput(new DebugDevice(5), 5);

            system.addInput(new DebugDevice(1, false, 1), 1);
            system.addInput(new DebugDevice(2), 2);

            var shift = new ShiftDevice();
            system.addInput(shift, 3);
            system.addOutput(shift, 2);
            system.addOutput(shift, 4);

            system.AddVideo(this);


            MemoryStream ms = loadInvaders();
            system.loadProgram(ms, 0);

            startInterruptTimer();

            // boot i8080
            bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerAsync(system);

        }

        private void bw_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = sender as BackgroundWorker;
            System8080 system = e.Argument as System8080;
            system.boot();
        }

        private void startInterruptTimer() {
            System.Timers.Timer timer = new System.Timers.Timer(16);
            timer.AutoReset = true;
            timer.Elapsed += (source, e) => {
                system.IoController.Interrupt(0, 0xcf); // RST 8 
                Thread.Sleep(7);
                vblank();
                Thread.Sleep(7);
                system.IoController.Interrupt(0, 0xd7); // RST 10 start vblank
            };
            timer.Enabled = true;
        }

        private MemoryStream loadInvaders() {
            var ms = new MemoryStream();
            using (var fs = File.OpenRead("../../../invaders rom/invaders.h")) {
                fs.CopyTo(ms);
            }
            using (var fs = File.OpenRead("../../../invaders rom/invaders.g")) {
                fs.CopyTo(ms);
            }
            using (var fs = File.OpenRead("../../../invaders rom/invaders.f")) {
                fs.CopyTo(ms);
            }
            using (var fs = File.OpenRead("../../../invaders rom/invaders.e")) {
                fs.CopyTo(ms);
            }
            return ms;
        }
    }
}
