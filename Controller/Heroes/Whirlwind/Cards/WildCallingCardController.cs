using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class WildCallingCardController : CardController
    {
        //Search your deck or trash for a microstorm and put it into play. If you searched your deck, shuffle it afterwards.

        public WildCallingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many Microstorms are in your deck?
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm") && c.Location == TurnTaker.Deck, "microstorm", false, singular: "microstorm", plural: "microstorms"));
            //How many Microstorms are in your trash?
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm") && c.Location == TurnTaker.Trash, "microstorm", false, singular: "microstorm", plural: "microstorms"));
        }

        public override IEnumerator Play()
        {
            //Search either your deck or trash for the right card, and shuffle your deck afterwards if you searched it
            IEnumerator findMicrostorm = SearchForCards(DecisionMaker, true, true, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm"), true, false, false, shuffleAfterwards: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findMicrostorm);
            }
            else
            {
                GameController.ExhaustCoroutine(findMicrostorm);
            }
            yield break;
        }
    }
}