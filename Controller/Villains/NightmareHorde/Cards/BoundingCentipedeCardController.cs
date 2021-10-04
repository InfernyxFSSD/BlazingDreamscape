using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class BoundingCentipedeCardController : FragmentCardController
    {
        public BoundingCentipedeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //after hero target hits non-hero target, this card is immune to damage
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsHero && !dd.Target.IsHero, new Func<DealDamageAction, IEnumerator>(this.ImmuneUntilEndOfTurnResponse), TriggerType.MakeImmuneToDamage, TriggerTiming.After, ActionDescription.DamageTaken);
            //when this card is destroyed, play some cards from the villain deck
            base.AddAfterDestroyedAction(new Func<GameAction, IEnumerator>(this.PlayCardsResponse));
            //at the end of the villain turn, boop highest hero target for 2 melee
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, 2, DamageType.Melee);
        }

        private IEnumerator PlayCardsResponse(GameAction action)
        {
            //IEnumerator playCards = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("fragment")), new int?(base.H - 2));
            IEnumerator playCards = base.GameController.PlayTopCardOfLocation(this.TurnTakerController, this.TurnTaker.Deck, false, (base.H - 2), (base.H - 2), cardSource: base.GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playCards);
            }
            else
            {
                GameController.ExhaustCoroutine(playCards);
            }
            yield break;
        }

        private IEnumerator ImmuneUntilEndOfTurnResponse(DealDamageAction dd)
        {
            ImmuneToDamageStatusEffect itdse = new ImmuneToDamageStatusEffect();
            itdse.TargetCriteria.IsSpecificCard = base.Card;
            itdse.UntilThisTurnIsOver(base.Game);
            itdse.UntilCardLeavesPlay(base.Card);
            IEnumerator immunity = base.AddStatusEffect(itdse);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(immunity);
            }
            else
            {
                GameController.ExhaustCoroutine(immunity);
            }
            yield break;
        }
    }
}