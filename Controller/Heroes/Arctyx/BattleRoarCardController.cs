using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Arctyx
{
    public class BattleRoarCardController : CardController
    {
        public BattleRoarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            RedirectDamageStatusEffect redirectDamageStatusEffect = new RedirectDamageStatusEffect
            {
                RedirectTarget = base.CharacterCard
            };
            redirectDamageStatusEffect.SourceCriteria.IsVillain = true;
            redirectDamageStatusEffect.TargetCriteria.IsNotSpecificCard = base.CharacterCard;
            redirectDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            redirectDamageStatusEffect.TargetRemovedExpiryCriteria.Card = base.CharacterCard;
            IEnumerator coroutine = base.AddStatusEffect(redirectDamageStatusEffect, true);
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