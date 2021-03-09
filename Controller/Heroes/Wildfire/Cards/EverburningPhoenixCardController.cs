using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class EverburningPhoenixCardController : CardController
    {
        //At the end of your turn, this card deals a target 2 fire damage.
        //At the start of your turn, if this card has less than 5 HP, set its HP to 5.

        public EverburningPhoenixCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Hit a target at the end of your turn
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => true, TargetType.SelectTarget, 2, DamageType.Fire);
            //Heal to 5 if still alive at the start of your turn
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, new Func<PhaseChangeAction, IEnumerator>(SetHPResponse), new TriggerType[] { }); 
        }

        private IEnumerator SetHPResponse(PhaseChangeAction p)
        {
            int currentHP = Card.HitPoints.Value;
            //Check to see if current HP is less than 5
            if (currentHP < 5)
            {
                //If it is less than 5, set it to 5
                IEnumerator setHP = GameController.SetHP(Card, 5, GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(setHP);
                }
                else
                {
                    GameController.ExhaustCoroutine(setHP);
                }
            }
            yield break;
        }
    }
}