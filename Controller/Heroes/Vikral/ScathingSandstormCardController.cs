using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class ScathingSandstormCardController : MicroManagerCardController
    {
        public ScathingSandstormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Projectile, 1, false, 1, false, false, false, null, storedResults, null, null, null, true, null, null, false, null, GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card target = storedResults.FirstOrDefault()?.SelectedCard;
            if ((base.GameController.IsCardInPlayAndNotUnderCard(base.PsionicTorrentIdentifier) || base.GameController.IsCardInPlayAndNotUnderCard(base.ToxicCloudIdentifier)) && target != null)
            {
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(1);
                rdse.SourceCriteria.IsSpecificCard = target;
                rdse.UntilStartOfNextTurn(this.TurnTaker);
                rdse.UntilCardLeavesPlay(target);
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
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.PsionicTorrentIdentifier) && base.GameController.IsCardInPlayAndNotUnderCard(base.ToxicCloudIdentifier))
            {
                PreventPhaseActionStatusEffect ppase = new PreventPhaseActionStatusEffect();
                ppase.ToTurnPhaseCriteria.Phase = new Phase?(Phase.PlayCard);
                ppase.ToTurnPhaseCriteria.TurnTaker = base.FindEnvironment(null).TurnTaker;
                ppase.UntilEndOfPhase(base.FindEnvironment(null).TurnTaker, Phase.PlayCard);
                coroutine = AddStatusEffect(ppase, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = base.GameController.SendMessageAction("Skip the next environment play phase!", Priority.High, GetCardSource(null), null, false);
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