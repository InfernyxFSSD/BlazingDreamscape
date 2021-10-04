using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BlazingDreamscape.Cyberknight
{
    public class SnareClutchMountCardController : CardController
	{
		//When Cyberknight would be dealt damage by a target other than herself, you may reduce damage dealt to Cyberknight by that target by 2 and reidrect damage dealt by that target to Cyberknight until the start of your next turn. If you do, destroy this card.

		public SnareClutchMountCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target == base.CharacterCard && !dda.IsPretend && dda.Amount > 0 && dda.DamageSource.IsInPlayAndHasGameText && !base.IsBeingDestroyed && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget, new Func<DealDamageAction, IEnumerator>(this.MakeFragileResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.ReduceDamage }, TriggerTiming.Before);
		}

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{
			return new CustomDecisionText("Reduce damage dealt and redirect damage dealt by target?", "Reduce damage dealt and redirect damage dealt by target?", "Vote for whether damage dealt should be reduced and redirected", "reduce damage dealt and redirect damage?");
		}

        private IEnumerator MakeFragileResponse (DealDamageAction dda)
		{
			List<YesNoCardDecision> yesOrNo = new List<YesNoCardDecision>();
			IEnumerator makeDecision = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.Custom, base.Card, storedResults: yesOrNo, cardSource: base.GetCardSource(null));
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(makeDecision);
			}
			else
			{
				GameController.ExhaustCoroutine(makeDecision);
			}
			if (base.DidPlayerAnswerYes(yesOrNo))
            {
				ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
				rdse.SourceCriteria.IsSpecificCard = dda.DamageSource.Card;
				rdse.TargetCriteria.IsSpecificCard = base.CharacterCard;
				rdse.UntilStartOfNextTurn(base.TurnTaker);
				rdse.UntilCardLeavesPlay(dda.DamageSource.Card);
				rdse.UntilCardLeavesPlay(base.CharacterCard);
				RedirectDamageStatusEffect rdse2 = new RedirectDamageStatusEffect();
				rdse2.SourceCriteria.IsSpecificCard = dda.DamageSource.Card;
				rdse2.RedirectableTargets.IsSpecificCard = base.CharacterCard;
				rdse2.UntilStartOfNextTurn(base.TurnTaker);
				rdse2.UntilCardLeavesPlay(dda.DamageSource.Card);
				rdse2.UntilCardLeavesPlay(base.CharacterCard);
				IEnumerator reduceEffect = base.AddStatusEffect(rdse);
				IEnumerator redirectEffect = base.AddStatusEffect(rdse2);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(reduceEffect);
					yield return GameController.StartCoroutine(redirectEffect);
				}
				else
				{
					GameController.ExhaustCoroutine(reduceEffect);
					GameController.ExhaustCoroutine(redirectEffect);
				}
				IEnumerator breakSelf = base.GameController.DestroyCard(this.DecisionMaker, this.Card, cardSource: base.GetCardSource(null));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(breakSelf);
				}
				else
				{
					GameController.ExhaustCoroutine(breakSelf);
				}
			}
			yield break;
		}
	}
}