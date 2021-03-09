using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public abstract class FrostCardController : CardController
	{
		public FrostCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//When one of your Frost cards enters play, break other Frost cards.
			IEnumerator breakFrost = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c != Card && c.DoKeywordsContain("frost"), "frost"), cardSource: GetCardSource(null));
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(breakFrost);
			}
			else
			{
				GameController.ExhaustCoroutine(breakFrost);
			}
			yield break;
		}
	}
}
