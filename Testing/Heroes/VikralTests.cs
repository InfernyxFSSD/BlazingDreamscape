using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SybithosInfernyx.Vikral;

namespace SybithosInfernyx.Testing
{
    [TestFixture()]
    public class VikralTests : BaseTest
    {
        #region VikralHelperFunctions
        protected HeroTurnTakerController vikral { get { return FindHero("Vikral"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(vikral.CharacterCard, 1);
            DealDamage(villain, vikral, 5, DamageType.Infernal);
        }
        #endregion

        [Test()]
        public void TestVikralLoads()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Vikral", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(vikral);
            Assert.IsInstanceOf(typeof(VikralCharacterCardController), vikral.CharacterCardController);

            Assert.AreEqual(26, vikral.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestSerratedClaws()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Vikral", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutInHand("SerratedClaws");
            Card claws = GetCardFromHand("SerratedClaws");
            GoToPlayCardPhase(vikral);
            PlayCard(claws);
            //Vikral deals a target 2 melee damage
            GoToUsePowerPhase(vikral);
            DecisionSelectTarget = legacy.CharacterCard;
            QuickHPStorage(legacy);
            UsePower(claws);
            QuickHPCheck(-2);
            QuickHPStorage(legacy);
            //At the end of the turn, that target deals itself 1 Toxic damage
            GoToStartOfTurn(legacy);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestToxicCloudTwoSynergy()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Vikral", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutInHand("ToxicCloud");
            PutInHand("OverloadPulse");
            PutInHand("VigilanteJustice");
            PutInHand("Fortitude");
            PlayCard(GetCardFromHand("Fortitude"));
            Card toxic = GetCardFromHand("ToxicCloud");
            Card pulse = GetCardFromHand("OverloadPulse");
            Card justice = GetCardFromHand("VigilanteJustice");
            GoToPlayCardPhase(vikral);
            PlayCards(toxic, pulse, justice);
            QuickHPStorage(tachyon);
            DecisionSelectTarget = tachyon.CharacterCard;
            DecisionSelectTarget = tachyon.CharacterCard;
            DecisionSelectTarget = tachyon.CharacterCard;
            GoToEndOfTurn(vikral);
            QuickHPCheck(-3);

            //Legacy was hit by Toxic Cloud while Pulse and Justice were in play, this should allow the next damage dealt by Legacy to be redirected
            QuickHPStorage(vikral);
            DecisionRedirectTarget = vikral.CharacterCard;
            DealDamage(tachyon, legacy, 4, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestPsionicTorrentWithViciousRetribution()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Vikral", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutInHand("PsionicTorrent");
            PutInHand("ViciousRetribution");
            Card psi = GetCardFromHand("PsionicTorrent");
            Card vic = GetCardFromHand("ViciousRetribution");
            GoToPlayCardPhase(vikral);
            PlayCards(psi, vic);
            QuickHPStorage(legacy);
            DecisionSelectTarget = legacy.CharacterCard;
            DecisionSelectTarget = legacy.CharacterCard;
            GoToEndOfTurn(vikral);
            QuickHPCheck(-2);

            QuickHPStorage(legacy);
            DealDamage(legacy, vikral, 4, DamageType.Melee);
            QuickHPCheck(-1);
        }
    }
}