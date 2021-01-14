using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class BladedGaleCardController : MicroManagerCardController
    {
        public BladedGaleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Sonic, 1, false, 1, false, false, false, null, storedResults, null, null, null, true, null, null, false, null, GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card target = storedResults.FirstOrDefault()?.SelectedCard;
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.VigilanteJusticeIdentifier) || base.GameController.IsCardInPlayAndNotUnderCard(base.ThunderStormIdentifier))
            {
                IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
                idse.TargetCriteria.IsSpecificCard = target;
                idse.NumberOfUses = 1;
                idse.UntilCardLeavesPlay(target);
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
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.VigilanteJusticeIdentifier) && base.GameController.IsCardInPlayAndNotUnderCard(base.ThunderStormIdentifier))
            {
                CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
                cddse.SourceCriteria.IsSpecificCard = target;
                cddse.NumberOfUses = 1;
                cddse.UntilCardLeavesPlay(target);
                coroutine = AddStatusEffect(cddse, true);
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