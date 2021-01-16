using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace SybithosInfernyx.Blitz
{
    public class BonfireCircletCardController : FocusCardController
    {
        public BonfireCircletCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
		{
			List<SelectNumberDecision> decision = new List<SelectNumberDecision>();
			IEnumerator coroutine = base.GameController.SelectNumber(base.HeroTurnTakerController, SelectionType.DealDamage, 1, base.GetPowerNumeral(0, 3), false, false, null, decision, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			int value = decision.First((SelectNumberDecision d) => d.Completed).SelectedNumber.Value;
			IEnumerator coroutine2 = base.DealDamage(base.CharacterCard, base.CharacterCard, value, DamageType.Fire, false, false, false, null, null, null, false, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			int amountOfFireDamageDealtToBlitzThisTurn = this.GetAmountOfFireDamageDealtToBlitzThisTurn();
			IEnumerator coroutine3 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), amountOfFireDamageDealtToBlitzThisTurn, DamageType.Fire, 1, false, 1, false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine3);
			}
			yield break;
		}

		private int GetAmountOfFireDamageDealtToBlitzThisTurn()
		{
			return (from e in base.GameController.Game.Journal.DealDamageEntriesThisTurn()
					where e.TargetCard == base.CharacterCard && e.DamageType == DamageType.Fire
					select e.Amount).Sum();
		}
	}
}
