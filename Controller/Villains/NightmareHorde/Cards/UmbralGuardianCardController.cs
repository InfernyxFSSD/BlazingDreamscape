using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class UmbralGuardianCardController : FragmentCardController
    {
        public UmbralGuardianCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //increase damage dealt by other villain targets
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsVillainTarget && dd.DamageSource.Card != this.Card, 1);
            //reduce damage dealt to other villain targets
            base.AddReduceDamageTrigger((Card c) => c.IsVillainTarget && c != base.Card, 1);
            //first time this card would be dealt damage each turn, prevent it
            base.AddPreventDamageTrigger((DealDamageAction dd) => dd.Target == base.Card && !base.HasBeenSetToTrueThisTurn(WouldBeDealtDamageThisTurn), new Func<DealDamageAction, IEnumerator>(this.PreventDamageResponse), new TriggerType[] { TriggerType.DealDamage }, isPreventEffect: true);
        }

        protected const string WouldBeDealtDamageThisTurn = "WouldBeDealtDamageThisTurn";

        private IEnumerator PreventDamageResponse(DealDamageAction dd)
        {
            base.SetCardPropertyToTrueIfRealAction(WouldBeDealtDamageThisTurn);
            return null;
        }
    }
}