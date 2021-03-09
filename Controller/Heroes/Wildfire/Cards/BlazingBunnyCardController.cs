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
        //At the end of your turn, you may put a Blazing Bunny from your hand into play.
        //At the start of your turn, this card deals a target X fire damage, where X is the number of Blazing Bunnys in play. Then, shuffle this card into your deck.

        public BlazingBunnyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many Blazing Bunnys are in play?
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.Identifier == "BlazingBunny", "Blazing Bunnys in play"));
        }

        public override void AddTriggers()
        {
            //Put a Blazing Bunny from your hand into play if you want
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, new Func<PhaseChangeAction, IEnumerator>(EndOfTurnResponse), new TriggerType[] { TriggerType.PutIntoPlay });
            //Blazing Bunny deals damage then bounces away
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, new Func<PhaseChangeAction, IEnumerator>(StartOfTurnResponse), new TriggerType[] { TriggerType.DealDamage, TriggerType.ShuffleCardIntoDeck});
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
        {
            //Play more bunnies
            IEnumerable<Card> choices = FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == "BlazingBunny" && c.IsInHand));
            IEnumerator moreBunnies = GameController.SelectAndPlayCard(DecisionMaker, choices, true, true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(moreBunnies);
            }
            else
            {
                GameController.ExhaustCoroutine(moreBunnies);
            }
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction p)
        {
            //Deal damage equal to the number of bunnies in play
            int X = FindCardsWhere((Card c) => c.Identifier == "BlazingBunny" && c.IsInPlayAndHasGameText).Count<Card>();
            IEnumerator dealFire = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), X, DamageType.Fire, 1, false, 0, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(dealFire);
            }
            else
            {
                GameController.ExhaustCoroutine(dealFire);
            }
            //Blazing Bunny bounces...baway. Shut up.
            IEnumerator bounceAway = GameController.ShuffleCardIntoLocation(DecisionMaker, Card, TurnTaker.Deck, false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(bounceAway);
            }
            else
            {
                GameController.ExhaustCoroutine(bounceAway);
            }
            yield break;
        }
    }
}