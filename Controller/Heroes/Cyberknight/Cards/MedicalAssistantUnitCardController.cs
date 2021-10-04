using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class MedicalAssistantUnitCardController : CardController
	{
		//When Cyberknight would be dealt damage by a target other than herself, she may regain 5 HP. If she gains HP, each other hero target regains 2, then break this card

		public MedicalAssistantUnitCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target == base.CharacterCard && !dda.IsPretend && dda.Amount > 0 && dda.DamageSource.IsInPlayAndHasGameText && !base.IsBeingDestroyed && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget, new Func<DealDamageAction, IEnumerator>(this.AllTheHealingResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.GainHP }, TriggerTiming.Before);
		}

		private IEnumerator AllTheHealingResponse(DealDamageAction dda)
		{
			List<YesNoCardDecision> yesOrNo = new List<YesNoCardDecision>();
			IEnumerator makeDecision = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.GainHP, base.Card, storedResults: yesOrNo, cardSource: base.GetCardSource(null));
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
				List<GainHPAction> gainedHP = new List<GainHPAction>();
				int oldHP = base.CharacterCard.HitPoints.Value;
				IEnumerator healCyber = base.GameController.GainHP(base.CharacterCard, 5, storedResults: gainedHP, cardSource: base.GetCardSource(null));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(healCyber);
				}
				else
				{
					GameController.ExhaustCoroutine(healCyber);
				}
				if (base.CharacterCard.HitPoints.Value > oldHP)
                {
					IEnumerator healOthers = base.GameController.GainHP(this.DecisionMaker, (Card c) => c.IsHero && c.IsInPlayAndHasGameText && c != base.CharacterCard, 2, cardSource: base.GetCardSource(null));
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(healOthers);
					}
					else
					{
						GameController.ExhaustCoroutine(healOthers);
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
			}
			yield break;
		}
	}
}