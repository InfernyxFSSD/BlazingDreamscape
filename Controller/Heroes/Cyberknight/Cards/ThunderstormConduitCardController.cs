using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class ThunderstormConduitCardController : CardController
	{
		//When Cyberknight would be dealt damage by a target other than herself, she may deal that target 4 lightning, then each other non-hero 2 lightning. If damage is dealt this way, destroy this card

		public ThunderstormConduitCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dda) => dda.Target == base.CharacterCard && !dda.IsPretend && dda.Amount > 0 && dda.DamageSource.IsInPlayAndHasGameText && !base.IsBeingDestroyed && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget, new Func<DealDamageAction, IEnumerator>(this.DealDamageResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.DealDamage }, TriggerTiming.Before);
		}

		private IEnumerator DealDamageResponse(DealDamageAction dda)
        {
			List<YesNoCardDecision> yesOrNo = new List<YesNoCardDecision>();
			IEnumerator makeDecision = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.DealDamage, base.Card, storedResults: yesOrNo, cardSource: base.GetCardSource(null));
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
				List<DealDamageAction> zapped = new List<DealDamageAction>();
				IEnumerator zapBack = base.DealDamage(base.CharacterCard, dda.DamageSource.Card, 4, DamageType.Lightning, storedResults: zapped, cardSource: base.GetCardSource(null));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(zapBack);
				}
				else
				{
					GameController.ExhaustCoroutine(zapBack);
				}
				List<DealDamageAction> moreZapped = new List<DealDamageAction>();
				IEnumerator moreZaps = base.GameController.DealDamage(this.DecisionMaker, base.CharacterCard, (Card c) => !c.IsHero && c.IsInPlayAndHasGameText && c != dda.DamageSource.Card, 2, DamageType.Lightning, storedResults: moreZapped, cardSource: base.GetCardSource(null));
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(moreZaps);
				}
				else
				{
					GameController.ExhaustCoroutine(moreZaps);
				}
				if ((zapped != null && zapped.Count(dd => dd.Amount > 0) > 0) || (moreZapped != null && moreZapped.Count(dd => dd.Amount > 0) > 0))
                {
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