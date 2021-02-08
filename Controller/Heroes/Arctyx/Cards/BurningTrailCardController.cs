using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
	public class BurningTrailCardController : FlameCardController
	{
		public BurningTrailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dd2) => dd2.DidDealDamage && dd2.DamageSource.IsSameCard(base.CharacterCard) && dd2.DamageType == DamageType.Fire, new Func<DealDamageAction, IEnumerator>(base.IncreaseDamageDealtToThatTargetBy1UntilTheStartOfYourNextTurnResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeral = base.GetPowerNumeral(0, 2);
			IEnumerator dealFire = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, DamageType.Fire, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(dealFire);
			}
			else
			{
				base.GameController.ExhaustCoroutine(dealFire);
			}
			yield break;
		}

	}
}