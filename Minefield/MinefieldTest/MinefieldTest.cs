using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minefield.Model;
using Minefield.Persistence;


namespace MinefieldTest
{
    [TestClass]
    public class MinefieldTest
    {
        private MinefieldGameModel _model;

        [TestInitialize]
        public void Initialize()
        {
            _model = new MinefieldGameModel(null);
        }

        [TestMethod]
        public void MinefieldModelNewGameTest()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;

            Assert.AreEqual(_model.GameTime, 0);
            Assert.IsFalse(_model.IsGameOver);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(i==9 && j==9)
                    {
                        Assert.AreEqual(table.fieldValues[i, j], FieldType.Player);
                    }
                    else
                    {
                        Assert.AreEqual(table.fieldValues[i, j], FieldType.Empty);
                    }
                }
            }
        }

        [TestMethod]
        public void MinefieldModelGamOver1Test()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;

            table.fieldValues[8, 9] = FieldType.HeavyB;

            _model.AdvanceTime();

            Assert.AreEqual(_model.IsGameOver, true);
            Assert.AreEqual(table.fieldValues[9, 9], FieldType.HeavyB);

        }

        [TestMethod]
        public void MinefieldModelGamOver2Test()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;
            _model.Step(Dir.Up);

            table.fieldValues[8,8] = FieldType.HeavyB;

            _model.Step(Dir.Left);

            Assert.AreEqual(_model.IsGameOver, true);
            Assert.AreEqual(table.fieldValues[8, 8], FieldType.HeavyB);
            Assert.AreEqual(table.fieldValues[9, 8], FieldType.Empty);
        }

        [TestMethod]
        public void MinefieldModelTimeAdvanceTest()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;

            _model.AdvanceTime();//1
            Assert.AreEqual(_model.GameTime, 1);
            Assert.IsFalse(_model.IsGameOver);

            _model.AdvanceTime();//2
            Assert.AreEqual(_model.GameTime, 2);
            Assert.IsFalse(_model.IsGameOver);

            _model.AdvanceTime();//3
            Assert.AreEqual(_model.GameTime, 3);
            Assert.IsFalse(_model.IsGameOver);

            _model.AdvanceTime();//4
            Assert.AreEqual(_model.GameTime, 4);
            Assert.IsFalse(_model.IsGameOver);

            _model.AdvanceTime();//5
            Assert.AreEqual(_model.GameTime, 5);
            Assert.IsFalse(_model.IsGameOver);

            _model.AdvanceTime();//6
            Assert.AreEqual(_model.GameTime, 6);
            Assert.IsFalse(_model.IsGameOver);

            _model.AdvanceTime();//7
            Assert.AreEqual(_model.GameTime, 7);
            Assert.IsFalse(_model.IsGameOver);

        }

        [TestMethod]
        public void MinefieldModelAdvancedBombsLightTest()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;
            
            table.fieldValues[0, 0] = FieldType.LightB;

            _model.AdvanceBomb(Weight.Light);
            Assert.IsFalse(_model.IsGameOver);
            Assert.AreEqual(table.fieldValues[0, 0], FieldType.Empty);
            Assert.AreEqual(table.fieldValues[1, 0], FieldType.LightB);
        }

        [TestMethod]
        public void MinefieldModelAdvancedBombsMediumTest()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;

            table.fieldValues[0, 0] = FieldType.MediumB;

            _model.AdvanceBomb(Weight.Medium);
            Assert.IsFalse(_model.IsGameOver);
            Assert.AreEqual(table.fieldValues[0, 0], FieldType.Empty);
            Assert.AreEqual(table.fieldValues[1, 0], FieldType.MediumB);

        }

        [TestMethod]
        public void MinefieldModelAdvancedBombsHeavyTest()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;

            table.fieldValues[0, 0] = FieldType.HeavyB;

            _model.AdvanceBomb(Weight.Heavy);
            Assert.IsFalse(_model.IsGameOver);
            Assert.AreEqual(table.fieldValues[0, 0], FieldType.Empty);
            Assert.AreEqual(table.fieldValues[1, 0], FieldType.HeavyB);
        }

        [TestMethod]
        public void MinefieldModelStepTest()
        {
            MinefieldTable table = new MinefieldTable();

            _model.NewGame();

            table = _model.Table;

            Assert.AreEqual(FieldType.Player, table.fieldValues[9, 9]);

            _model.Step(Dir.Up);

            Assert.AreEqual(FieldType.Player, table.fieldValues[8, 9]);

            _model.Step(Dir.Left);

            Assert.AreEqual(FieldType.Player, table.fieldValues[8, 8]);

            _model.Step(Dir.Right);

            Assert.AreEqual(FieldType.Player, table.fieldValues[8, 9]);

            _model.Step(Dir.Down);

            Assert.AreEqual(FieldType.Player, table.fieldValues[9, 9]);

            _model.Step(Dir.Down);//Stepping into a wall, shouldn't happen

            Assert.AreEqual(FieldType.Player, table.fieldValues[9, 9]);
        }

    }
}
