using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace BlazingDreamscape.Arctyx
{
    public class HeavyMaceCardController : CardController
    {
        //Power: Arctyx deals a target 3 Melee damage.

        public HeavyMaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = GetPowerNumeral(0, 3);
            IEnumerator smack = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeral, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(smack);
            }
            else
            {
                GameController.ExhaustCoroutine(smack);
            }
            yield break;
        }
    }
}
