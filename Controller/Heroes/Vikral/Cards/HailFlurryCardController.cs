using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class HailFlurryCardController : MicroManagerCardController
    {
        public HailFlurryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            List<SelectCardDecision> storedResults2 = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Cold, 1, false, 1, false, false, false, null, storedResults, null, null, null, true, null, null, false, null, GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card target = storedResults.FirstOrDefault()?.SelectedCard;
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.FlaringBlazeIdentifier))
            {
                coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Cold, 2, false, 2, false, false, false, ((Card c) => c != target), storedResults2, null, null, null, true, null, null, false, null, GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            var targets = storedResults2.ToArray();
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.BladedGaleIdentifier))
            {
                foreach (var sucker in targets)
                {
                    if(sucker != null)
                    {
                        ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                        rdse.SourceCriteria.IsSpecificCard = sucker.SelectedCard;
                        rdse.NumberOfUses = 1;
                        rdse.UntilCardLeavesPlay(sucker.SelectedCard);
                        coroutine = AddStatusEffect(rdse, true);
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
                ReduceDamageStatusEffect rdse2 = new ReduceDamageStatusEffect(1);
                rdse2.SourceCriteria.IsSpecificCard = target;
                rdse2.NumberOfUses = 1;
                rdse2.UntilCardLeavesPlay(target);
                coroutine = AddStatusEffect(rdse2, true);
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
}