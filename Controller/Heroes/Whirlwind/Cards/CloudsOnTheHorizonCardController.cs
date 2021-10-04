using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
	public class CloudsOnTheHorizonCardController : CardController
	{
		//At the start of your turn, you may put a Weather Effect from your trash into play. Then, if there are at least five Weather Effects in play, destroy this card.
		//You may draw an extra card during your draw phase.

		public CloudsOnTheHorizonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//How many Weather Effects are in your play area?
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.PlayArea, new LinqCardCriteria((Card c) => c.IsWeatherEffect, "weather effect", false, singular: "weather effect", plural: "weather effects"));
			//How many Weather Effects are in your trash?
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => c.IsWeatherEffect, "weather effect", false, singular: "weather effect", plural: "weather effects"));

		}

		public override void AddTriggers()
		{
			//Get an extra draw
			AddAdditionalPhaseActionTrigger((TurnTaker tt) => ShouldIncreasePhaseActionCount(tt), Phase.DrawCard, 1);
			//May put a weather effect from trash into play at the start of your turn
			AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => StartOfTurnResponse(), new TriggerType[] { TriggerType.PutIntoPlay, TriggerType.DestroySelf });
		}
		private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
		{
			return tt == TurnTaker;
		}

		private IEnumerator StartOfTurnResponse()
        {
			//Search your trash for a weather effect and put it into play
			IEnumerator recoverWeather = SearchForCards(DecisionMaker, false, true, 1, 1, new LinqCardCriteria((Card c) => c.IsWeatherEffect, "weather effect"), true, false, false, optional: true);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(recoverWeather);
			}
			else
			{
				GameController.ExhaustCoroutine(recoverWeather);
			}
			//If there are five or more microstorms in play, destroy this card.
			int weathersInPlay = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsWeatherEffect)).Count();
			if (weathersInPlay >= 5)
            {
				IEnumerator destroySelf = GameController.DestroyCard(DecisionMaker, Card, responsibleCard: Card, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(destroySelf);
				}
				else
				{
					GameController.ExhaustCoroutine(destroySelf);
				}
			}
			yield break;
		}
	}
}