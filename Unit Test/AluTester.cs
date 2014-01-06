using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using eZet.i8080.Emulator;

using Word = System.Byte;
using DWord = System.UInt16;

namespace eZet.i8080.Test {
    [TestClass]
    public class AluTester {

        private Alu alu;

        public TestContext Context { get; set; }

        public AluTester() {
            alu = new Alu();
        }

        [TestMethod]
        public void add_Word() {
            Word res = alu.add(Word.MaxValue, 1);
            Assert.AreEqual(Word.MinValue, res);
            Assert.IsTrue(alu.Carry);
            res = alu.add(1, 1);
            Assert.AreEqual(2, res);
            Assert.IsFalse(alu.Carry);
        }

        [TestMethod]
        public void add_DWord() {
            DWord res = alu.add(DWord.MaxValue, 1);
            Assert.AreEqual(DWord.MinValue, res);
            Assert.IsTrue(alu.Carry);
            res = alu.add(1, 1);
            Assert.AreEqual(2, res);
            Assert.IsFalse(alu.Carry);
        }

        [TestMethod]
        public void sub_Word() {
            Word res = alu.sub(Word.MinValue, 1);
            Assert.AreEqual(Word.MaxValue, res);
            Assert.IsTrue(alu.Carry);
            res = alu.sub(1, 1);
            Assert.AreEqual(0, res);
            Assert.IsFalse(alu.Carry);
        }

        [TestMethod]
        public void sub_DWord() {
            DWord res = alu.sub(DWord.MinValue, 1);
            Assert.AreEqual(DWord.MaxValue, res);
            Assert.IsTrue(alu.Carry);
            res = alu.sub(1, 1);
            Assert.AreEqual(0, res);
            Assert.IsFalse(alu.Carry);
        }

        [TestMethod]
        public void shiftLeft() {
            Word res, val;
            val = 8;
            res = alu.shiftLeft(val, 4);
            Assert.AreEqual(val << 4, res);
            res = alu.shiftLeft(8, -1);
            Assert.AreEqual(8 << -1, res);
        }

        [TestMethod]
        public void shiftRight() {
            Word res, val;
            val = 8;
            res = alu.shiftRight(val, 4);
            Assert.AreEqual(val >> 4, res);
            res = alu.shiftRight(8, -1);
            Assert.AreEqual(8 >> -1, res);
        }

        [TestMethod]
        public void rotateLeft() {
            Word res = alu.rotateLeft(127, 1);
            Assert.AreEqual(254, res);
            res = alu.rotateLeft(254, 8);
            Assert.AreEqual(254, res);
        }

        [TestMethod]
        public void rotateRight() {
            Word res = alu.rotateRight(254, 1);
            Assert.AreEqual(127, res);
            res = alu.rotateRight(254, 8);
            Assert.AreEqual(254, res);
        }

        [TestMethod]
        public void rotateCarryLeft() {
            // TODO;
        }

        [TestMethod]
        public void rotateCarryRight() {
            // TODO;
        }


    }
}
