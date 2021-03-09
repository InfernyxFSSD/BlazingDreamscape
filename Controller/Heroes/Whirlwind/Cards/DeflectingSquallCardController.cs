using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class DeflectingSquallCardController : CardController
    {
        //Select a hero other than Whirlwind. Reduce the next damage dealt to that target by 2.

        public DeflectingSquallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            //Select a hero other than Whirlwind
            IEnumerator selectHero = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsHero && c.IsCharacter && c != TurnTaker.CharacterCard, $"hero other than {TurnTaker.Name}"), storedResults, false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectHero);
            }
            else
            {
                GameController.ExhaustCoroutine(selectHero);
            }
            Card selectedCard = GetSelectedCard(storedResults);
            if (selectedCard != null)
            {
                //Reduce the next damage they would be dealt by 2
                ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(2);
                rdse.TargetCriteria.IsSpecificCard = selectedCard;
                rdse.NumberOfUses = 1;
                rdse.UntilCardLeavesPlay(selectedCard);
                IEnumerator applyStatus = AddStatusEffect(rdse);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(applyStatus);
                }
                else
                {
                    GameController.ExhaustCoroutine(applyStatus);
                }
            }
            yield break;
        }
    }
}