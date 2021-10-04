using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace BlazingDreamscape.NightmareHorde
{
    public class NightmareHordeTurnTakerController : TurnTakerController
    {
        public NightmareHordeTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            //Put H-1 fragments into play at the start
            IEnumerator findFragments = base.PutCardsIntoPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("fragment"), "fragment"), base.H - 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(findFragments);
            }
            else
            {
                GameController.ExhaustCoroutine(findFragments);
            }
            yield break;
        }
    }
}