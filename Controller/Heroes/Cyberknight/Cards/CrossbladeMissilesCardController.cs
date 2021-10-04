using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
	public class CrossbladeMissilesCardController : CardController
	{
		//When Cyberknight would be dealt damage by a target other than herself, she may deal that target 2 fire X times, where X is the amount of damage she would take. If damage is dealt this way, destroy this card

		public CrossbladeMissilesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
				List<DealDamageAction> burned = new List<DealDamageAction>();
				int burnCount = dda.Amount;
				int X = 0;
				while (X < burnCount)
				{
					IEnumerator burnBack = base.DealDamage(base.CharacterCard, dda.DamageSource.Card, 2, DamageType.Fire, storedResults: burned, cardSource: base.GetCardSource(null));
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(burnBack);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(burnBack);
                    }
					X++;
				}
				if ((burned != null && burned.Count(dd => dd.Amount > 0) > 0))
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
		}
	}
}