using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace BlazingDreamscape.Wildfire
{
    public class BlazingBunnyCardController : CardController
    {
        public BlazingBunnyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), new TriggerType[] { TriggerType.PutIntoPlay }, null, false);
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.StartOfTurnResponse), new TriggerType[] { TriggerType.DealDamage, TriggerType.ShuffleCardIntoDeck}, null, false);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
        {
            IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == "BlazingBunny" && c.IsInHand));
            IEnumerator moreBunnies = base.GameController.SelectAndPlayCard(this.DecisionMaker, choices, true, true, null, null, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(moreBunnies);
            }
            else
            {
                base.GameController.ExhaustCoroutine(moreBunnies);
            }
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction p)
        {
            int X = base.FindCardsWhere((Card c) => c.Identifier == "BlazingBunny" && c.IsInPlayAndHasGameText).Count<Card>();
            IEnumerator dealFire = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), X, DamageType.Fire, 1, false, 0, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealFire);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealFire);
            }
            IEnumerator shuffleIntoDeck = base.GameController.ShuffleCardIntoLocation(this.DecisionMaker, this.Card, this.TurnTaker.Deck, false, false, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleIntoDeck);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleIntoDeck);
            }
        }
    }
}