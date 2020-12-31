using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SybithosInfernyx.Kyoss;

namespace SybithosInfernyx.Testing
{
    [TestFixture()]
    public class KyossTests : BaseTest
    {
        #region KyossHelperFunctions
        protected HeroTurnTakerController kyoss { get { return FindHero("Kyoss"); } }
        /*private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(kyoss.CharacterCard, 1);
            DealDamage(villain, kyoss, 5, DamageType.Infernal);
        }*/
        #endregion

        [Test()]
        public void TestKyossLoads()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Kyoss", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(kyoss);
            Assert.IsInstanceOf(typeof(KyossCharacterCardController), kyoss.CharacterCardController);

            Assert.AreEqual(26, kyoss.CharacterCard.HitPoints);
        }
    }
}