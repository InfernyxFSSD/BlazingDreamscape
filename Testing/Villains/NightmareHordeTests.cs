using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using System.Reflection;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazingDreamscape.NightmareHorde;

namespace BlazingDreamscape.Testing
{
    [TestFixture()]
    public class NightmareHordeTests : BaseTest
    {
        #region NightmareHordeHelperFunctions

        protected TurnTakerController nightmare { get { return FindVillain("NightmareHorde"); } }

        #endregion

        [Test()]
        public void TestNightmareHordeLoads()
        {
            SetupGameController("BlazingDreamscape.NightmareHorde", "Legacy", "Tachyon", "Haka", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(nightmare);
            Assert.IsInstanceOf(typeof(NightmareHordeCharacterCardController), nightmare.CharacterCardController);

            Assert.AreEqual(30, nightmare.CharacterCard.HitPoints);
        }
    }
}