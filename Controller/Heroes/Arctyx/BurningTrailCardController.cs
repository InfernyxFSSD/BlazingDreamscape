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
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Melee, (DealDamageAction dd) => base.DealDamage(base.Card, dd.Target, 1, DamageType.Fire, false, false, false, null, null, null, false, null), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Fire, new Func<DealDamageAction, IEnumerator>(base.IncreaseDamageDealtToThatTargetBy1UntilTheStartOfYourNextTurnResponse), TriggerType.AddStatusEffectToDamage, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}
	}
}