using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class WildCallingCardController : CardController
    {
        //Search your deck or trash for a weather effect and put it into play. If you searched your deck, shuffle it afterwards.

        public WildCallingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many weather effects are in your deck?
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.IsWeatherEffect, "weather effect", false, singular: "weather effect", plural: "weather effects"));
            //How many weather effects are in your trash?
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => c.IsWeatherEffect, "weather effect", false, singular: "weather effect", plural: "weather effects"));
        }

        public override IEnumerator Play()
        {
            //Search either your deck or trash for the right card, and shuffle your deck afterwards if you searched it
            IEnumerator findWeather = SearchForCards(DecisionMaker, true, true, 1, 1, new LinqCardCriteria((Card c) => c.IsWeatherEffect, "weather effect"), true, false, false, shuffleAfterwards: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findWeather);
            }
            else
            {
                GameController.ExhaustCoroutine(findWeather);
            }
            yield break;
        }
    }
}