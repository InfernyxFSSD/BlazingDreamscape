using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class RecycleCardController : CardController
    {
        //Each player may discard a card. Any player who does may put a card from their trash into their hand.

        public RecycleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            List<DiscardCardAction> discardedCards = new List<DiscardCardAction>();
            //Each player may discard a card
            IEnumerator discardCards = GameController.EachPlayerDiscardsCards(0, new int?(1), discardedCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCards);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCards);
            }
            foreach(DiscardCardAction dca in discardedCards.Where(dca => dca.WasCardDiscarded))
            {
                //When they discard, they can get a card from their trash into their hand
                IEnumerator moveCard = GameController.SelectAndMoveCard(dca.HeroTurnTakerController, (Card c) => c.Location == dca.HeroTurnTakerController.TurnTaker.Trash, dca.HeroTurnTakerController.HeroTurnTaker.Hand, cardSource: GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(moveCard);
                }
                else
                {
                    GameController.ExhaustCoroutine(moveCard);
                }
            }
            yield break;
        }
    }
}