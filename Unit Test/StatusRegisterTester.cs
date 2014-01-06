using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eZet.i8080.Emulator;
using Flag = eZet.i8080.Emulator.StatusFlag;

namespace eZet.i8080.Test {
    [TestClass]
    public class StatusRegisterTester {

        private StatusRegister flags;

        public StatusRegisterTester() {
            flags = new StatusRegister();
        }

        [TestMethod]
        public void get() {
            flags.reset();
            Assert.IsTrue(flags.get(Flag.Reset));
            Assert.IsFalse(flags.get(Flag.All));
            Assert.IsFalse(flags.get(Flag.A));
            Assert.IsFalse(flags.get(Flag.C));
            Assert.IsFalse(flags.get(Flag.P));
            Assert.IsFalse(flags.get(Flag.S));
            Assert.IsFalse(flags.get(Flag.Z));
        }

        [TestMethod]
        public void set() {
            flags.reset();
            flags.set(Flag.A, Flag.C, Flag.P, Flag.S, Flag.Z);
            assertFlagsTrue();
            flags.set(Flag.All);
            Assert.IsTrue(flags.get(Flag.All));

        }

        [TestMethod]
        public void clear() {
            flags.set(Flag.All);
            flags.clear(Flag.A, Flag.C, Flag.P, Flag.S, Flag.Z);
            assertFlagsFalse();
         }

        [TestMethod]
        public void reset() {
            flags.put(0xff);
            flags.reset();
            Assert.IsTrue(flags.get(Flag.Reset));
            assertFlagsFalse();
        }

        [TestMethod]
        public void toggle() {
            flags.clear();
            flags.toggle(Flag.A, Flag.C, Flag.P, Flag.S, Flag.Z);
            assertFlagsTrue();
            flags.toggle(Flag.A, Flag.C, Flag.P, Flag.S, Flag.Z);
            assertFlagsFalse();
            flags.toggle();
            Assert.IsTrue(flags.get(Flag.All));
        }

        private void assertFlagsTrue() {
            Assert.IsTrue(flags.get(Flag.A));
            Assert.IsTrue(flags.get(Flag.C));
            Assert.IsTrue(flags.get(Flag.P));
            Assert.IsTrue(flags.get(Flag.S));
            Assert.IsTrue(flags.get(Flag.Z));
            Assert.IsTrue(flags.get(Flag.A, Flag.C, Flag.P, Flag.S, Flag.Z));
        }

        private void assertFlagsFalse() {
            Assert.IsFalse(flags.get(Flag.A));
            Assert.IsFalse(flags.get(Flag.C));
            Assert.IsFalse(flags.get(Flag.P));
            Assert.IsFalse(flags.get(Flag.S));
            Assert.IsFalse(flags.get(Flag.Z));
        }

        [TestMethod]
        public void put() {


        }


    }
}
