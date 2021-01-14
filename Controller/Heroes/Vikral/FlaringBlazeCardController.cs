using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
    public class FlaringBlazeCardController : MicroManagerCardController
    {
        public FlaringBlazeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            int damageToDeal = 1;
            var isDamageIrreducible = false;
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.HailFlurryIdentifier))
            //if (base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "HailFlurry", true, null, false).Count<Card>() > 0)
            {
                damageToDeal = 2;
            }
            if (base.GameController.IsCardInPlayAndNotUnderCard(base.ToxicCloudIdentifier))
            //if (base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "ToxicCloud", true, null, false).Count<Card>() > 0)
            {
                isDamageIrreducible = true;
            }
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageToDeal, DamageType.Fire, 1, false, 1, isDamageIrreducible, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null));
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