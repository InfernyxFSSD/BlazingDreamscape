using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
	public class BurningTrailCardController : FlameCardController
	{
		public BurningTrailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => !base.IsPropertyTrue("FirstTimeDamage", null) && dd.DidDealDamage && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Melee, new Func<DealDamageAction, IEnumerator>(this.DealFireDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
			base.AddTrigger<DealDamageAction>((DealDamageAction dd2) => dd2.DidDealDamage && dd2.DamageSource.IsSameCard(base.CharacterCard) && dd2.DamageType == DamageType.Fire, new Func<DealDamageAction, IEnumerator>(base.IncreaseDamageDealtToThatTargetBy1UntilTheStartOfYourNextTurnResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
			base.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagAfterLeavesPlay("FirstTimeDamage"), TriggerType.Hidden);
		}

		private IEnumerator DealFireDamageResponse(DealDamageAction dd)
        {
			base.SetCardPropertyToTrueIfRealAction("FirstTimeDamage", null);
			IEnumerator coroutine = base.DealDamage(base.CharacterCard, dd.Target, 1, DamageType.Fire, false, true, false, null, null, null, false, null);
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

		public const string FirstTimeDamage = "FirstTimeDamage";
	}
}