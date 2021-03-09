using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class NanobotResourceCacheCardController : CardController
    {
        //Reveal cards from the top of your deck until two equipment cards are revealed. Put one into play and the other in your hand. Shuffle the other revealed cards back into the deck.

        public NanobotResourceCacheCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many Equipments are in your deck?
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => IsEquipment(c)));

        }
        public override IEnumerator Play()
        {
            List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
            //Reveal cards until you get to 2 equipment
            IEnumerator revealCards = GameController.RevealCards(DecisionMaker, TurnTaker.Deck, (Card c) => IsEquipment(c), 2, revealedCards, RevealedCardDisplay.ShowMatchingCards, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(revealCards);
            }
            else
            {
                GameController.ExhaustCoroutine(revealCards);
            }
            //Sort the revealed cards into equips and non-equips
            List<Card> equipCards = GetRevealedCards(revealedCards).Where(c => IsEquipment(c)).Take(2).ToList();
            List<Card> nonEquipCards = GetRevealedCards(revealedCards).Where(c => !IsEquipment(c)).ToList();
            if(equipCards.Any())
            {
                //If any equips are revealed, select one to put into play
                IEnumerator putIntoPlay = GameController.SelectAndPlayCard(DecisionMaker, equipCards, isPutIntoPlay: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(putIntoPlay);
                }
                else
                {
                    GameController.ExhaustCoroutine(putIntoPlay);
                }
                Card otherEquip = equipCards.FirstOrDefault(c => c.Location.IsRevealed);
                if (otherEquip != null)
                {
                    //If there's another revealed equip, put it in your hand
                    IEnumerator putIntoHand = GameController.MoveCard(DecisionMaker, otherEquip, FindCardController(otherEquip).HeroTurnTaker.Hand, showMessage: true, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(putIntoHand);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(putIntoHand);
                    }
                }
            }
            if(nonEquipCards.Any())
            {
                //Shuffle the non-equips back into the deck
                IEnumerator moveToDeck = GameController.MoveCards(DecisionMaker, nonEquipCards, TurnTaker.Deck, cardSource: GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(moveToDeck);
                }
                else
                {
                    GameController.ExhaustCoroutine(moveToDeck);
                }
                IEnumerator shuffleDeck = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(shuffleDeck);
                }
                else
                {
                    GameController.ExhaustCoroutine(shuffleDeck);
                }
            }
            yield break;
        }
    }
}