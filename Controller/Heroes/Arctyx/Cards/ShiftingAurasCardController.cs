using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    //When this card enters play and at the start of your turn, you may play an Aura card.
    //Whenever a Flame card is destroyed, Arctyx may deal up to 2 targets 1 Fire damage each.
    //Whenever a Frost card is destroyed, Arctyx may deal a target 2 Cold damage.

    public class ShiftingAurasCardController : CardController
    {
        public ShiftingAurasCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Start of your turn, can play an aura
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => SelectAndPlayCardFromHand(DecisionMaker, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura"), "aura")), TriggerType.PlayCard);
            //When a flame is destroyed, burn things
            AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy.Card.DoKeywordsContain("flame"), new Func<DestroyCardAction, IEnumerator>(DealFireDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
            //When a frost is destroyed, freeze things
            AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy.Card.DoKeywordsContain("frost"), new Func<DestroyCardAction, IEnumerator>(DealColdDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, you may play an aura
            IEnumerator playAura = SelectAndPlayCardFromHand(DecisionMaker, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura"), "aura"));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playAura);
            }
            else
            {
                GameController.ExhaustCoroutine(playAura);
            }
            yield break;
        }

        private IEnumerator DealFireDamageResponse(DestroyCardAction d)
        {
            IEnumerator burnThings = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Fire, new int?(2), false, new int?(0), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(burnThings);
            }
            else
            {
                GameController.ExhaustCoroutine(burnThings);
            }
            yield break;
        }

        private IEnumerator DealColdDamageResponse(DestroyCardAction d)
        {
            IEnumerator freezeThing = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Cold, new int?(1), false, new int?(0), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(freezeThing);
            }
            else
            {
                GameController.ExhaustCoroutine(freezeThing);
            }
            yield break;
        }

    }
}