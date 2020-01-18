using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp
{
    [TestClass]
    public class CleanSoftware_BowlingTest
    {
        private Game _game;

        [TestInitialize]
        public void Init() => _game = new Game();

        [TestMethod]
        public void SingleFrame()
        {
            _game.Throw(5);
            _game.Throw(3);

            Assert.AreEqual(8, _game.Score);
            Assert.AreEqual(8, _game.CurrentFrame.Score);
        }

        [TestMethod]
        public void Spare()
        {
            _game.Throw(8);
            _game.Throw(2);

            _game.Throw(6);

            Assert.AreEqual(16, _game.Frames[0].Score);
            Assert.AreEqual(22, _game.Score);
        }

        [TestMethod]
        public void Strike()
        {
            _game.Throw(10);

            _game.Throw(2);
            _game.Throw(6);

            Assert.AreEqual(18, _game.Frames[0].Score);
            Assert.AreEqual(26, _game.Score);
        }

        [TestMethod]
        public void StrikeAndSpare()
        {
            _game.Throw(10);

            _game.Throw(6);
            _game.Throw(4);

            _game.Throw(5);

            Assert.AreEqual(20, _game.Frames[0].Score);
            Assert.AreEqual(15, _game.Frames[1].Score);
            Assert.AreEqual(40, _game.Score);
        }

        [TestMethod]
        public void ThreeStrikes()
        {
            _game.Throw(10);
            _game.Throw(10);
            _game.Throw(10);

            Assert.AreEqual(30, _game.Frames[0].Score);
            Assert.AreEqual(20, _game.Frames[1].Score);
            Assert.AreEqual(10, _game.Frames[2].Score);
            Assert.AreEqual(60, _game.Score);
        }

        [TestMethod]
        public void PerfectGame()
        {
            for (int i = 0; i < 12; i++)
            {
                _game.Throw(10);
            }

            Assert.AreEqual(300, _game.Score);
        }

    }



    class Game
    {
        private const int NO_OF_FRAMES_IN_A_GAME = 10;

        internal Frame[] Frames { get; } = new Frame[NO_OF_FRAMES_IN_A_GAME];
        private int _frameIndex = 0;

        private readonly List<int> _allThrows = new List<int>();

        public Frame CurrentFrame => Frames[_frameIndex];

        public int Score => Frames.Sum(f => f.Score);

        public Game()
        {
            for (int i = 0; i < NO_OF_FRAMES_IN_A_GAME; i++)
            {
                Frames[i] = new Frame(_allThrows);
            }
        }

        internal void Throw(int pins)
        {
            _allThrows.Add(pins);

            if (_frameIndex == NO_OF_FRAMES_IN_A_GAME - 1)
                return;

            if (CurrentFrame.IsFinished)
            {
                _frameIndex++;
                CurrentFrame.ThrowIndex = _allThrows.Count - 1;
            }

            CurrentFrame.AddThrow(pins);
        }
    }


    class Frame
    {
        private readonly List<int> _allThrows;

        public Frame(List<int> allThrows)
        {
            _allThrows = allThrows;
        }

        public int ThrowCounter { get; private set; }

        public int ThrowIndex { get; internal set; }
        public int NextFrameThrowIndex => IsStrike ? ThrowIndex + 1 : ThrowIndex + 2;

        public int FirstThrow => ThrowCounter == 0 ? 0 : _allThrows[ThrowIndex];
        public int SecondThrow => IsStrike || ThrowCounter < 2 ? 0 : _allThrows[ThrowIndex + 1];

        public int NextThrow => ReadAllThrows(NextFrameThrowIndex);
        public int NextNextThrow => ReadAllThrows(NextFrameThrowIndex + 1);

        public int Score => FirstThrow + SecondThrow + (IsStrike || IsSpare ? NextThrow : 0) + (IsStrike ? NextNextThrow : 0);

        public bool IsFinished => ThrowCounter == 2 || IsStrike;

        public bool IsStrike => FirstThrow == 10;

        public bool IsSpare => ThrowCounter == 2 && FirstThrow + SecondThrow == 10;


        internal void AddThrow(int pins)
        {
            ThrowCounter++;

            if (ThrowCounter > 2)
                throw new Exception("3rd throw in one frame is illegal");
        }

        private int ReadAllThrows(int index) => index < _allThrows.Count ? _allThrows[index] : 0;
    }
}
