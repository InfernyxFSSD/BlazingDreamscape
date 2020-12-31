using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using SybithosInfernyx.Arctyx;

namespace SybithosInfernyx.Testing
{
    [TestFixture()]
    public class ArctyxTests : BaseTest
    {
        #region ArctyxHelperFunctions
        protected HeroTurnTakerController arctyx { get { return FindHero("Arctyx"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(arctyx.CharacterCard, 1);
            DealDamage(villain, arctyx, 5, DamageType.Infernal);
        }

        protected void AssertNumberOfAurasInHand(TurnTakerController ttc, int number)
        {
            var cardsInHand = ttc.TurnTaker.GetAllCards().Where(c => c.IsInHand && this.IsAura(c));
            var actual = cardsInHand.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in hand, but actually had {2}: {3}", ttc.Name, number, actual, cardsInHand.Select(c => c.Title).ToCommaList()));
        }

        protected void AssertNumberOfAurasInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsAura(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected void AssertNumberOfFlamesInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsFlame(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected void AssertNumberOfFrostsInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsFrost(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected int GetNumberOfAurasInHand(TurnTakerController ttc)
        {
            var cardsInHand = ttc.TurnTaker.GetAllCards().Where(c => c.IsInHand && this.IsAura(c));
            var actual = cardsInHand.Count();
            return actual;
        }

        protected int GetNumberOfAurasInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsAura(c));
            var actual = cardsInPlay.Count();
            return actual;
        }

        private bool IsAura(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "aura", false, false);
        }

        private bool IsFlame(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "flame", false, false);
        }

        private bool IsFrost(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "frost", false, false);
        }

        #endregion

        [Test()]
        public void TestArctyxLoads()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(arctyx);
            Assert.IsInstanceOf(typeof(ArctyxCharacterCardController), arctyx.CharacterCardController);

