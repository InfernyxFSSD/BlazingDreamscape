using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class FlameGuardCardController : FlameCardController
    {
        public FlameGuardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddCounterDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard && dealDamage.DamageSource.IsTarget && !dealDamage.DamageSource.IsHero, () => base.CharacterCard, () => base.CharacterCard, true, 2, DamageType.Fire, null, true, null);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator usePower = base.GameController.SelectHeroToUsePower(this.DecisionMaker, false, true, false, null, null, new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.TurnTaker), true, true, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(usePower);
            }
            else
            {
                base.GameController.ExhaustCoroutine(usePower);
            }
            yield break;
        }
    }
}