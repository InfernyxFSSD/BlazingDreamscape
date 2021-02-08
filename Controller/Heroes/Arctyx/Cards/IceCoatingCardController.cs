using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class IceCoatingCardController : FrostCardController
    {
        public IceCoatingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>((DealDamageAction dd2) => dd2.DidDealDamage && dd2.DamageSource.IsSameCard(base.CharacterCard) && dd2.DamageType == DamageType.Cold, (DealDamageAction dd) => this.ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dd, 1), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeral = base.GetPowerNumeral(0, 2);
			IEnumerator dealCold = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, DamageType.Cold, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(dealCold);
			}
			else
			{
				base.GameController.ExhaustCoroutine(dealCold);
			}
			yield break;
		}
	}
}