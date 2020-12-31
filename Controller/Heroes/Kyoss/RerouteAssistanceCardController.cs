using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class RerouteAssistanceCardController : KyossCardController
	{
		public RerouteAssistanceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			base.AddTrigger<PhaseChangeAction>((PhaseChangeAction p) => p.ToPhase.IsEnd && (!this.DidHeroPlayCardDuringCardPhaseThisTurn(p.ToPhase.TurnTaker) || !this.DidHeroUsePowerDuringPowerPhaseThisTurn(p.ToPhase.TurnTaker)), new Func<PhaseChangeAction, IEnumerator>(this.SkippedPhaseToDrawResponse), new TriggerType[] { TriggerType.SkipPhase, TriggerType.DrawCard }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
		}

		private IEnumerator SkippedPhaseToDrawResponse(PhaseChangeAction p)
        {
			IEnumerator mayDrawCard = base.GameController.DrawCard(p.FromPhase.TurnTaker as HeroTurnTaker, true, null, true, null, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(mayDrawCard);
			}
			else
			{
				base.GameController.ExhaustCoroutine(mayDrawCard);
			}
			if (p.FromPhase.TurnTaker != this.HeroTurnTaker && p.FromPhase.TurnTaker.IsHero && base.Nuke.IsInPlayAndHasGameText)
			{
				TokenPool nukePool = base.TurnTaker.FindCard("EmpowermentOfFriendship").FindTokenPool("NukePool");
				IEnumerator coroutine = base.GameController.AddTokensToPool(nukePool, 1, GetCardSource());
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

		private bool DidHeroPlayCardDuringCardPhaseThisTurn(TurnTaker tt)
		{
			return tt.IsHero && base.Journal.PlayCardEntriesThisTurn().Any((PlayCardJournalEntry p) => p.CardPlayed.Owner == tt && p.TurnPhase.TurnTaker == tt && p.TurnPhase.IsPlayCard);
		}

		// Token: 0x06002B41 RID: 11073 RVA: 0x0006BFEC File Offset: 0x0006A1EC
		private bool DidHeroUsePowerDuringPowerPhaseThisTurn(TurnTaker tt)
		{
			return tt.IsHero && base.Journal.UsePowerEntriesThisTurn().Any((UsePowerJournalEntry p) => p.PowerUser == tt && p.TurnPhase.TurnTaker == tt && p.TurnPhase.IsUsePower);
		}
	}
}
