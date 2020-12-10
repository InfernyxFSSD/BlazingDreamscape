using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class IceCoatingCardController : FrostCardController
    {
        public IceCoatingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Melee, (DealDamageAction dd) => base.DealDamage(base.Card, dd.Target, 1, DamageType.Cold, false, false, false, null, null, null, false, null), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Cold, new Func<DealDamageAction, IEnumerator>(this.AddReduceDamageResponse), TriggerType.AddStatusEffectToDamage, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
        }

		private IEnumerator AddReduceDamageResponse(DealDamageAction dd)
		{
            IEnumerator p(DealDamageAction dd2)
            {
                if (!dd2.DidDealDamage)
                {
                    return base.DoNothing();
                }
                IEnumerator enumerator = base.ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dd2, 1);
                if (base.UseUnityCoroutines)
                {
                    return enumerator;
                }
                base.GameController.ExhaustCoroutine(enumerator);
                return base.DoNothing();
            }
            Func<DealDamageAction, IEnumerator> statusEffectResponse = p;
			dd.AddStatusEffectResponse(statusEffectResponse);
			yield return null;
			yield break;
		}
	}
}