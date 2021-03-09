using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class AuraDisplacementArctyxCharacterCardController : HeroCharacterCardController
    {

        public AuraDisplacementArctyxCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//One player may play a card.
						IEnumerator playCard = SelectHeroToPlayCard(DecisionMaker);
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
						//One hero may use a power.
						IEnumerator usePower = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
						//One player may draw a card.
						IEnumerator drawCard = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(drawCard);
						}
						else
						{
							GameController.ExhaustCoroutine(drawCard);
						}
						break;
					}
			}
			yield break;
		}

        public override IEnumerator UsePower(int index = 0)
        {
			//Discard the top card of your deck. If an aura was discarded this way, one other player may play a card. Otherwise, one player may draw a card.
			List<MoveCardAction> storedResults = new List<MoveCardAction>();
			IEnumerator discardTop = DiscardCardsFromTopOfDeck(TurnTakerController, 1, storedResults: storedResults, responsibleTurnTaker: TurnTaker);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(discardTop);
			}
			else
			{
				GameController.ExhaustCoroutine(discardTop);
			}
			foreach (MoveCardAction moveCardAction in storedResults)
            {
				IEnumerator drawOrPlayCard;
				if (moveCardAction.CardToMove.DoKeywordsContain("aura"))
                {
					drawOrPlayCard = SelectHeroToPlayCard(DecisionMaker, heroCriteria: new LinqTurnTakerCriteria((TurnTaker h) => h != TurnTaker, () => "a hero other than " + TurnTaker.Name));
				}
				else
                {
					drawOrPlayCard = GameController.SelectHeroToDrawCard(DecisionMaker, numberOfCards: new int?(1), cardSource: GetCardSource());
				}
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(drawOrPlayCard);
				}
				else
				{
					GameController.ExhaustCoroutine(drawOrPlayCard);
				}
				yield break;
            }
        }
    }
}