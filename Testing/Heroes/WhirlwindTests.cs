using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazingDreamscape.Whirlwind;

namespace BlazingDreamscape.Testing
{
    [TestFixture()]
    public class WhirlwindTests : BaseTest
    {
        #region WhirlwindHelperFunctions
        protected HeroTurnTakerController whirlwind { get { return FindHero("Whirlwind"); } }

        #endregion

        [Test()]
        public void TestWhirlwindLoads()
        {
            SetupGameController("BaronBlade", "BlazingDreamscape.Whirlwind", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(whirlwind);
            Assert.IsInstanceOf(typeof(WhirlwindCharacterCardController), whirlwind.CharacterCardController);

            Assert.AreEqual(26, whirlwind.CharacterCard.HitPoints);
        }
    }
}
