using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class MicrocoatCardController : CardController
    {
        //Power: Discard up to {H - 1} cards. Select up to X heroes, where X is the number of cards discarded this way plus one. Reduce damage dealt to the selected heroes by 1 until the start of your next turn.

        public MicrocoatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = GetPowerNumeral(0, H - 1);
            int powerNumeral2 = GetPowerNumeral(1, 1);
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            //Discard up to H-1 cards
            IEnumerator discardCards = GameController.SelectAndDiscardCards(DecisionMaker, powerNumeral, false, 0, storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCards);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCards);
            }
            int discardCount = GetNumberOfCardsDiscarded(storedResults) + 1;
            List<Card> selectedTargets = new List<Card>();
            for (int i = 0; i < discardCount; i++)
            {
                //For each card discarded (plus 1), select a hero to reduce the damage they take
                List<SelectCardDecision> storedResults2 = new List<SelectCardDecision>();
                IEnumerator selectHero = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.IsHeroCharacterCard && !selectedTargets.Contains(c), "heroes in play", useCardsSuffix: false), storedResults2, optional: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(selectHero);
                }
                else
                {
                    GameController.ExhaustCoroutine(selectHero);
                }
                Card selectedCard = GetSelectedCard(storedResults2);
                if (selectedCard != null)
                {
                    selectedTargets.Add(selectedCard);
                    ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(powerNumeral2);
                    rdse.TargetCriteria.IsSpecificCard = selectedCard;
                    rdse.UntilStartOfNextTurn(TurnTaker);
                    rdse.UntilCardLeavesPlay(selectedCard);
                    IEnumerator applyStatus = AddStatusEffect(rdse);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(applyStatus);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(applyStatus);
                    }
                    continue;
                }
            }
            yield break;
        }
    }
}