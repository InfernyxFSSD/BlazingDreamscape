using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class ShockStallCardController : CardController
    {
        //Kyoss 2.1 deals each non-hero target 1 lightning damage. For each target dealt damage this way, one player may draw a card.

        public ShockStallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Kyoss hits each non-hero and gives a draw for each target dealt damage
            IEnumerator shockThemAll = DealDamage(CharacterCard, (Card c) => !c.IsHero, 1, DamageType.Lightning, addStatusEffect: new Func<DealDamageAction, IEnumerator>(SelectPlayerToDrawCard));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shockThemAll);
            }
            else
            {
                GameController.ExhaustCoroutine(shockThemAll);
            }
            yield break;
        }

        private IEnumerator SelectPlayerToDrawCard(DealDamageAction _)
        {
            //This is the part that gives the draws
            IEnumerator drawCard = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(drawCard);
            }
            else
            {
                GameController.ExhaustCoroutine(drawCard);
            }
            yield break;
        }
    }
}