using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class EmpowermentOfFriendshipCardController : KyossCardController
	{
		public EmpowermentOfFriendshipCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowTokenPool(base.Card.FindTokenPool("NukePool"), null, null).Condition = (() => base.Card.IsInPlayAndNotUnderCard && !base.Card.IsFlipped);
		}

		public override void AddTriggers()
		{
			List<ITrigger> list = new List<ITrigger>();
			TokenPool pool = base.Card.FindTokenPool("NukePool");
			list.Add(AddTrigger((PlayCardAction pc) => pc.WasCardPlayed && !pc.IsPutIntoPlay && pc.Origin.OwnerTurnTaker != base.Card.Location.OwnerTurnTaker && pc.Origin.OwnerTurnTaker.IsHero && (pc.ResponsibleTurnTaker == base.Card.Location.OwnerTurnTaker || (pc.CardSource != null && pc.CardSource.Card.Owner == base.Card.Location.OwnerTurnTaker) || (pc.ActionSource != null && pc.ActionSource.DecisionMaker == FindHeroTurnTakerController(base.Card.Location.OwnerTurnTaker.ToHero()))), (PlayCardAction pc) => base.GameController.AddTokensToPool(pool, 1, GetCardSource()), TriggerType.AddTokensToPool, TriggerTiming.After));
			list.Add(AddTrigger((UsePowerAction p) => p.CardSource != null && p.Power.TurnTakerController != FindTurnTakerController(base.Card.Location.OwnerTurnTaker) && p.CardSource.Card.Owner == base.Card.Location.OwnerTurnTaker, (UsePowerAction p) => base.GameController.AddTokensToPool(pool, 1, GetCardSource()), TriggerType.AddTokensToPool, TriggerTiming.After));
			list.Add(AddTrigger((DrawCardAction dc) => dc.DidDrawCard && dc.HeroTurnTaker != base.Card.Location.OwnerTurnTaker && dc.CardSource != null && dc.CardSource.Card.Owner == base.Card.Location.OwnerTurnTaker, (DrawCardAction dc) => base.GameController.AddTokensToPool(pool, 1, GetCardSource()), TriggerType.AddTokensToPool, TriggerTiming.After));
			list.Add(AddTrigger((PlayCardAction pc) => base.GameController.ActiveTurnPhase.IsPlayCard && pc.Origin.OwnerTurnTaker != base.Card.Location.OwnerTurnTaker && pc.Origin.OwnerTurnTaker == base.GameController.ActiveTurnTakerController.TurnTaker && base.GameController.ActiveTurnPhase.PhaseActionCountUsed > 0 && base.GameController.ActiveTurnPhase.PhaseActionCountUsed <= (from cc in base.GameController.GetCardsIncreasingPhaseActionCount()
																																																																																															  where cc.Card.Owner == base.Card.Location.OwnerTurnTaker
																																																																																															  select cc).Count(), (PlayCardAction p) => base.GameController.AddTokensToPool(pool, 1, GetCardSource()), TriggerType.AddTokensToPool, TriggerTiming.After));
			list.Add(AddTrigger((UsePowerAction p) => base.GameController.ActiveTurnPhase.IsUsePower && p.Power.TurnTakerController != FindTurnTakerController(base.Card.Location.OwnerTurnTaker) && p.Power.TurnTakerController == base.GameController.ActiveTurnTakerController && base.GameController.ActiveTurnPhase.PhaseActionCountUsed > 0 && base.GameController.ActiveTurnPhase.PhaseActionCountUsed <= (from cc in base.GameController.GetCardsIncreasingPhaseActionCount()
																																																																																																				  where cc.Card.Owner == base.Card.Location.OwnerTurnTaker
																																																																																																				  select cc).Count(), (UsePowerAction p) => base.GameController.AddTokensToPool(pool, 1, GetCardSource()), TriggerType.AddTokensToPool, TriggerTiming.After));
			list.Add(AddTrigger((DrawCardAction dc) => base.GameController.ActiveTurnPhase.IsDrawCard && dc.HeroTurnTaker != base.Card.Location.OwnerTurnTaker && base.GameController.ActiveTurnPhase.PhaseActionCountUsed > 0 && base.GameController.ActiveTurnPhase.PhaseActionCountUsed <= (from cc in base.GameController.GetCardsIncreasingPhaseActionCount()
																																																																							   where cc.Card.Owner == base.Card.Location.OwnerTurnTaker
																																																																							   select cc).Count(), (DrawCardAction p) => base.GameController.AddTokensToPool(pool, 1, GetCardSource()), TriggerType.AddTokensToPool, TriggerTiming.After));
			base.AddWhenDestroyedTrigger((DestroyCardAction dc) => this.ResetTokenValue(), TriggerType.Hidden);
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine = this.ResetTokenValue();
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

		public IEnumerator ResetTokenValue()
		{
			base.Card.FindTokenPool("NukePool").SetToInitialValue();
			yield return null;
			yield break;
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//{Kyoss} deals a target X irreducible Energy damage, where X equals the number of tokens on this card. Then, remove this card from the game.
			TokenPool nukePool = base.Card.FindTokenPool("NukePool");
			int? X(Card c) => new int?(nukePool.CurrentValue);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), X, DamageType.Energy, () => 1, false, new int?(1), true, false, null, null, null, null, null, false, null, null, true, false, base.GetCardSource(null));
			//IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), nukePool.CurrentValue, DamageType.Energy, 1, false, 1, true, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			IEnumerator coroutine2 = base.GameController.MoveCard(base.TurnTakerController, base.Card, base.TurnTaker.OutOfGame, false, false, true, null, false, null, null, null, false, false, null, false, false, false, false, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
		}
	}
}
