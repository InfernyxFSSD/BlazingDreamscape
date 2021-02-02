using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class InfernoWispCardController : CardController
    {
        public InfernoWispCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private const string WildfireDealtEnoughDamage = "WildfireDealtEnoughDamage";

        public override void AddTriggers()
        {
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => true, TargetType.SelectTarget, 1, DamageType.Fire, false, false, 1, 1, null, null);
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => !base.IsPropertyTrue("WildfireDealtEnoughDamage", null) && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount >= 2, new Func<DealDamageAction, IEnumerator>(this.IncreaseNextDamageResponse), new TriggerType[] { TriggerType.IncreaseDamage }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
        }

        private IEnumerator IncreaseNextDamageResponse(DealDamageAction dd)
        {
            base.SetCardPropertyToTrueIfRealAction("WildfireDealtEnoughDamage", null);
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.SourceCriteria.IsSpecificCard = this.Card;
            idse.NumberOfUses = 1;
            IEnumerator coroutine = base.AddStatusEffect(idse, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}