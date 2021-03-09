using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
	public class GaspingSurgeCardController : CardController
	{
		//When this card enters play, you may play a microstorm.
		//When one of your microstorms is destroyed, Whirlwind may deal a target 1 damage of a type listed on that card.

		public GaspingSurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//When a microstorm is destroyed, may deal damage.
			AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy.Card.DoKeywordsContain("microstorm") && d.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(DealDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
		}

		private IEnumerator DealDamageResponse(DestroyCardAction d)
		{
			var microstormIdentifier = d.CardToDestroy.Card.Identifier;
			var damageType = new DamageType();
			//Check which of your microstorms was destroyed to set damage type
			switch (microstormIdentifier)
            {
				case "HailFlurry":
					damageType = DamageType.Cold;
					break;
				case "VigilanteGales":
					damageType = DamageType.Radiant;
					break;
				case "LightningStorm":
					damageType = DamageType.Lightning;
					break;
				case "ToxicCloud":
					damageType = DamageType.Toxic;
					break;
				case "PsionicTorrent":
					damageType = DamageType.Psychic;
					break;
            }
			//May deal damage of that type
			IEnumerator blowAir = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, damageType, 1, false, 0, cardSource: GetCardSource());
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
			//When this card enters play, may play a Microstorm
			IEnumerator playCard = GameController.SelectAndPlayCardFromHand(DecisionMaker, true, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm")), cardSource: GetCardSource());
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