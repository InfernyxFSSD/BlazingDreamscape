using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class RecyclingManaCardController : MicrostormCardController
    {
        public RecyclingManaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cep) => IsMicrostorm(cep.CardEnteringPlay), new Func<CardEntersPlayAction, IEnumerator>(this.MicrostormEntersPlayResponse), new TriggerType[] { TriggerType.AddTokensToPool }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
        }

        private IEnumerator MicrostormEntersPlayResponse(CardEntersPlayAction cep)
        {
            Card card = base.TurnTaker.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == cep.CardEnteringPlay.Identifier).FirstOrDefault();
            string torrentPoolIdentifier = $"{card.Identifier}TorrentPool";
            TokenPool torrentPool = card.FindTokenPool(torrentPoolIdentifier);
            IEnumerator coroutine = base.GameController.AddTokensToPool(torrentPool, 1, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator Play()
        {
            List<Card> selectedMicrostorms = new List<Card>();
            for (int i = 0; i < 2; i++)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.AddTokens, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsMicrostorm(c) && !selectedMicrostorms.Contains(c), "microstorms in play", false, false, null, null, false), storedResults, true, false, null, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                Card selectedCard = GetSelectedCard(storedResults);
                if (selectedCard != null)
                {
                    selectedMicrostorms.Add(selectedCard);
                    Card card = base.TurnTaker.GetCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == selectedCard.Identifier).FirstOrDefault();
                    string torrentPoolIdentifier = $"{card.Identifier}TorrentPool";
                    TokenPool torrentPool = card.FindTokenPool(torrentPoolIdentifier);
                    coroutine = base.GameController.AddTokensToPool(torrentPool, 1, GetCardSource(null));
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    yield break;
                }
            }
            IEnumerator coroutine2 = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c != base.Card && c.Identifier == base.Card.Identifier), false, null, null, null, SelectionType.DestroyCard, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }
    }
}