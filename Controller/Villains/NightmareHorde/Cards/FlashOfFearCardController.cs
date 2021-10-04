using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class FlashOfFearCardController : CardController
    {
        public FlashOfFearCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            Func<HeroTurnTakerController, IEnumerable<Function>> functions = (HeroTurnTakerController httc) => new Function[2]
            {
                new Function (httc, "Discard 2 cards", SelectionType.DiscardCard, () => this.SelectAndDiscardCards(httc, new int?(2), selectionType: SelectionType.DiscardCard), new bool?(httc.HeroTurnTaker.Hand.Cards.Count<Card>() >= 2)),
                new Function(httc, "Deal yourself 2 irreducible Psychic damage", SelectionType.DealDamageSelf, () => DealDamage(httc.CharacterCard, httc.CharacterCard, 2, DamageType.Psychic, isIrreducible: true), forcedActionMessage: httc.Name + " does not have at least 2 cards in their hand, and therefore must deal themself damage.")

            };
            IEnumerator discardOrDestroy = EachPlayerSelectsFunction((HeroTurnTakerController httc) => !httc.IsIncapacitatedOrOutOfGame, functions);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardOrDestroy);
            }
            else
            {
                GameController.ExhaustCoroutine(discardOrDestroy);
            }
            yield break;
        }
    }
}