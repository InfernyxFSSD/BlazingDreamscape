using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SybithosInfernyx.Vikral
{
    public class ClarityShotCardController : MicrostormCardController
    {
        public ClarityShotCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 3);
            int powerNumeral2 = base.GetPowerNumeral(1, 1);
            int amount = base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsMicrostorm(c), false, null, false).Count<Card>();
            int amount2 = powerNumeral - amount;
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), amount2, DamageType.Energy, new int?(1), false, new int?(1), false, false, false, null, null, null, null, new Func<DealDamageAction, IEnumerator>(this.IncreaseDamageResponse), true, null, null, false, null, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator IncreaseDamageResponse(DealDamageAction dd)
        {
            if (!dd.DidDealDamage)
            {
                IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
                increaseDamageStatusEffect.TargetCriteria.IsSpecificCard = dd.Target;
                increaseDamageStatusEffect.UntilStartOfNextTurn(base.GameController.FindNextTurnTaker());
                increaseDamageStatusEffect.UntilCardLeavesPlay(dd.Target);
                IEnumerator coroutine = base.AddStatusEffect(increaseDamageStatusEffect, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
