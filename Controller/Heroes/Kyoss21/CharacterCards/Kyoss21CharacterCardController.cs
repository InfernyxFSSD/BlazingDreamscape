using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class Kyoss21CharacterCardController : HeroCharacterCardController
    {

        public Kyoss21CharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//One player may play a card, even if they would otherwise be prevented
						IEnumerator playCard = SelectHeroToPlayCard(DecisionMaker, canBeCancelled: false);
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(playCard);
						}
						else
						{
							GameController.ExhaustCoroutine(playCard);
						}
						break;
					}
				case 1:
					{
						//One hero may use a power, even if they would otherwise be prevented
						IEnumerator usePower = GameController.SelectHeroToUsePower(DecisionMaker, canBeCancelled: false, cardSource: GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(usePower);
						}
						else
						{
							GameController.ExhaustCoroutine(usePower);
						}
						break;
					}
				case 2:
					{
						//One player may draw a card, even if they would otherwise be prevented
						AddToTemporaryTriggerList(AddTrigger<CancelAction>((CancelAction ca) => ca.ActionToCancel.CardSource != null && ca.ActionToCancel.CardSource.Card == CharacterCard, new Func<CancelAction, IEnumerator>(CancelResponse), TriggerType.CancelAction, TriggerTiming.Before, ActionDescription.Unspecified));
						IEnumerator drawCard = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(drawCard);
						}
						else
						{
							GameController.ExhaustCoroutine(drawCard);
						}
						RemoveTemporaryTriggers();
						break;
					}
			}
			yield break;
		}

		public override bool AskIfCardMayPreventAction<T>(TurnTakerController ttc, CardController preventer)
		{
			if (Card.IsIncapacitated)
			{
				return false;
			}
			return AskIfCardMayPreventAction<T>(ttc, preventer);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//One hero target regains 2 HP.
			IEnumerator heal = GameController.SelectAndGainHP(DecisionMaker, 2, additionalCriteria: (Card c) => c.IsTarget && c.IsHero && c.IsInPlay, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(heal);
			}
			else
			{
				GameController.ExhaustCoroutine(heal);
			}
			yield break;
		}
	}
}