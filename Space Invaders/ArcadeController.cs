using eZet.i8080.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.Byte;
using DWord = System.UInt16;
using System.Windows.Forms;
using System.Diagnostics;

namespace eZet.i8080.Games.SpaceInvaders {
    public class ArcadeController : IInputDevice {
        [Flags]
        public enum Port0 {

        }

        [Flags]
        public enum Port1 {
            None = 0x0,
            Credit = 0x1, // 0 when active
            P2Start = 0x2,
            P1Start = 0x4,
            Reset = 0x8,
            P1Shot = 0x10,
            P1Left = 0x20,
            P1Right = 0x40,
            //Zero = 0x80,
            All = 0xff,
        }

        [Flags]
        public enum Port2 {
            None = 0x0,
            Dip3 = 0x1,
            Dip5 = 0x2,
            Lives = 0x3, // value + 3
            Tilt = 0x4,
            Dip6 = 0x8,
            P2Shot = 0x10,
            P2Left = 0x20,
            P2Right = 0x40,
            Dip7 = 0x80,
            All = 0xff,
        }

        public ArcadeController() {
            ports[1] = (Byte)Port1.Reset;
        }

        private Word[] ports = new Word[3];
        
        public Word read(DWord port) {
            return (Word)ports[port];
        }

        public void KeyDown(object sender, KeyEventArgs e) {
            Debug.WriteLine(e.KeyCode);
            switch (e.KeyCode) {
                case Keys.Left: set(Port1.P1Left); break;
                case Keys.Right: set(Port1.P1Right); break;
                case Keys.Up: set(Port1.P1Shot); break;
                case Keys.D1: set(Port1.P1Start); break;
                case Keys.D2: set(Port1.P2Start); break;
                case Keys.Space: set(Port1.Credit); break;
            }
        }

        public void KeyUp(object sender, KeyEventArgs e) {
            return;
            Debug.WriteLine("UP: ", e.KeyCode);
            switch (e.KeyCode) {
                case Keys.Left: clear(Port1.P1Left); break;
                case Keys.Right: clear(Port1.P1Right); break;
                case Keys.D1: clear(Port1.P1Shot); break;
                case Keys.D2: clear(Port1.P2Start); break;
                case Keys.Space: clear(Port1.Credit); break;
            }
        }

        private void set(Port1 key) {
            ports[1] |= (Byte)key;
        }

        private void set(Port2 key) {
            ports[2] |= (Byte)key;
        }

        private void clear(Port1 key) {
            ports[1] &= (Byte)key;
        }

        private void clear(Port2 key) {
            ports[2] &= (Byte)key;
        }


    }
}
