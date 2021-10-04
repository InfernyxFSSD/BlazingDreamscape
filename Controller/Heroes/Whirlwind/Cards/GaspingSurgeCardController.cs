using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
	public class GaspingSurgeCardController : CardController
	{
		//When this card enters play, you may play a weather effect.
		//When a weather effect is destroyed, Whirlwind may deal a target 1 energy damage.

		public GaspingSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//When a weather effect is destroyed, may deal damage.
			AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy.Card.IsWeatherEffect && d.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(DealDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
		}

		private IEnumerator DealDamageResponse(DestroyCardAction d)
		{
			IEnumerator blowAir = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Energy, 1, false, 0, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(blowAir);
			}
			else
			{
				GameController.ExhaustCoroutine(blowAir);
			}
			yield break;
		}

		public override IEnumerator Play()
		{
			//When this card enters play, may play a Weather Effect
			IEnumerator playCard = GameController.SelectAndPlayCardFromHand(DecisionMaker, true, cardCriteria: new LinqCardCriteria((Card c) => c.IsWeatherEffect), cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(playCard);
			}
			else
			{
				GameController.ExhaustCoroutine(playCard);
			}
			yield break;
		}
	}
}