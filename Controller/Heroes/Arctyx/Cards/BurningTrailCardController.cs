using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public class BurningTrailCardController : FlameCardController
	{

		//When this card enters play, destroy any other flame cards in your play area.
		//When Arctyx deals a target fire damage, increase damage dealt to that target by 1 until the start of your next turn.
		//Power: Arctyx deals a target 2 fire damage.

		public BurningTrailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//When Arctyx deals fire, increase damage dealt to that target by 1
			AddTrigger<DealDamageAction>((DealDamageAction dd2) => dd2.DidDealDamage && dd2.DamageSource.IsSameCard(CharacterCard) && dd2.DamageType == DamageType.Fire, new Func<DealDamageAction, IEnumerator>(IncreaseDamageDealtToThatTargetBy1UntilTheStartOfYourNextTurnResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//Select a target and hit for 2 fire
			int powerNumeral = GetPowerNumeral(0, 2);
			IEnumerator dealFire = GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeral, DamageType.Fire, 1, false, 1, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(dealFire);
			}
			else
			{
				GameController.ExhaustCoroutine(dealFire);
			}
			yield break;
		}

	}
}