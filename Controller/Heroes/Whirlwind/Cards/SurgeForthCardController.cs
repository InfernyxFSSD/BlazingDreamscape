using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    //Discard any number of cards. Whirlwind deals X targets 3 Cold damage each, where X is the number of cards discarded this way.

    public class SurgeForthCardController : CardController
    {
        public SurgeForthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many cards are in your hand other than this card??
            SpecialStringMaker.ShowNumberOfCards(new LinqCardCriteria((Card c) => c.Location == HeroTurnTaker.Hand && c != Card, "in " + HeroTurnTaker.Hand.GetFriendlyName() + " other than this card"));
        }

        public override IEnumerator Play()
        {
            //Discard any number of cards
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator discardCards = GameController.SelectAndDiscardCards(DecisionMaker, new int?(DecisionMaker.NumberOfCardsInHand), false, 0, storedResults, false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCards);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCards);
            }
            int numTargets = storedResults.Count();
            if (numTargets > 0)
            {
                //If you discarded more than 0 cards, deal that many targets damage
                IEnumerator dealDamage = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Cold, numTargets, false, numTargets, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(dealDamage);
                }
                else
                {
                    GameController.ExhaustCoroutine(dealDamage);
                }
            }
            yield break;
        }
    }
}