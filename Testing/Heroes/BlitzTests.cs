using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SybithosInfernyx.Blitz;

namespace SybithosInfernyx.Testing
{
    [TestFixture()]
    public class BlitzTests : BaseTest
    {
        #region BlitzHelperFunctions
        protected HeroTurnTakerController blitz { get { return FindHero("Blitz"); } }

        #endregion

        [Test()]
        public void TestBlitzLoads()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Blitz", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(blitz);
            Assert.IsInstanceOf(typeof(BlitzCharacterCardController), blitz.CharacterCardController);

            Assert.AreEqual(28, blitz.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestElementalWrathNoConduit()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Blitz", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutInHand("InfernoWisp");
            PutInHand("ElementalWrath");
            Card wisp = GetCardFromHand("InfernoWisp");
            Card wrath = GetCardFromHand("ElementalWrath");
            GoToPlayCardPhase(blitz);
            PlayCard(wisp);
            Assert.AreEqual(5, GetCardInPlay("InfernoWisp").HitPoints);
            DecisionSelectCard = GetCardInPlay("InfernoWisp");
            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(baron);
            PlayCard(wrath);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestElementalWrath4Conduit()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Blitz", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutInHand("InfernoWisp");
            PutInHand("ElementalWrath");
            PutIntoPlay("BladeBattalion");
            Card wisp = GetCardFromHand("InfernoWisp");
            Card wrath = GetCardFromHand("ElementalWrath");
            Card bat = GetCardInPlay("BladeBattalion");
            GoToPlayCardPhase(blitz);
            PutIntoPlay("Ruby");
            PutIntoPlay("Ruby");
            PutIntoPlay("Ruby");
            PutIntoPlay("Ruby");
            PlayCard(wisp);
            Assert.AreEqual(5, GetCardInPlay("InfernoWisp").HitPoints);
            DecisionSelectCard = GetCardInPlay("InfernoWisp");
            QuickHPStorage(baron);
            PlayCard(wrath);
            QuickHPCheck(-5);
            AssertInTrash(bat);
        }

        [Test()]
        public void TestScorchedEarth3Conduit()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Blitz", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutInHand("ScorchedEarth");
            Card scorch = GetCardFromHand("ScorchedEarth");
            GoToPlayCardPhase(blitz);
            PutIntoPlay("Ruby");
            PutIntoPlay("Ruby");
            PutIntoPlay("Ruby");
            DecisionAutoDecide = SelectionType.SelectTarget;
            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(baron);
            PlayCard(scorch);
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestConflagrationRodPower()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Blitz", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutIntoPlay("ConflagrationRod");
            Card rod = GetCardInPlay("ConflagrationRod");
            PutOnDeck("ControlledBurn");
            GoToUsePowerPhase(blitz);
            DecisionYesNo = false;
            DecisionAutoDecide = SelectionType.SelectTarget;
            QuickHPStorage(baron);
            UsePower(rod);
            AssertInTrash("ControlledBurn");
            QuickHPCheck(-1);
        }
    }
}
