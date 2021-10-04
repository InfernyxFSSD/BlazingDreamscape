using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazingDreamscape.Cyberknight;
using System.Reflection;

namespace BlazingDreamscape.Testing
{
    [TestFixture()]
    public class CyberknightTests : BaseTest
    {
        #region CyberknightHelperFunctions
        protected HeroTurnTakerController ck { get { return FindHero("Cyberknight"); } }

        #endregion

        [Test()]
        public void TestCyberknightLoads()
        {
            SetupGameController("BaronBlade", "BlazingDreamscape.Cyberknight", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ck);
            Assert.IsInstanceOf(typeof(CyberknightCharacterCardController), ck.CharacterCardController);

            Assert.AreEqual(30, ck.CharacterCard.HitPoints);
        }
    }
}
