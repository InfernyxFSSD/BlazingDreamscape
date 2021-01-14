using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class ViciousRetributionCardController : MicroManagerCardController
    {
        public ViciousRetributionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            int timesDamaged = this.GetNumberOfTimesDamagedSinceLastTurn();
            int damageToDeal = 1;
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.ThunderStormIdentifier))
            {
                damageToDeal = timesDamaged + 1;
            }
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageToDeal, DamageType.Infernal, 1, false, 1, false, false, false, null, storedResults, null, null, null, false, null, null, false, null, GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card target = storedResults.FirstOrDefault()?.SelectedCard;
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.HailFlurryIdentifier) && target != null)
            {
                coroutine = base.DealDamage(base.CharacterCard, target, damageToDeal, DamageType.Infernal, false, false, false, null, null, null, false, null);
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

        private int GetNumberOfTimesDamagedSinceLastTurn()
        {
            int result = 0;
            try
            {
                result = (from e in base.GameController.Game.Journal.DealDamageEntriesToTargetSinceLastTurn(base.CharacterCard, base.HeroTurnTakerController.TurnTaker)
                select e).Count();
            }
            catch (OverflowException ex)
            {
                Log.Warning("GetNumberOfTimesDamagedSinceLastTurn overflowerd: " + ex.Message);
                result = int.MaxValue;
            }
            return result;
        }
    }
}