using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
	public class CloudsOnTheHorizonCardController : CardController
	{
		//At the start of your turn, you may put a Microstorm from your trash into play. Then, if there are at least five microstorms in play, destroy this card.
		//You may draw an extra card during your draw phase.

		public CloudsOnTheHorizonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//How many Microstorms are in your play area?
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.PlayArea, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm") && c.Location == TurnTaker.PlayArea, "microstorm", false, singular: "microstorm", plural: "microstorms"));
			//How many Microstorms are in your trash?
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstomr") && c.Location == TurnTaker.Trash, "microstorm", false, singular: "microstorm", plural: "microstorms"));

		}

		public override void AddTriggers()
		{
			//Get an extra draw
			AddAdditionalPhaseActionTrigger((TurnTaker tt) => ShouldIncreasePhaseActionCount(tt), Phase.DrawCard, 1);
			//May put a microstorm from trash into play at the start of your turn
			AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => StartOfTurnResponse(), new TriggerType[] { TriggerType.PutIntoPlay, TriggerType.DestroySelf });
		}
		private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
		{
			return tt == TurnTaker;
		}

		private IEnumerator StartOfTurnResponse()
        {
			//Search your trash for a microstorm and put it into play
			IEnumerator recoverMicrostorm = SearchForCards(DecisionMaker, false, true, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm"), true, false, false, optional: true);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(recoverMicrostorm);
			}
			else
			{
				GameController.ExhaustCoroutine(recoverMicrostorm);
			}
			//If there are five or more microstorms in play, destroy this card.
			int microstormsInPlay = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("microstorm"))).Count();
			if (microstormsInPlay >= 5)
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