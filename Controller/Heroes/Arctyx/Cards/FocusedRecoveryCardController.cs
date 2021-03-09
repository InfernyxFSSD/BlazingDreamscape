using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class FocusedRecoveryCardController : CardController
    {
        //Arctyx regains 3 HP.
        //You may discard any number of cards from your hand. Draw X cards, where X is the number of cards discarded this way.

        public FocusedRecoveryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Arctyx regains 3 HP
            IEnumerator heal = GameController.GainHP(CharacterCard, new int?(3), cardSource: GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(heal);
            }
            else
            {
                GameController.ExhaustCoroutine(heal);
            }
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            //Discard any number of cards
            IEnumerator discardCards = GameController.SelectAndDiscardCards(DecisionMaker, null, false, new int?(0), storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCards);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCards);
            }
            int num = GetNumberOfCardsDiscarded(storedResults);
            if (num > 0)
            {
                //Draw cards equal to the amount discarded
                IEnumerator drawCards = DrawCards(DecisionMaker, num);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(drawCards);
                }
                else
                {
                    GameController.ExhaustCoroutine(drawCards);
                }
            }
            yield break;
        }
    }
}