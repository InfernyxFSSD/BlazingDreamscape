using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class SerratedClawsCardController : CardController
    {
        public SerratedClawsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectCardDecision> storedResultsDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, new int?(1), false, new int?(1), false, false, false, null, storedResultsDecision, null, null, new Func<DealDamageAction, IEnumerator>(this.ApplyStatusEffects), true, null, null, false, null, base.GetCardSource(null));
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

        public IEnumerator DealToxicToSelfResponse(PhaseChangeAction _, OnPhaseChangeStatusEffect effect)
        {
            var target = effect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
            if (target.IsTarget && target.IsInPlayAndNotUnderCard)
            {
                IEnumerator coroutine = this.DealDamage(target, target, 1, DamageType.Toxic, false, false, false, null, null, null, false, GetCardSource(effect));
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

        private IEnumerator ApplyStatusEffects(DealDamageAction dd)
        {
            OnPhaseChangeStatusEffect opcse = new OnPhaseChangeStatusEffect(this.CardWithoutReplacements, nameof(DealToxicToSelfResponse), $"{dd.Target.Title} will deal itself one Toxic damage at the end of this turn!", new TriggerType[] { TriggerType.PhaseChange, TriggerType.DealDamage }, this.Card);
            opcse.NumberOfUses = 1;
            opcse.UntilTargetLeavesPlay(dd.Target);
            opcse.TurnPhaseCriteria.Phase = Phase.End;
            opcse.TurnPhaseCriteria.TurnTaker = base.GameController.ActiveTurnTaker;
            opcse.BeforeOrAfter = BeforeOrAfter.After;
            opcse.CardSource = this.Card;
            opcse.UntilStartOfNextTurn(base.GameController.FindNextTurnTaker());
            IEnumerator coroutine = base.AddStatusEffect(opcse, true);
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
