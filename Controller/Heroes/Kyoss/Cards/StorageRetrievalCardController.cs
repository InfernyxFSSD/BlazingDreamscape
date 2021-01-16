using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public class StorageRetrievalCardController : CardController
	{
		public StorageRetrievalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
			IEnumerator coroutine2 = base.EachPlayerDrawsACard(null, true, true, null, true, false);
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
