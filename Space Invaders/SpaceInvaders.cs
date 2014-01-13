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

        private PictureBox box = new PictureBox();

        private Bitmap bmap;


        public SpaceInvaders() {
            InitializeComponent();
            Controls.Add(box);
            box.Show();
            initializeCpu();
        }

        public void vblank() {
            Debug.WriteLine("VBLANK");
            bmap = new Bitmap(system.getVram());
            box.Image = bmap;
            box.Refresh();

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
            System.Timers.Timer rst2Timer = new System.Timers.Timer(16);
            rst2Timer.AutoReset = true;
            rst2Timer.Elapsed += (source, e) => {
                system.IoController.Interrupt(0, 0xcf); // RST8 
                system.IoController.Interrupt(0, 0xd7); // RST 10 start vblank
            };
            rst2Timer.Enabled = true;

            System.Timers.Timer videoTimer = new System.Timers.Timer(1000);
            videoTimer.AutoReset = true;
            videoTimer.Elapsed += (source, e) => {
                system.draw();
            };
            videoTimer.Enabled = true;
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
