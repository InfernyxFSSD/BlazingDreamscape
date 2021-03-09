using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace BlazingDreamscape.Wildfire
{
    public class CombustionStaffCardController : CardController
    {
        //Power: Increase damage dealt by elementals in your play area by 1 until the start of your next turn.

        public CombustionStaffCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Increase damage dealt by elementals
            int powerNumeral = GetPowerNumeral(0, 1);
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(powerNumeral);
            idse.SourceCriteria.HasAnyOfTheseKeywords = new List<string> { "elemental" };
            idse.UntilStartOfNextTurn(TurnTaker);
            idse.SourceCriteria.IsAtLocation = TurnTaker.PlayArea;
            IEnumerator applyStatus = AddStatusEffect(idse);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(applyStatus);
            }
            else
            {
                GameController.ExhaustCoroutine(applyStatus);
            }
            yield break;
        }
    }
}
