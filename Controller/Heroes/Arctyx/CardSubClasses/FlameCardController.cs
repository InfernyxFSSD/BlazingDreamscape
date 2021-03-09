using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public abstract class FlameCardController : CardController
	{
		public FlameCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//When one of your Flame cards enters play, break other Flame cards.
			IEnumerator breakFlames = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c != Card && c.DoKeywordsContain("flame"), "flame"), cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(breakFlames);
			}
			else
			{
				GameController.ExhaustCoroutine(breakFlames);
			}
			yield break;
		}
	}
}
