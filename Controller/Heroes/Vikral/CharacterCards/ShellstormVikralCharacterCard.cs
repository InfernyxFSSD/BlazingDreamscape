using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class ShellstormVikralCharacterCardController : HeroCharacterCardController
    {
        public ShellstormVikralCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private ITrigger _reduceTrigger;
        private static readonly string MicrostormReducePropertyKey = "VikralMicrostormReduce";

        public override void AddTriggers()
        {
            _reduceTrigger = base.AddTrigger(base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.CharacterCard && !this.HasBeenSetToTrueThisTurn("VikralHasBeenDealtDamage", null) && base.GetCardPropertyJournalEntryBoolean(MicrostormReducePropertyKey) == true, new Func<DealDamageAction, IEnumerator>(this.ReduceDamageResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.DiscardCard, TriggerType.ReduceDamageLimited }, TriggerTiming.Before, null, false, true, null, false, null, TriggerPriority.High, false, false));
            base.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagAfterLeavesPlay("VikralHasBeenDealtDamage"), TriggerType.Hidden);
        }
        private IEnumerator ReduceDamageResponse(DealDamageAction dd)
        {
            int X = base.FindCardsWhere((Card c) => c.DoKeywordsContain("microstorm", false, false) && c.IsInPlayAndHasGameText, false, null, false).Count<Card>();
            this.SetCardPropertyToTrueIfRealAction("VikralHasBeenDealtDamage", null);
            IEnumerator coroutine = base.GameController.ReduceDamage(dd, X, this._reduceTrigger, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //"Select a hero target. Reduce damage dealt to that target by 1 until the start of your next turn."
                        List<SelectCardDecision> storedResult = new List<SelectCardDecision>();
                        IEnumerator reduceDamage = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.IsHero, "hero target in play", false, false, null, null, false), storedResult, false, false, null, true, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(reduceDamage);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(reduceDamage);
                        }
                        Card selectedCard = base.GetSelectedCard(storedResult);
                        ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                        rdse.TargetCriteria.IsSpecificCard = selectedCard;
                        rdse.UntilStartOfNextTurn(base.TurnTaker);
                        rdse.UntilCardLeavesPlay(selectedCard);
                        reduceDamage = base.AddStatusEffect(rdse, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(reduceDamage);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(reduceDamage);
                        }
                        break;
                    }
                case 1:
                    {
                        //"Until the start of your next turn, whenever a hero is dealt damage, that hero’s player may draw a card."
                        OnDealDamageStatusEffect oddse = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(DrawCardResponse), "Until the start of your next turn, whenever a hero is dealt damage, their player may draw a card.", new TriggerType[] { TriggerType.DrawCard }, base.TurnTaker, this.Card);
                        oddse.TargetCriteria.IsHeroCharacterCard = true;
                        oddse.BeforeOrAfter = BeforeOrAfter.After;
                        oddse.UntilStartOfNextTurn(base.TurnTaker);
                        IEnumerator dealtDamage = base.AddStatusEffect(oddse, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(dealtDamage);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(dealtDamage);
                        }
                        break;
                    }
                case 2:
                    {
                        //"Destroy any number of non-character hero cards.  For each card destroyed this way, a hero regains 2 HP."
                        WhenCardIsDestroyedStatusEffect wcidse = new WhenCardIsDestroyedStatusEffect(CardWithoutReplacements, nameof(GainHPResponse), null, new TriggerType[] { TriggerType.GainHP }, base.HeroTurnTaker, this.Card);
                        wcidse.CardDestroyedCriteria.IsHero = true;
                        wcidse.BeforeOrAfter = BeforeOrAfter.After;
                        wcidse.UntilEndOfPhase(base.TurnTaker, Phase.End);
                        IEnumerator cardDestroyed = base.AddStatusEffect(wcidse, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(cardDestroyed);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(cardDestroyed);
                        }
                        int value = base.FindCardsWhere((Card c) => c.IsHero && !c.IsHeroCharacterCard && c.IsInPlay, false, null, false).Count<Card>();
                        IEnumerator destroyCards = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && !c.IsHeroCharacterCard, "non-character hero card", true, false, null, null, false), new int?(value), false, new int?(0), null, null, null, false, null, null, null, GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(destroyCards);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(destroyCards);
                        }
                        break;
                    }
            }
        }

        private IEnumerator DrawCardResponse(DealDamageAction dd)
        {
            if (dd.Target.IsHeroCharacterCard)
            {
                Card target = dd.Target;
                HeroTurnTakerController heroController = base.FindHeroTurnTakerController(target.Owner.ToHero());
                IEnumerator drawCard = base.GameController.DrawCards(heroController, 1, true, false, null, true, null, null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(drawCard);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(drawCard);
                }
            }
        }

        private IEnumerator GainHPResponse(DestroyCardAction d)
        {
            IEnumerator healHero = base.GameController.SelectAndGainHP(this.DecisionMaker, 2, false, (Card c) => c.IsInPlay && c.IsHeroCharacterCard, 1, null, false, null, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(healHero);
            }
            else
            {
                base.GameController.ExhaustCoroutine(healHero);
            }
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //Power: "Until the end of your next turn, reduce the first damage dealt to Vikral each turn by X, where X is the number of Microstorms in play."
            //Check Scatter Deflector and Energized Chassis (Kyoss) for referencing on ways to make it variable amount per turn and once per turn?
            base.AddCardPropertyJournalEntry(MicrostormReducePropertyKey, true);
            OnPhaseChangeStatusEffect opcse = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DoNothing), $"Until the end of your next turn, reduce the first damage dealt to {base.TurnTaker.Name} by X, where X is the number of Microstorms in play.", new TriggerType[] { TriggerType.ReduceDamage }, this.Card);
            opcse.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
            opcse.UntilEndOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = AddStatusEffect(opcse, true);
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}