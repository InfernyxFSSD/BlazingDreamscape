using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Cyberknight
{
    public class RepairArmorCardController : CardController
    {
        //Power: Discard a card to put an armor from trash into play

        public RepairArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<DiscardCardAction> discardedCard = new List<DiscardCardAction>();
            IEnumerator discardCard = base.GameController.SelectAndDiscardCard(this.DecisionMaker, storedResults: discardedCard, cardSource: base.GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCard);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCard);
            }
            if(discardedCard.Any())
            {
                IEnumerator findArmor = base.SearchForCards(base.HeroTurnTakerController, false, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("armor"), "armor"), true, false, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(findArmor);
                }
                else
                {
                    GameController.ExhaustCoroutine(findArmor);
                }
            }
            yield break;
        }
    }
}