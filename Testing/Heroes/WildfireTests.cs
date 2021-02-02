using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazingDreamscape.Wildfire;

namespace BlazingDreamscape.Testing
{
    [TestFixture()]
    public class WildfireTests : BaseTest
    {
        #region WildfireHelperFunctions
        protected HeroTurnTakerController wildfire { get { return FindHero("Wildfire"); } }

        #endregion

        [Test()]
        public void TestWildfireLoads()
        {
            SetupGameController("BaronBlade", "BlazingDreamscape.Wildfire", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(wildfire);
            Assert.IsInstanceOf(typeof(WildfireCharacterCardController), wildfire.CharacterCardController);

            Assert.AreEqual(28, wildfire.CharacterCard.HitPoints);
        }
    }
}
