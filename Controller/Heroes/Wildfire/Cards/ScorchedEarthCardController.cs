using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
	public class ScorchedEarthCardController : CardController
	{
		//Wildfire deals each non-hero target 2 fire damage.

		public ScorchedEarthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			//Wildfire hits each non-hero for 2 fire.
			IEnumerator burn = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => c.IsTarget && c.IsInPlayAndHasGameText && !c.IsHero, 2, DamageType.Fire, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(burn);
			}
			else
			{
				GameController.ExhaustCoroutine(burn);
			}
			yield break;
		}
	}
}