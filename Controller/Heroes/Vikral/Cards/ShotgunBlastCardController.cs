using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace SybithosInfernyx.Vikral
{
    public class ShotgunBlastCardController : MicrostormCardController
    {
        public ShotgunBlastCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 1);
            int powerNumeral2 = base.GetPowerNumeral(1, 1);
            int amount = base.FindCardsWhere((Card c) => c.IsInPlay && IsMicrostorm(c), false, null, false).Count<Card>();
            int amount2 = amount + powerNumeral2;
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), amount2, DamageType.Energy, new int?(powerNumeral), false, new int?(powerNumeral), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
