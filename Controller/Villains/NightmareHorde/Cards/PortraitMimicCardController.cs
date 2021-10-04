using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class PortraitMimicCardController : FragmentCardController
    {
        public PortraitMimicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        //End of villain turn, each player may discard a card. If fewer than (H)-1 discarded, hit highest for 3 melee

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DiscardMaybeDealDamageResponse), new TriggerType[] { TriggerType.DiscardCard, TriggerType.DealDamage });
        }

        private IEnumerator DiscardMaybeDealDamageResponse(PhaseChangeAction phaseChange)
        {
            List<DiscardCardAction> discards = new List<DiscardCardAction>();
            IEnumerator discardCards = base.GameController.EachPlayerDiscardsCards(0, new int?(1), discards, showCounter: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCards);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCards);
            }
            int numberOfDiscards = base.GetNumberOfCardsDiscarded(discards);
            if(numberOfDiscards < (base.H-1))
            {
                IEnumerator bapHero = base.DealDamageToHighestHP(this.Card, 1, (Card c) => c.IsHero, (Card c) => 3, DamageType.Melee);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(bapHero);
                }
                else
                {
                    GameController.ExhaustCoroutine(bapHero);
                }
            }
            yield break;
        }
    }
}