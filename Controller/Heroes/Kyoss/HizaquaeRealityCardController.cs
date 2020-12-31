using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
    public class HizaquaeRealityCardController : CardController
    {
        public HizaquaeRealityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.DealDamageOrDestroyThisCardResponse(p, base.CharacterCard, base.CharacterCard, 2, DamageType.Energy, true), new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            }, null, false);
            base.AddTrigger(new ChangeDamageTypeTrigger(base.GameController, (DealDamageAction dd) => dd.DamageSource.IsInPlayAndHasGameText, new Func<DealDamageAction, IEnumerator>(base.SelectDamageTypeForDealDamageAction), new TriggerType[]
            {
                TriggerType.ChangeDamageType
            }, null, base.GetCardSource(null), null, false, true));
        }
    }
}