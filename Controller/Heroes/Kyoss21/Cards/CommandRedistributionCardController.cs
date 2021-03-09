using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class CommandRedistributionCardController : CardController
    {
        //Destroy up to 2 ongoing or environment cards. Up to X players may play a card, where X is the number of cards destroyed this way.
        public CommandRedistributionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //Destroy up to 2 ongoing/environment cards
            List<DestroyCardAction> destroyedCards = new List<DestroyCardAction>();
            IEnumerator destroyCards = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "ongoing or environment", true, false, null, null, false), new int?(2), false, new int?(0), storedResultsAction: destroyedCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyCards);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyCards);
            }
            int num = GetNumberOfCardsDestroyed(destroyedCards);
            if(num > 0)
            {
                //If more than 0 cards were destroyed this way, that many players may play a card
                List<TurnTaker> selectedHeroes = new List<TurnTaker>();
                for (int i = 0; i < num; i++)
                {
                    List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                    IEnumerator givePlay = GameController.SelectHeroToPlayCard(DecisionMaker, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => !selectedHeroes.Contains(tt)), storedResultsTurnTaker: storedResults, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(givePlay);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(givePlay);
                    }
                    TurnTaker chosenHero = GetSelectedTurnTaker(storedResults);
                    selectedHeroes.Add(chosenHero);
                    storedResults = null;
                }
            }
            yield break;
        }
    }
}