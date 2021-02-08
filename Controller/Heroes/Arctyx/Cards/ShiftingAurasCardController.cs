using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class ShiftingAurasCardController : CardController
    {
        public ShiftingAurasCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.SelectAndPlayCardFromHand(this.DecisionMaker, true, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura", false, false), "aura", true, false, null, null, false), false, false, true, null), TriggerType.PlayCard, null, false);
            base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy.Card.DoKeywordsContain("flame", false, false), new Func<DestroyCardAction, IEnumerator>(this.DealFireDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
            base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy.Card.DoKeywordsContain("frost", false, false), new Func<DestroyCardAction, IEnumerator>(this.DealColdDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
        }

        public override IEnumerator Play()
        {
            IEnumerator playAura = base.SelectAndPlayCardFromHand(this.DecisionMaker, true, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura", false, false), "aura", true, false, null, null, false), false, false, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playAura);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playAura);
            }
            yield break;
        }

        private IEnumerator DealFireDamageResponse(DestroyCardAction d)
        {
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Fire, new int?(2), false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator DealColdDamageResponse(DestroyCardAction d)
        {
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Cold, new int?(1), false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

    }
}