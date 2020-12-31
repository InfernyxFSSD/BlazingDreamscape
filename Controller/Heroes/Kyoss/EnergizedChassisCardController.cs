using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
    public class EnergizedChassisCardController : CardController
    {
        public EnergizedChassisCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private ITrigger _reduceTrigger;

        public override void AddTriggers()
        {
            base.AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);
            _reduceTrigger = base.AddTrigger(base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.CharacterCard && !this.HasBeenSetToTrueThisRound("KyossDiscardedToReduceDamage", null), new Func<DealDamageAction, IEnumerator>(this.ReduceDamageResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.DiscardCard, TriggerType.ReduceDamageLimited }, TriggerTiming.Before, null, false, true, null, false, null, TriggerPriority.High, false, false));
            base.AddTrigger(base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.CharacterCard && !this.HasBeenSetToTrueThisRound("KyossDiscardedToReduceDamage", null) && dd.DidDealDamage, delegate (DealDamageAction dd)
            {
                base.SetCardPropertyToTrueIfRealAction("KyossDiscardedToReduceDamage", null);
                return base.DoNothing();
            }, TriggerType.Hidden, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false));
        }

        private IEnumerator ReduceDamageResponse(DealDamageAction dd)
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, new int?(1), true, null, storedResults, false, null, null, null, SelectionType.DiscardCard, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.GetNumberOfCardsDiscarded(storedResults) == 1)
            {
                this.SetCardPropertyToTrueIfRealAction("KyossDiscardedToReduceDamage", null);
                coroutine = base.GameController.ReduceDamage(dd, 1, this._reduceTrigger, base.GetCardSource(null));
                /*
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                rdse.TargetCriteria.IsSpecificCard = base.CharacterCard;
                rdse.UntilCardLeavesPlay(base.CharacterCard);
                rdse.BeforeOrAfter = BeforeOrAfter.Before;
                rdse.NumberOfUses = 1;
                coroutine = base.AddStatusEffect(rdse, true);*/
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
        //private const string DiscardedToReduceDamage = "KyossDiscardedToReduceDamage";
    }
}