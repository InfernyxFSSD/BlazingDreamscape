using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Vikral
{
	public class WildCallingCardController : MicrostormCardController
	{
		public WildCallingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			List<SelectCardDecision> storedResultMicrostorm = new List<SelectCardDecision>();
			IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.None, new LinqCardCriteria((Card c) => c.IsInPlayAndNotUnderCard && IsMicrostorm(c)), storedResultMicrostorm, false, false, null, true, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidSelectCard(storedResultMicrostorm))
			{
				Card selectedMicrostorm = storedResultMicrostorm.FirstOrDefault()?.SelectedCard;
				List<string> choices = new List<string>();
				if (selectedMicrostorm.Identifier == HailFlurryIdentifier)
				{
					choices.Add(FlaringBlazeIdentifier);
					choices.Add(BladedGaleIdentifier);
				}
				else if (selectedMicrostorm.Identifier == FlaringBlazeIdentifier)
				{
					choices.Add(HailFlurryIdentifier);
					choices.Add(ToxicCloudIdentifier);
				}
				else if (selectedMicrostorm.Identifier == ScathingSandstormIdentifier)
				{
					choices.Add(PsionicTorrentIdentifier);
					choices.Add(ToxicCloudIdentifier);
				}
				else if (selectedMicrostorm.Identifier == BladedGaleIdentifier)
				{
					choices.Add(VigilanteJusticeIdentifier);
					choices.Add(ThunderStormIdentifier);
				}
				else if (selectedMicrostorm.Identifier == VigilanteJusticeIdentifier)
				{
					choices.Add(BladedGaleIdentifier);
					choices.Add(PsionicTorrentIdentifier);
				}
				else if (selectedMicrostorm.Identifier == ViciousRetributionIdentifier)
				{
					choices.Add(ThunderStormIdentifier);
					choices.Add(HailFlurryIdentifier);
				}
				else if (selectedMicrostorm.Identifier == ThunderStormIdentifier)
				{
					choices.Add(ScathingSandstormIdentifier);
					choices.Add(OverloadPulseIdentifier);
				}
				else if (selectedMicrostorm.Identifier == ToxicCloudIdentifier)
				{
					choices.Add(OverloadPulseIdentifier);
					choices.Add(VigilanteJusticeIdentifier);
				}
				else if (selectedMicrostorm.Identifier == PsionicTorrentIdentifier)
				{
					choices.Add(ViciousRetributionIdentifier);
					choices.Add(FlaringBlazeIdentifier);
				}
				else if (selectedMicrostorm.Identifier == OverloadPulseIdentifier)
				{
					choices.Add(ScathingSandstormIdentifier);
					choices.Add(ViciousRetributionIdentifier);
				}
				coroutine = base.SearchForCards(base.HeroTurnTakerController, true, true, 1, 1, new LinqCardCriteria((Card c) => choices.Contains(c.Identifier), "microstorm listed on the selected microstorm", false, false, null, null, false), true, false, false, false, null, false, null, null);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			else
            {
				coroutine = base.GameController.SendMessageAction("No microstorm selected.", Priority.High, GetCardSource(null), null, false);
				if (UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			coroutine = base.GameController.DrawCard(base.HeroTurnTaker, true, null, true, null, null);
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
}
