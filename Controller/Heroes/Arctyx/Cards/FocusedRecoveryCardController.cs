using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class FocusedRecoveryCardController : CardController
    {
        public FocusedRecoveryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(3), null, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator DiscardCardsAndDrawCards = base.GameController.SelectAndDiscardCards(base.HeroTurnTakerController, null, false, new int?(0), storedResults, false, null, null, null, null, SelectionType.DiscardCard, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(DiscardCardsAndDrawCards);
            }
            else
            {
                base.GameController.ExhaustCoroutine(DiscardCardsAndDrawCards);
            }
            int num = base.GetNumberOfCardsDiscarded(storedResults);
            if (num > 0)
            {
                coroutine = base.DrawCards(this.DecisionMaker, num, true, false, null, true, null);
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