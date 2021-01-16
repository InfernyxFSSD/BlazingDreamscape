using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Blitz
{
	public class ElementalWrathCardController : CardController
	{
		public ElementalWrathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && c.Location.IsPlayAreaOf(this.TurnTaker) && !c.IsCharacter && c.DoKeywordsContain("elemental", false, false)));
			IEnumerator selectTarget = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetNoDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && c.Location.IsPlayAreaOf(this.TurnTaker) && !c.IsCharacter), storedResults, false, false, null, true, GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(selectTarget);
			}
			else
			{
				base.GameController.ExhaustCoroutine(selectTarget);
			}
			if (DidSelectCard(storedResults))
			{
				Card card = base.GetSelectedCard(storedResults);
				int X = card.HitPoints.Value;
				IEnumerator burn;
				int conduitCount = base.FindCardsWhere((Card c) => c.DoKeywordsContain("conduit", false, false) && c.IsInPlayAndHasGameText).Count<Card>();
				if (conduitCount > 3)
				{
					burn = base.GameController.DealDamage(this.DecisionMaker, card, (Card c) => c.IsVillainTarget && c.IsInPlayAndHasGameText, X, DamageType.Fire, false, false, null, null, null, false, null, null, false, false, GetCardSource(null));
				}
				else
				{
					burn = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, card), X, DamageType.Fire, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, GetCardSource(null));
				}
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(burn);
				}
				else
				{
					base.GameController.ExhaustCoroutine(burn);
				}
			}
			yield break;
		}
	}
}