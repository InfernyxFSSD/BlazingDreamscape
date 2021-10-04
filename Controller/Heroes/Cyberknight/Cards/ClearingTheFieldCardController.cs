using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class ClearingTheFieldCardController : CardController
	{
		//Destroy up to two environment cards

		public ClearingTheFieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			IEnumerator breakEnvironment = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), new int?(2), requiredDecisions: 0, cardSource: base.GetCardSource(null));
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(breakEnvironment);
			}
			else
			{
				GameController.ExhaustCoroutine(breakEnvironment);
			}
			yield break;
		}
	}
}