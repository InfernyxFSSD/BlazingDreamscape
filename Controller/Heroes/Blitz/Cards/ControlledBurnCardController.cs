using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Blitz
{
	public class ControlledBurnCardController : CardController
	{
		public ControlledBurnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Fire, 1, false, 1, false, false, false, null, null, null, null, new Func<DealDamageAction, IEnumerator>(this.RemoveEndOfTurnResponse), false, null, null, false, null, base.GetCardSource(null));
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
		private IEnumerator RemoveEndOfTurnResponse(DealDamageAction dd)
		{
			int conduitCount = base.FindCardsWhere((Card c) => c.DoKeywordsContain("conduit", false, false) && c.IsInPlayAndHasGameText && c.Location.IsPlayAreaOf(this.TurnTaker)).Count<Card>();
			if (dd != null && dd.DidDealDamage && dd.Target != null && !dd.Target.IsCharacter && conduitCount > 0)
			{
				PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect(Phase.End);
				preventPhaseEffectStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
				preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = dd.Target;
				IEnumerator coroutine = base.AddStatusEffect(preventPhaseEffectStatusEffect, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
		}
	}
}
