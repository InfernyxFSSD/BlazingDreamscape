using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class CounterShieldProjectorCardController : CardController
	{
		//When Cyberknight would be dealt damage by a target other than herself, she may reduce damage dealt to other hero targets by 2 until the start of your next turn. If you do so, break this card

		public CounterShieldProjectorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target == base.CharacterCard && !dda.IsPretend && dda.Amount > 0 && dda.DamageSource.IsInPlayAndHasGameText && !base.IsBeingDestroyed && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget, new Func<DealDamageAction, IEnumerator>(this.ProtectOthersResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.ReduceDamage}, TriggerTiming.Before);
		}

		private IEnumerator ProtectOthersResponse(DealDamageAction dda)
		{
			List<YesNoCardDecision> yesOrNo = new List<YesNoCardDecision>();
			IEnumerator makeDecision = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.ReduceDamageTaken, base.Card, storedResults: yesOrNo, cardSource: base.GetCardSource(null));
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
				rdse.TargetCriteria.IsHero = true;
				rdse.TargetCriteria.IsNotSpecificCard = base.CharacterCard;
				rdse.UntilStartOfNextTurn(base.TurnTaker);
				IEnumerator reduceEffect = base.AddStatusEffect(rdse);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(reduceEffect);
				}
				else
				{
					GameController.ExhaustCoroutine(reduceEffect);
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