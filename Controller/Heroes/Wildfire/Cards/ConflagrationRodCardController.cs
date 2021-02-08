using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace BlazingDreamscape.Wildfire
{
    public class ConflagrationRodCardController : CardController
    {
        public ConflagrationRodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(this.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), null, false);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator dealFire;
            int powerNumeral = base.GetPowerNumeral(0, 1);
            List<MoveCardAction> storedResults = new List<MoveCardAction>();
            IEnumerator discardTopCard = base.GameController.DiscardTopCard(this.TurnTaker.Deck, storedResults, null, base.TurnTaker, GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardTopCard);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardTopCard);
            }
            MoveCardAction moveCard = storedResults.FirstOrDefault<MoveCardAction>();
            if (moveCard != null && moveCard.CardToMove != null && moveCard.CardToMove.DoKeywordsContain("elemental", false, false))
            {
                List<PlayCardAction> playedCard = new List<PlayCardAction>();
                IEnumerator maybePlayCard = base.GameController.PlayCard(this.DecisionMaker, moveCard.CardToMove, true, null, false, null, null, false, null, playedCard, null, false, false, true, GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(maybePlayCard);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(maybePlayCard);
                }
                PlayCardAction playCard = playedCard.FirstOrDefault<PlayCardAction>();
                if (playCard == null)
                {
                    dealFire = base.GameController.DealDamage(this.DecisionMaker, base.CharacterCard, (Card c) => !c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, powerNumeral, DamageType.Fire, false, false, null, null, null, false, null, null, false, false, GetCardSource(null));
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(dealFire);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(dealFire);
                    }
                }
            }
            else
            {
                dealFire = base.GameController.DealDamage(this.DecisionMaker, base.CharacterCard, (Card c) => !c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText, powerNumeral, DamageType.Fire, false, false, null, null, null, false, null, null, false, false, GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealFire);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealFire);
                }
            }
            yield break;
        }
    }
}