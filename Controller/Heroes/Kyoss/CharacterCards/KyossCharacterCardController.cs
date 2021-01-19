using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class KyossCharacterCardController : HeroCharacterCardController
	{

		protected TokenPool NukePool
		{
			get
			{
				return base.CharacterCard.FindTokenPool("NukePool");
			}
		}

		public KyossCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						IEnumerator coroutine = base.SelectHeroToPlayCard(base.HeroTurnTakerController, false, true, false, null, null, null, false, false);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 1:
					{
						IEnumerator coroutine2 = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, false, true, false, null, null, null, true, false, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}
						break;
					}
				case 2:
					{
						base.AddToTemporaryTriggerList(base.AddTrigger<CancelAction>((CancelAction ca) => ca.ActionToCancel.CardSource != null && ca.ActionToCancel.CardSource.Card == base.CharacterCard, new Func<CancelAction, IEnumerator>(base.CancelResponse), TriggerType.CancelAction, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false));
						IEnumerator coroutine3 = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, false, true, false, null, null, null, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						base.RemoveTemporaryTriggers();
						break;
					}
			}
			yield break;
		}

		public override bool AskIfCardMayPreventAction<T>(TurnTakerController ttc, CardController preventer)
		{
			if (base.Card.IsIncapacitated)
			{
				return false;
			}
			return base.AskIfCardMayPreventAction<T>(ttc, preventer);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			IEnumerator coroutine = base.GameController.SelectAndGainHP(this.DecisionMaker, 2, false, (Card c) => c.IsTarget && c.IsHero && c.IsInPlay, 1, null, false, null, base.GetCardSource());
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