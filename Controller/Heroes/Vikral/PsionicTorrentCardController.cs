using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class PsionicTorrentCardController : MicroManagerCardController
    {
        public PsionicTorrentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            if (!base.GameController.IsCardInPlayAndNotUnderCard(base.ViciousRetributionIdentifier))
            {
                applyStatusFunction = null;
            }
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Psychic, 1, false, 1, false, false, false, null, null, null, null, applyStatusFunction, true, null, null, false, null, GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.FlaringBlazeIdentifier))
            {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                idse.SourceCriteria.IsSpecificCard = base.CharacterCard;
                idse.NumberOfUses = 1;
                idse.UntilCardLeavesPlay(base.CharacterCard);
                coroutine = AddStatusEffect(idse, true);
                if (UseUnityCoroutines)
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
            OnDealDamageStatusEffect oddse = new OnDealDamageStatusEffect(this.CardWithoutReplacements, nameof(DealDamageToSelfResponse), $"Until the end of your next turn, when {target.Title} deals another target damage, {target.Title} deals itself 1 psychic damage.", new TriggerType[] { TriggerType.DealDamage }, base.TurnTaker, this.Card);
            oddse.UntilTargetLeavesPlay(target);
            oddse.SourceCriteria.IsSpecificCard = target;
            oddse.TargetCriteria.IsNotSpecificCard = target;
            oddse.UntilEndOfNextTurn(base.TurnTaker);
            oddse.BeforeOrAfter = BeforeOrAfter.After;
            oddse.DoesDealDamage = true;
            oddse.DamageAmountCriteria.GreaterThan = 0;
            IEnumerator coroutine = AddStatusEffect(oddse, true);
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public IEnumerator DealDamageToSelfResponse(DealDamageAction dd, TurnTaker _, StatusEffect effect, int[] powerNumerals = null)
        {
            //var target = dd.DamageSource.Card;
            var target = effect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();
            IEnumerator coroutine = this.DealDamage(target, target, 1, DamageType.Psychic, false, false, false, null, null, null, false, GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}