            Assert.AreEqual(30, arctyx.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestArctyxADLoads()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx/AuraDisplacementArctyxCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(arctyx);
            Assert.IsInstanceOf(typeof(AuraDisplacementArctyxCharacterCardController), arctyx.CharacterCardController);

            Assert.AreEqual(28, arctyx.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestArctyxInnatePower()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            //Arctyx deals one target 2 Melee damage.
            GoToUsePowerPhase(arctyx);
            DecisionSelectTarget = legacy.CharacterCard;
            QuickHPStorage(legacy);
            UsePower(arctyx.CharacterCard);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestArctyxADInnatePowerIsAura()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx/AuraDisplacementArctyxCharacter", "Tachyon", "Legacy", "Megalopolis");
            StartGame();

            //Discard the top card of your deck. If it was an aura, one other player may play a card.
            PutOnDeck("IcyShell");
            PutInHand("Fortitude");
            Card fort = GetCardFromHand("Fortitude");
            GoToUsePowerPhase(arctyx);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectCard = fort;
            UsePower(arctyx.CharacterCard);
            AssertIsInPlay(fort);
        }

        [Test()]
        public void TestArctyxADInnatePowerIsNotAura()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx/AuraDisplacementArctyxCharacter", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            //Discard the top card of your deck. If it was not an aura, one player may draw a card.
            PutOnDeck("DragonHide");
            GoToUsePowerPhase(arctyx);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(legacy);
            UsePower(arctyx.CharacterCard);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestArctyxIncap1()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(arctyx);

            //Reduce damage dealt by a target by 1 until the start of your next turn
            GoToUseIncapacitatedAbilityPhase(arctyx);

            DecisionSelectTarget = legacy.CharacterCard;
            UseIncapacitatedAbility(arctyx, 0);

            QuickHPStorage(tachyon);
            DealDamage(legacy, tachyon, 5, DamageType.Melee);

            //Damage dealt should be 5 -1 =4
            QuickHPCheck(-4);

            //Check that it only lasts until start of your turn
            GoToStartOfTurn(arctyx);
            QuickHPStorage(tachyon);
            DealDamage(legacy, tachyon, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestArctyxADIncap1()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx/AuraDisplacementArctyxCharacter", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(arctyx);

            PutInHand("Fortitude");
            Card fort = GetCardFromHand("Fortitude");
            //One player may play a card
            GoToUseIncapacitatedAbilityPhase(arctyx);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectCard = fort;
            UseIncapacitatedAbility(arctyx, 0);
            AssertIsInPlay(fort);
        }

        [Test()]
        public void TestArctyxIncap2()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(arctyx);

            //Increase damage dealt by a target by 1 until the start of your next turn
            GoToUseIncapacitatedAbilityPhase(arctyx);

            DecisionSelectTarget = legacy.CharacterCard;
            UseIncapacitatedAbility(arctyx, 1);

            QuickHPStorage(tachyon);
            DealDamage(legacy, tachyon, 5, DamageType.Melee);

            //Damage dealt should be 5 +1 =6
            QuickHPCheck(-6);

            //Check that it only lasts until start of your turn
            GoToStartOfTurn(arctyx);
            QuickHPStorage(tachyon);
            DealDamage(legacy, tachyon, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestArctyxADIncap2()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx/AuraDisplacementArctyxCharacter", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(arctyx);

            //One hero may use a power
            GoToUseIncapacitatedAbilityPhase(arctyx);
            DecisionSelectTarget = legacy.CharacterCard;
            UseIncapacitatedAbility(arctyx, 1);
            QuickHPStorage(tachyon);
            DealDamage(legacy, tachyon, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestArctyxIncap3()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(arctyx);

            //One player may draw a card
            GoToUseIncapacitatedAbilityPhase(arctyx);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(legacy);
            UseIncapacitatedAbility(arctyx, 2);
            //Should have one more card in hand
            QuickHandCheck(1);
        }

        [Test()]
        public void TestArctyxADIncap3()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx/AuraDisplacementArctyxCharacter", "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(arctyx);

            //One player may draw a card
            GoToUseIncapacitatedAbilityPhase(arctyx);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(legacy);
            UseIncapacitatedAbility(arctyx, 2);
            //Should have one more card in hand
            QuickHandCheck(1);
        }

        [Test()]
        public void TestHeavyMace()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            PutInHand("HeavyMace");
            Card mace = GetCardFromHand("HeavyMace");
            GoToPlayCardPhase(arctyx);
            PlayCard(mace);
            //Arctyx deals a target 3 melee damage
            GoToUsePowerPhase(arctyx);
            DecisionSelectTarget = legacy.CharacterCard;
            QuickHPStorage(legacy);
            UsePower(mace);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIcyShell()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("IceCoating");
            Card coat = GetCardInPlay("IceCoating");
            PutInHand("IcyShell");
            Card shell = GetCardFromHand("IcyShell");
            SetHitPoints(arctyx, 10);
            GoToPlayCardPhase(arctyx);
            //Playing Icy Shell should destroy Ice Coating, since they're both Frost cards
            PlayCard(shell);
            AssertNotInPlay(coat);
            AssertInTrash(coat);

            //Icy Shell should make Arctyx immune to Cold damage
            QuickHPStorage(arctyx);
            DealDamage(legacy, arctyx, 5, DamageType.Cold);
            QuickHPCheck(0);

            //At the end of your turn, Arctyx should regain 1 HP
            QuickHPStorage(arctyx);
            GoToEndOfTurn(arctyx);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestFlameGuard()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("BurningTrail");
            Card trail = GetCardInPlay("BurningTrail");
            PutInHand("FlameGuard");
            Card guard = GetCardFromHand("FlameGuard");
            GoToPlayCardPhase(arctyx);
            //Playing Flame Guard should destroy Burning Trail, since they're both Flame cards
            PlayCard(guard);
            AssertNotInPlay(trail);
            AssertInTrash(trail);

            //Flame Guard should make Arctyx immune to Fire damage
            QuickHPStorage(arctyx);
            DealDamage(legacy, arctyx, 5, DamageType.Fire);
            QuickHPCheck(0);

            //Flame Guard should retaliate the first time Arctyx is dealt damage each turn
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);
            QuickHPStorage(baron);
            DealDamage(baron, arctyx, 5, DamageType.Melee);
            QuickHPCheck(-2);

            //It should only be the *first* time each turn, so more damage on the same turn shouldn't trigger it
            QuickHPStorage(baron);
            DealDamage(baron, arctyx, 5, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestIceCoating()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("IcyShell");
            Card shell = GetCardInPlay("IcyShell");
            PutInHand("IceCoating");
            Card coat = GetCardFromHand("IceCoating");
            GoToPlayCardPhase(arctyx);
            //Playing Ice Coating should destroy Icy Shell, since they're both Frost cards
            PlayCard(coat);
            AssertNotInPlay(shell);
            AssertInTrash(shell);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);

