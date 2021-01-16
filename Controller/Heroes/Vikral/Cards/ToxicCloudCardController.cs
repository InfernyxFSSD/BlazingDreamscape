using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class ToxicCloudCardController : MicroManagerCardController
    {
        public ToxicCloudCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && this.Card.IsInPlayAndHasGameText, (PhaseChangeAction p) => this.RemoveTokenFromMicrostorm(this.Card), new TriggerType[] { TriggerType.ModifyTokens }, null, false);
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => this.EndOfTurnResponse(), new TriggerType[] { TriggerType.DealDamage }, null, false);
            base.AddTrigger<RemoveTokensFromPoolAction>((RemoveTokensFromPoolAction rp) => rp.TokenPool.Identifier == $"{this.Card.Identifier}TorrentPool" && rp.TokenPool.CurrentValue <= 0 && rp.TokenPool.CardWithTokenPool.IsInPlay, base.DestroyThisCardResponse, new TriggerType[] { TriggerType.DestroySelf }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
        }

        private IEnumerator EndOfTurnResponse()
        {
            var applyStatusFunction = new Func<DealDamageAction, IEnumerator>(this.ApplyStatusEffects);
            if (!(base.GameController.IsCardInPlayAndNotUnderCard(base.OverloadPulseIdentifier) && base.GameController.IsCardInPlayAndNotUnderCard(base.VigilanteJusticeIdentifier)))
            {
                applyStatusFunction = null;
            }
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Toxic, 1, false, 1, false, false, false, null, null, null, null, applyStatusFunction, true, null, null, false, null, GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.OverloadPulseIdentifier) || base.GameController.IsCardInPlayAndNotUnderCard(base.VigilanteJusticeIdentifier))
            {
                coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsVillain && c.HitPoints <= 3 && c.IsTarget, "villain target with 3 or less HP", false, false, "villain target with 3 or less HP", "villain targets with 3 or less HP", false), true, null, null, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        private IEnumerator ApplyStatusEffects(DealDamageAction dd)
        {
            var target = dd.Target;
            OnDealDamageStatusEffect oddse = new OnDealDamageStatusEffect(this.CardWithoutReplacements, nameof(this.RedirectDamageResponse), $"You may redirect the next damage dealt by {target.Title}.", new TriggerType[] { TriggerType.RedirectDamage }, this.TurnTaker, base.Card, null);
            oddse.SourceCriteria.IsSpecificCard = target;
            oddse.NumberOfUses = 1;
            oddse.BeforeOrAfter = BeforeOrAfter.Before;
            oddse.UntilTargetLeavesPlay(target);
            IEnumerator coroutine = AddStatusEffect(oddse, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public IEnumerator RedirectDamageResponse(DealDamageAction dd, TurnTaker _, StatusEffect _2, int[] powerNumerals = null)
        {
            IEnumerator coroutine = base.GameController.SelectTargetAndRedirectDamage(base.HeroTurnTakerController, null, dd, true, null, base.GetCardSource(null));
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