using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class IronTitanCardController : FragmentCardController
    {
        public IronTitanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        //end of villain turn, hit lowest hero target and self for 2 energy

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
        {
            List<Card> lowestHP = new List<Card>();
            //find the hero target with the lowest HP, so we know who to hit
            IEnumerator findVictimOne = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHero, lowestHP, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findVictimOne);
            }
            else
            {
                GameController.ExhaustCoroutine(findVictimOne);
            }
            var victimOne = lowestHP.FirstOrDefault();
            //hit the victim first, then hit itself
            IEnumerator bapVictimOne = base.DealDamage(this.Card, victimOne, 2, DamageType.Energy);
            IEnumerator bapVictimTwo = base.DealDamage(this.Card, this.Card, 2, DamageType.Energy);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(bapVictimOne);
                yield return GameController.StartCoroutine(bapVictimTwo);
            }
            else
            {
                GameController.ExhaustCoroutine(bapVictimOne);
                GameController.ExhaustCoroutine(bapVictimTwo);
            }
        }
    }
}