using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class ReconfigureOutputCardController : KyossCardController
	{
		public ReconfigureOutputCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount <= 3, new Func<DealDamageAction, IEnumerator>(this.AddTokensResponse), new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.AddTokensToPool }, TriggerTiming.Before, null, false, true, null, false, null, null, false, false);
		}

		private IEnumerator AddTokensResponse(DealDamageAction dd)
		{
			TokenPool nukePool = base.TurnTaker.FindCard("EmpowermentOfFriendship").FindTokenPool("NukePool");
			int tokenGain = dd.Amount;
			YesNoDecision yesNo = new YesNoDecision(base.GameController, this.HeroTurnTakerController, SelectionType.PreventDamage, false, dd, null, base.GetCardSource(null));
			IEnumerator coroutine = base.GameController.MakeDecisionAction(yesNo, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (yesNo.Answer != null && yesNo.Answer.Value)
			{
				coroutine = base.CancelAction(dd, true, true, null, false);
				if(base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				coroutine = base.GameController.AddTokensToPool(nukePool, tokenGain, base.GetCardSource());
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

		public override IEnumerator Play()
		{
			if (base.Nuke.IsInDeck || base.Nuke.IsInTrash)
			{
				IEnumerator coroutine = base.GameController.PlayCard(base.TurnTakerController, base.Nuke, true, null, false, null, null, false, null, null, null, false, false, true, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			IEnumerator coroutine2 = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
		}
	}
}