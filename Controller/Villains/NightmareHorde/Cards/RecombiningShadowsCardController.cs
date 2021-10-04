using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class RecombiningShadowsCardController : CardController
    {
        public RecombiningShadowsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator shuffleTrash = base.GameController.ShuffleTrashIntoDeck(this.TurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shuffleTrash);
            }
            else
            {
                GameController.ExhaustCoroutine(shuffleTrash);
            }
            int frags = base.FindCardsWhere((Card c) => c.DoKeywordsContain("fragment") && c.IsInPlayAndHasGameText).Count<Card>();
            if (frags < base.H)
            {
                IEnumerator spawnFragments = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("fragment")), new int?(1));
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(spawnFragments);
                }
                else
                {
                    GameController.ExhaustCoroutine(spawnFragments);
                }
            }
            IEnumerator healNightmares = base.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsVillain, 2, cardSource:GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(healNightmares);
            }
            else
            {
                GameController.ExhaustCoroutine(healNightmares);
            }
            yield break;
        }
    }
}