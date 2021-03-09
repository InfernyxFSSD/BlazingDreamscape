using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BlazingDreamscape.Kyoss21;

namespace BlazingDreamscape.Testing
{
    [TestFixture()]
    public class Kyoss21Tests : BaseTest
    {
        #region Kyoss21HelperFunctions
        protected HeroTurnTakerController kyoss21 { get { return FindHero("Kyoss21"); } }

        #endregion

        [Test()]
        public void TestKyoss21Loads()
        {
            SetupGameController("BaronBlade", "BlazingDreamscape.Kyoss21", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(kyoss21);
            Assert.IsInstanceOf(typeof(Kyoss21CharacterCardController), kyoss21.CharacterCardController);

            Assert.AreEqual(26, kyoss21.CharacterCard.HitPoints);
        }
    }
}
