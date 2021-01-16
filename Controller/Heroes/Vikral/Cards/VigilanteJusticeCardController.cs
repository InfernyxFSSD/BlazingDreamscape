using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class VigilanteJusticeCardController : MicroManagerCardController
    {
        public VigilanteJusticeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            IEnumerator coroutine;
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.BladedGaleIdentifier))
            {
                ITrigger doNotAllowRedirect = AddMakeDamageNotRedirectableTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard));
                coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Radiant, 1, false, 1, false, false, false, null, storedResults, null, null, null, true, null, null, false, null, GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                RemoveTrigger(doNotAllowRedirect);
            }
            else
            {
                coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Radiant, 1, false, 1, false, false, false, null, storedResults, null, null, null, false, null, null, false, null, GetCardSource(null));
                if (UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            Card target = storedResults.FirstOrDefault()?.SelectedCard;
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.PsionicTorrentIdentifier) && target.IsInPlayAndHasGameText)
            {
                coroutine = base.GameController.DealDamageToSelf(base.HeroTurnTakerController, (Card c) => c == target, 1, DamageType.Radiant, false, null, false, null, null, GetCardSource(null));
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