using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class BalanceAmplifierCardController : CardController
	{
		public BalanceAmplifierCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), new TriggerType[] { TriggerType.IncreaseDamage, TriggerType.ReduceDamage }, null, false);
		}

		private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
		{
			var storedYesNo = new List<YesNoCardDecision>();
			IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.IncreaseNextDamage, this.Card, p, storedYesNo, null, base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidPlayerAnswerYes(storedYesNo))
			{
				List<Card> selectedTargets = new List<Card>();
				int X = 0;
				while (X < 2)
				{
					List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
					IEnumerator chooseHeroTarget = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !selectedTargets.Contains(c) && c != base.HeroTurnTaker.CharacterCard, "hero targets in play", false, false, null, null, false), storedResults, true, false, null, true, base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(chooseHeroTarget);
					}
					else
					{
						base.GameController.ExhaustCoroutine(chooseHeroTarget);
					}
					Card selectedCard = base.GetSelectedCard(storedResults);
					if (selectedCard == null)
					{
						break;
					}
					selectedTargets.Add(selectedCard);
					IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
					increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = selectedCard;
					increaseDamageStatusEffect.UntilCardLeavesPlay(selectedCard);
					increaseDamageStatusEffect.NumberOfUses = 1;
					coroutine = base.AddStatusEffect(increaseDamageStatusEffect, true);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
					storedResults = null;
					X++;
				}
				if (selectedTargets != null)
                {
					ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
					reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = this.HeroTurnTaker.CharacterCard;
					reduceDamageStatusEffect.UntilCardLeavesPlay(this.HeroTurnTaker.CharacterCard);
					reduceDamageStatusEffect.NumberOfUses = 1;
					coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
			}
			yield break;
		}
	}
}