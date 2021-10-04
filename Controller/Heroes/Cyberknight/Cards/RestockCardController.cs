using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class RestockCardController : CardController
	{
		//Draw three cards

		public RestockCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator drawCards = base.DrawCards(this.DecisionMaker, 3);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(drawCards);
			}
			else
			{
				GameController.ExhaustCoroutine(drawCards);
			}
		}
	}
}