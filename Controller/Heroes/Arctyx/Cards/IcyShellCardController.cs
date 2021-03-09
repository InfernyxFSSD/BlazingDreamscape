using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class IcyShellCardController : FrostCardController
    {
        //When this card enters play, destroy any other frost cards in play.
        //At the end of your turn, Arctyx regains 2 HP.
        //Power: Reduce damage dealt to heroes by 1 until the start of your next turn.

        public IcyShellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //End of your turn, Arctyx regains 2 HP
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => GameController.GainHP(CharacterCard, new int?(1), cardSource: GetCardSource()), TriggerType.GainHP);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Reduce damage dealt to heroes by 1 until the start of your next turn
            int powerNumeral = GetPowerNumeral(0, 1);
            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(powerNumeral);
            rdse.TargetCriteria.IsHero = true;
            rdse.TargetCriteria.IsCharacter = true;
            rdse.UntilStartOfNextTurn(this.TurnTaker);
            IEnumerator applyStatus = AddStatusEffect(rdse);
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