            //The first time Arctyx deals melee damage each turn, she can also deal 1 cold
            DecisionYesNo = true;
            QuickHPStorage(baron);
            DealDamage(arctyx, baron, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //Should not trigger on the second hit of Melee, or to other targets
            QuickHPStorage(legacy);
            DealDamage(arctyx, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Whenever Arctyx deals cold, reduce damage dealt by that target
            QuickHPStorage(arctyx);
            DealDamage(baron, arctyx, 5, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestBurningTrail()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("FlameGuard");
            Card guard = GetCardInPlay("FlameGuard");
            PutInHand("BurningTrail");
            Card trail = GetCardFromHand("BurningTrail");
            GoToPlayCardPhase(arctyx);
            //Playing Burning Trail should destroy Flame Guard, since they're both Flame cards
            PlayCard(trail);
            AssertNotInPlay(guard);
            AssertInTrash(guard);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, tachyon.CharacterCard);

            //The first time Arctyx deals melee damage each turn, she can also deal 1 fire
            DecisionYesNo = true;
            QuickHPStorage(baron);
            DealDamage(arctyx, baron, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //Should not trigger on the second hit of Melee, or to other targets
            QuickHPStorage(legacy);
            DealDamage(arctyx, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Whenever Arctyx deals fire, increase damage dealt to that target
            QuickHPStorage(baron);
            DealDamage(arctyx, baron, 5, DamageType.Melee);
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestDragonHide()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("DragonHide");
            //Dragon Hide should reduce damage dealt to Arctyx by 1.
            QuickHPStorage(arctyx);
            DealDamage(baron, arctyx, 5, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestBattleRoar()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutInHand("BattleRoar");
            //Battle Roar should redirect damage from villain targets to Arctyx
            GoToPlayCardPhase(arctyx);
            Card roar = GetCardFromHand("BattleRoar");
            PlayCard(roar);
            QuickHPStorage(arctyx);
            DealDamage(baron, tachyon, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestArmorBurst2()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutInHand("ArmorBurst");
            PutIntoPlay("IceCoating");
            PutIntoPlay("BurningTrail");
            Card burst = GetCardFromHand("ArmorBurst");
            Card coat = GetCardInPlay("IceCoating");
            Card trail = GetCardInPlay("BurningTrail");
            GoToPlayCardPhase(arctyx);
            //Playing Armor Burst destroys all auras in play, lets you choose fire or cold, and deals X targets 3 damage each, where X is the number of auras destroyed
            DecisionSelectDamageType = DamageType.Fire;
            DecisionSelectCard = coat;
            DecisionSelectTargets = new Card[] { tachyon.CharacterCard, legacy.CharacterCard };
            QuickHPStorage(tachyon, legacy);
            PlayCard(burst);
            QuickHPCheck(-6, -6);
            AssertInTrash(coat);
            AssertInTrash(trail);
            //It also increases damage dealt to Arctyx by X until the start of your next turn.
            QuickHPStorage(arctyx);
            DealDamage(baron, arctyx, 2, DamageType.Melee);
            QuickHPCheck(-4);

            //Check that it's over after the start of your next turn
            GoToStartOfTurn(arctyx);
            QuickHPStorage(arctyx);
            DealDamage(baron, arctyx, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestArmorBurst1()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutInHand("ArmorBurst");
            PutIntoPlay("IceCoating");
            Card burst = GetCardFromHand("ArmorBurst");
            Card coat = GetCardInPlay("IceCoating");
            GoToPlayCardPhase(arctyx);
            //Playing Armor Burst destroys all auras in play, lets you choose fire or cold, and deals X targets 3 damage each, where X is the number of auras destroyed
            DecisionSelectDamageType = DamageType.Fire;
            DecisionSelectTarget = tachyon.CharacterCard;
            QuickHPStorage(tachyon);
            PlayCard(burst);
            QuickHPCheck(-6);
            AssertInTrash(coat);
            //It also increases damage dealt to Arctyx by X until the start of your next turn.
            QuickHPStorage(arctyx);
            DealDamage(baron, arctyx, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //Check that it's over after the start of your next turn
            GoToStartOfTurn(arctyx);
            QuickHPStorage(arctyx);
            DealDamage(baron, arctyx, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestArmorBurst0()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutInHand("ArmorBurst");
            Card burst = GetCardFromHand("ArmorBurst");
            GoToPlayCardPhase(arctyx);
            //Playing Armor Burst destroys all auras in play, lets you choose fire or cold, and deals X targets 3 damage each, where X is the number of auras destroyed
            PlayCard(burst);
            //It also increases damage dealt to Arctyx by X until the start of your next turn, and X should be 0 here.
            QuickHPStorage(arctyx);
            DealDamage(baron, arctyx, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAuraLace()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("AuraLace");
            PutIntoPlay("IcyShell");
            Card lace = GetCardInPlay("AuraLace");
            Card shell = GetCardInPlay("IcyShell");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp);
            //While an Aura is in play, if Arctyx is dealt damage by a target, increase the next damage dealt to that target by Arctyx by 1.
            DealDamage(legacy, arctyx, 2, DamageType.Melee);
            QuickHPStorage(tachyon, legacy, baron);
            DealDamage(arctyx, tachyon, 2, DamageType.Melee);
            DealDamage(arctyx, legacy, 2, DamageType.Melee);
            DealDamage(arctyx, baron, 2, DamageType.Melee);
            QuickHPCheck(-2, -3, -2);

            //Without an Aura in play, there shouldn't be a damage buff
            DestroyCard(shell);
            DealDamage(legacy, arctyx, 2, DamageType.Melee);
            QuickHPStorage(legacy);
            DealDamage(arctyx, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);


            //Power: Discard 2 cards, then search for an aura from your deck or trash and put it into play, then shuffle your trash into your deck
            DiscardAllCards(arctyx);
            PutInHand("DragonHide");
            PutInHand("TauntTheNewcomers");
            Card hide = GetCardFromHand("DragonHide");
            Card taunt = GetCardFromHand("TauntTheNewcomers");
            IEnumerable<Card> cardsInHand = base.GameController.GetAllCardsInHand(arctyx);
            int numAurasBefore = this.GetNumberOfAurasInPlay(arctyx);
            PutOnDeck("BurningTrail");
            GoToUsePowerPhase(arctyx);
            DecisionDiscardCard = cardsInHand.ElementAt(0);
            DecisionSelectCard = arctyx.HeroTurnTaker.Deck.Cards.Where(c => IsAura(c)).Take(1).First();
            UsePower(lace);
            AssertNumberOfAurasInPlay(arctyx, numAurasBefore + 1);
            AssertNumberOfCardsInTrash(arctyx, 0, null);
        }

        [Test()]
        public void TestFocusedRecovery()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            DiscardAllCards(arctyx);
            PutInHand("FocusedRecovery");
            Card focus = GetCardFromHand("FocusedRecovery");
            PutInHand("IcyShell");
            SetHitPoints(arctyx, 10);

            //Focused Recovery should let Arctyx regain 3 HP, then discard any number of auras to draw that many cards
            GoToPlayCardPhase(arctyx);
            QuickHPStorage(arctyx);
            PlayCard(focus);
            QuickHPCheck(3);
            IEnumerable<Card> cardsInHand = base.GameController.GetAllCardsInHand(arctyx);
            QuickHandStorage(arctyx);
            DecisionDiscardCard = cardsInHand.ElementAt(0);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestOpportuneLunge()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutInHand("OpportuneLunge");
            PutInHand("IcyShell");
            Card lunge = GetCardFromHand("OpportuneLunge");
            Card shell = GetCardFromHand("IcyShell");
            GoToPlayCardPhase(arctyx);

            //Deal a target 2 melee damage, then you may play a card
            DecisionSelectTarget = legacy.CharacterCard;
            DecisionSelectCardToPlay = shell;
            QuickHPStorage(legacy);
            PlayCard(lunge);
            QuickHPCheck(-2);
            AssertIsInPlay(shell);
        }

        [Test()]
        public void TestShiftingAurasFlame()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("ShiftingAuras");
            PutIntoPlay("FlameGuard");
            PutInHand("BurningTrail");
            Card trail = GetCardFromHand("BurningTrail");
            GoToPlayCardPhase(arctyx);

            //When a Flame card is destroyed, Arctyx may deal up to 2 targets 1 fire damage each
            QuickHPStorage(legacy, tachyon);
            DecisionSelectCards = new Card[] { legacy.CharacterCard, tachyon.CharacterCard };
            PlayCard(trail);
            QuickHPCheck(-1, -1);
        }

        [Test()]
        public void TestShiftingAurasFrost()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("ShiftingAuras");
            PutIntoPlay("IcyShell");
            PutInHand("IceCoating");
            Card coat = GetCardFromHand("IceCoating");
            GoToPlayCardPhase(arctyx);

            //When a Frost card is destroyed, Arctyx may deal a target 2 cold damage
            QuickHPStorage(legacy);
            DecisionSelectTarget = legacy.CharacterCard;
            PlayCard(coat);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestTauntTheNewcomers()
        {
            SetupGameController("BaronBlade", "SybithosInfernyx.Arctyx", "Legacy", "Tachyon", "Megalopolis");
            StartGame();
            PutIntoPlay("TauntTheNewcomers");
            PutOnDeck("BladeBattalion");
            GoToPlayCardPhase(baron);
            DecisionYesNo = true;

            //Taunt the Newcomers lets Arctyx optionally deal 1 Sonic damage to a non-hero target when it enters play
            PlayTopCard(baron);
            QuickHPStorage(GetCardInPlay("BladeBattalion"));
            AssertHitPoints(GetCardInPlay("BladeBattalion"), 4);
        }
    }
}
