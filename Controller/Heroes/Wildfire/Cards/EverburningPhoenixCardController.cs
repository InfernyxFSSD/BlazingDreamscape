using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class EverburningPhoenixCardController : CardController
    {
        public EverburningPhoenixCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => true, TargetType.SelectTarget, 2, DamageType.Fire, false, false, 1, 1, null, null);
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.SetHPResponse), new TriggerType[] { }, null, false); 
        }

        private IEnumerator SetHPResponse(PhaseChangeAction p)
        {
            int X = base.Card.HitPoints.Value;
            int Y = base.Card.MaximumHitPoints.Value;
            if (X < Y)
            {
                IEnumerator setHP = base.GameController.SetHP(base.Card, Y, GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(setHP);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(setHP);
                }
            }
            yield break;
        }
    }
}