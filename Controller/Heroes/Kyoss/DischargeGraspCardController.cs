using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class DischargeGraspCardController : CardController
	{
		public DischargeGraspCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeral = base.GetPowerNumeral(0, 1);
			int powerNumeral2 = base.GetPowerNumeral(1, 1);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, DamageType.Projectile, 1, false, new int?(powerNumeral), false, false, false, null, null, null, null, (DealDamageAction dd) => this.ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dd, powerNumeral2), true, null, null, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}
	}
}
