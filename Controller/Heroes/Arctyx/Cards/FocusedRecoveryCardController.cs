using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
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
            bool func(Card card) => card.DoKeywordsContain("aura", false, false);
            if (base.HeroTurnTakerController.HeroTurnTaker.Hand.Cards.Where(func).Count<Card>() > 0)
            {
                List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
                IEnumerator DiscardAuraCardsAndDrawCards = base.GameController.SelectAndDiscardCards(base.HeroTurnTakerController, null, true, new int?(0), storedResults, false, null, null, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura", false, false), "aura", true, false, null, null, false), SelectionType.DiscardCard, null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(DiscardAuraCardsAndDrawCards);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(DiscardAuraCardsAndDrawCards);
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
            }
            yield break;
        }
    }
}