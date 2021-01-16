using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace SybithosInfernyx.Blitz
{
    public class ConflagrationRodCardController : FocusCardController
    {
        public ConflagrationRodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
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
            if (moveCard != null && moveCard.CardToMove != null && moveCard.CardToMove.IsOneShot)
            {
                List<PlayCardAction> playedCard = new List<PlayCardAction>();
                IEnumerator mayPlayCard = base.GameController.PlayCard(this.DecisionMaker, moveCard.CardToMove, true, null, true, null, null, false, null, playedCard, null, false, false, true, GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(mayPlayCard);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(mayPlayCard);
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