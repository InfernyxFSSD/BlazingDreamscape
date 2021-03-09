using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazingDreamscape.Arctyx;

namespace BlazingDreamscape.Testing
{
    [TestFixture()]
    public class ArctyxTests : BaseTest
    {
        #region ArctyxHelperFunctions
        protected HeroTurnTakerController arctyx { get { return FindHero("Arctyx"); } }

        #endregion

        [Test()]
        public void TestArctyxLoads()
        {
            SetupGameController("BaronBlade", "BlazingDreamscape.Arctyx", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(arctyx);
            Assert.IsInstanceOf(typeof(ArctyxCharacterCardController), arctyx.CharacterCardController);

            Assert.AreEqual(30, arctyx.CharacterCard.HitPoints);
        }
    }
}
