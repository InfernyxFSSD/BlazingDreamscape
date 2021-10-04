using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class NightmareNuisancesCardController : CardController
    {
        public NightmareNuisancesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //The first time each turn each Fragment deals damage to a non-villain target, Nightmare Horde deals that target 1 projectile damage
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && !dd.Target.IsVillain && dd.DamageSource.Card.DoKeywordsContain("fragment") && !base.IsPropertyTrue(base.GeneratePerTargetKey("FragmentCardDealtDamage", dd.DamageSource)), new Func<DealDamageAction, IEnumerator>(this.FirstTimeDealDamageResponse), new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            base.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagsAfterLeavesPlay("FragmentCardDealtDamage"), TriggerType.Hidden);
        }

        private IEnumerator FirstTimeDealDamageResponse(DealDamageAction dd)
        {
            base.SetCardPropertyToTrueIfRealAction(base.GeneratePerTargetKey("FragmentCardDealtDamage", dd.DamageSource));
            IEnumerator peckPeck = base.DealDamage(base.CharacterCard, dd.Target, 1, DamageType.Projectile);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(peckPeck);
            }
            else
            {
                GameController.ExhaustCoroutine(peckPeck);
            }
            yield break;
        }

        private const string FragmentCardDealtDamage = "FragmentCardDealtDamage";
    }
}