using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZet.i8080.Emulator {
    public interface IDebug {

        void executing(int adr);
    }
}
