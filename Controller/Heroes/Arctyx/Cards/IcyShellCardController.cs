using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class IcyShellCardController : FrostCardController
    {
        public IcyShellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.GameController.GainHP(base.CharacterCard, new int?(1), null, null, base.GetCardSource(null)), TriggerType.GainHP, null, false);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 1);
            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(powerNumeral);
            rdse.TargetCriteria.IsHero = true;
            rdse.TargetCriteria.IsCharacter = true;
            rdse.UntilStartOfNextTurn(this.TurnTaker);
            IEnumerator reduceDamage = base.AddStatusEffect(rdse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(reduceDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(reduceDamage);
            }
            yield break;
        }
    }
}