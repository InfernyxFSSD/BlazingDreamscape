using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class FlameGuardCardController : FlameCardController
    {
        public FlameGuardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard && dealDamage.DamageType == DamageType.Fire, false);
            base.AddCounterDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.CharacterCard && dealDamage.DamageSource.IsTarget && !dealDamage.DamageSource.IsHero, () => base.CharacterCard, () => base.CharacterCard, true, 2, DamageType.Fire, null, true, null);
        }
    }
}