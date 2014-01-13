using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eZet.i8080.Games.SpaceInvaders {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (object sender, ThreadExceptionEventArgs e) => { Debug.Write(e.ToString()); };
            Application.Run(new SpaceInvaders());
        }
    }
}
