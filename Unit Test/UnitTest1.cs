using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using eZet.i8080.Emulator;

namespace eZet.i8080.Test {
    [TestClass]
    public class RegistryTest {

        private Register reg = new Register();
        private ushort b = 0xabcd;


        [TestMethod]
        public void TestWordRegister() {
            reg[SymRef.A] = 1;
            Assert.AreEqual(reg[SymRef.A], 1);
            reg[SymRef.B] = 2;
            Assert.AreEqual(reg[SymRef.B], 2);
            reg[SymRef.L2] = 3;
            Assert.AreEqual(reg[SymRef.L2], 3);
            reg[SymRef.I] = 4;
            Assert.AreEqual(reg[SymRef.I], 4);
            reg[SymRef.R] = 5;
            Assert.AreEqual(reg[SymRef.R], 5);
        }

        [TestMethod]
        public void TestDWordRegister() {
            reg[SymRefD.AF] = 1;
            Assert.AreEqual(reg[SymRefD.AF], 1);
            reg[SymRefD.BC] = 2;
            Assert.AreEqual(reg[SymRefD.BC], 2);
            reg[SymRefD.IX] = 3;
            Assert.AreEqual(reg[SymRefD.IX], 3);
            reg[SymRefD.PC] = 4;
            Assert.AreEqual(reg[SymRefD.PC], 4);
            reg[SymRefD.AF] = b;
            Assert.AreEqual(reg[SymRef.A], 0xcd);
            Assert.AreEqual(reg[SymRef.F], 0xab);

        }






    }
}
