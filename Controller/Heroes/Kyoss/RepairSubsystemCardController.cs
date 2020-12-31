using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
    public class RepairSubsystemCardController : CardController
    {
        public RepairSubsystemCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.GainHP
            }, null, false);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
        {
            var gainHPAmount = 1;
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, new int?(1), true, null, storedResults, false, null, null, null, SelectionType.DiscardCard, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.GetNumberOfCardsDiscarded(storedResults) == 1)
            {
                gainHPAmount = 2;
            }
            coroutine = base.GameController.GainHP(base.CharacterCard, gainHPAmount, null, null, base.GetCardSource(null));
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