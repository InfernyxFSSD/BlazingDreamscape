using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazingDreamscape.Sploder;

namespace BlazingDreamscape.Testing
{
    [TestFixture()]
    public class SploderTests : BaseTest
    {
        #region SploderHelperFunctions
        protected HeroTurnTakerController sploder { get { return FindHero("Sploder"); } }

        #endregion

        [Test()]
        public void TestSploderLoads()
        {
            SetupGameController("BaronBlade", "BlazingDreamscape.Sploder", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(sploder);
            Assert.IsInstanceOf(typeof(SploderCharacterCardController), sploder.CharacterCardController);

            Assert.AreEqual(15, sploder.CharacterCard.HitPoints);
        }
    }
}
