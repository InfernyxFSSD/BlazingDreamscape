using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace BlazingDreamscape.Wildfire
{
    public class ConflagrationRodCardController : CardController
    {
        //Power: Discard the top card of your deck. If it is an elemental, put it into play. If no cards entered play this way, wildfire deals each non-hero target 1 fire damage.

        public ConflagrationRodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many elementals are in your deck?
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental"), "elemental"));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator dealFire;
            int powerNumeral = GetPowerNumeral(0, 1);
            List<MoveCardAction> storedResults = new List<MoveCardAction>();
            //Discard the top card of your deck
            IEnumerator discardTopCard = GameController.DiscardTopCard(TurnTaker.Deck, storedResults, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardTopCard);
            }
            else
            {
                GameController.ExhaustCoroutine(discardTopCard);
            }
            MoveCardAction moveCard = storedResults.FirstOrDefault<MoveCardAction>();
            if (moveCard != null && moveCard.CardToMove != null && moveCard.CardToMove.DoKeywordsContain("elemental"))
            {
                //If it was an elemental, put it into play
                List<PlayCardAction> playedCard = new List<PlayCardAction>();
                IEnumerator playCard = GameController.PlayCard(DecisionMaker, moveCard.CardToMove, true, storedResults: playedCard, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(playCard);
                }
                else
                {
                    GameController.ExhaustCoroutine(playCard);
                }
                //Do I actually need this section? It only plays it if it's an elemental anyways
                /*PlayCardAction playCard = playedCard.FirstOrDefault<PlayCardAction>();
                if (playCard == null)
                {
                    dealFire = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, powerNumeral, DamageType.Fire, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(dealFire);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(dealFire);
                    }
                }*/
            }
            else
            {
                //If no cards entered play this way, Wildfire deals fire
                dealFire = GameController.DealDamage(DecisionMaker, CharacterCard, (Card c) => !c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, powerNumeral, DamageType.Fire, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(dealFire);
                }
                else
                {
                    GameController.ExhaustCoroutine(dealFire);
                }
            }
            yield break;
        }
    }
}