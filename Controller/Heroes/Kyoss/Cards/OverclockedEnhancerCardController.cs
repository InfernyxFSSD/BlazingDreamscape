using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class OverclockedEnhancerCardController : CardController
	{
		public OverclockedEnhancerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{
			List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
			IEnumerator coroutine = base.GameController.SelectLocation(this.DecisionMaker, new LocationChoice[]
			{
				new LocationChoice(base.TurnTaker.Deck, null, false),
				new LocationChoice(base.TurnTaker.Trash, null, false)
			}, SelectionType.SearchLocation, storedResults, false, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (base.DidSelectLocation(storedResults))
			{
				Location selectedLocation = base.GetSelectedLocation(storedResults);
				coroutine = base.PlayCardFromLocation(selectedLocation, "EmpowermentOfFriendship", true, null, false, true, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			int X = 0;
			int heroMinusOne = base.H - 1;
			while (X < heroMinusOne)
			{
				List<DealDamageAction> storedResults2 = new List<DealDamageAction>();
				IEnumerator dealSelfDamage = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Lightning, false, true, false, null, storedResults2, null, false, null);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(dealSelfDamage);
				}
				else
				{
					base.GameController.ExhaustCoroutine(dealSelfDamage);
				}
				if (base.DidDealDamage(storedResults2, base.CharacterCard, null))
                {
					IEnumerator anotherHeroUsesAPower = base.GameController.SelectHeroToUsePower(this.DecisionMaker, false, true, false, null, null, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && tt != base.TurnTaker), true, true, base.GetCardSource(null));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(anotherHeroUsesAPower);
					}
					else
					{
						base.GameController.ExhaustCoroutine(anotherHeroUsesAPower);
					}
				}
				X++;
            }
			yield break;
		}
	}
}