using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class AidGuidanceProtocolsCardController : CardController
    {
        //At the end of your turn, reveal the top card of two decks. Replace one and put the other on the bottom of its deck.
        //Power: One player may draw two cards, then they may play a card. Destroy this card.

        public AidGuidanceProtocolsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            //End of your turn, reveal the top card of two decks, then replace one and put the other on the bottom of its deck
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, new Func<PhaseChangeAction, IEnumerator>(EndOfTurnResponse), new TriggerType[] { TriggerType.RevealCard });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
        {
            //At the end of your turn, reveal the top card of two decks. Replace one and put the other on the bottom of its deck.
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            //First, select the two decks you want to reveal the top cards of
            IEnumerator selectDecks = SelectDecks(DecisionMaker, 2, SelectionType.RevealCardsFromDeck, (Location l) => true, storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectDecks);
            }
            else
            {
                GameController.ExhaustCoroutine(selectDecks);
            }
            IEnumerable<Location> decks = from l in storedResults
                                          where l.Completed && l.SelectedLocation.Location != null
                                          select l.SelectedLocation.Location;
            List<Card> storedCards = new List<Card>();
            int num;
            for (int i = 0; i < decks.Count<Location>(); i = num + 1)
            {
                //Reveal the top card of both chosen decks
                IEnumerator revealCards = GameController.RevealCards(TurnTakerController, decks.ElementAt(i), 1, storedCards, revealedCardDisplay: RevealedCardDisplay.Message, cardSource: GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(revealCards);
                }
                else
                {
                    GameController.ExhaustCoroutine(revealCards);
                }
                num = i;
            }
            //Select which card you want to replace on top of its deck
            List<SelectCardDecision> storedTop = new List<SelectCardDecision>();
            IEnumerator selectTopCard = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.MoveCardOnDeck, storedCards, storedTop, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectTopCard);
            }
            else
            {
                GameController.ExhaustCoroutine(selectTopCard);
            }
            Card topCard = GetSelectedCard(storedTop);
            //Put that card on top of its deck
            IEnumerator moveCardToTop = GameController.MoveCard(TurnTakerController, topCard, topCard.NativeDeck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(moveCardToTop);
            }
            else
            {
                GameController.ExhaustCoroutine(moveCardToTop);
            }
            Card otherCard = storedCards.Where((Card c) => c != topCard).FirstOrDefault();
            if (otherCard != null)
            {
                //Put the other card on the bottom of its deck
                IEnumerator moveCardToBottom = GameController.MoveCard(TurnTakerController, otherCard, otherCard.NativeDeck, toBottom: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(moveCardToBottom);
                }
                else
                {
                    GameController.ExhaustCoroutine(moveCardToBottom);
                }
                storedCards.Remove(otherCard);
            }
            for (int i = 0; i < decks.Count();)
            {
                //Clean up any other revealed cards
                List<Location> list = new List<Location>();
                list.Add(decks.ElementAt(i).OwnerTurnTaker.Revealed);
                IEnumerator cleanReveals = CleanupCardsAtLocations(list, decks.ElementAt(i), cardsInList: storedCards);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(cleanReveals);
                }
                else
                {
                    GameController.ExhaustCoroutine(cleanReveals);
                }
                break;
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //One player may draw two cards, then they may play a card. Destroy this card.
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator drawCards = GameController.SelectHeroToDrawCards(DecisionMaker, 2, storedResults: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(drawCards);
            }
            else
            {
                GameController.ExhaustCoroutine(drawCards);
            }
            HeroTurnTakerController httc = FindHeroTurnTakerController(GetSelectedTurnTaker(storedResults).ToHero());
            IEnumerator playCard = SelectAndPlayCardFromHand(httc);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playCard);
            }
            else
            {
                GameController.ExhaustCoroutine(playCard);
            }
            //Destroy this card
            IEnumerator destroyThisCard = GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyThisCard);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyThisCard);
            }
            yield break;
        }
    }
}