using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class MandatoryShutdownCardController : CardController
    {
        //Power: Kyoss 2.1 deals a target 2 lightning. Prevent the next damage dealt by that target.

        public MandatoryShutdownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = GetPowerNumeral(0, 2);
            //Select a target to boop
            IEnumerator lightningBoop = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), powerNumeral, DamageType.Lightning, 1, false, 1, addStatusEffect: new Func<DealDamageAction, IEnumerator> (PreventNextDamageDealtByTarget), selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(lightningBoop);
            }
            else
            {
                GameController.ExhaustCoroutine(lightningBoop);
            }
            yield break;
        }

        private IEnumerator PreventNextDamageDealtByTarget(DealDamageAction dda)
        {
            //Target booped can't deal damage once, even if no damage was actually dealt
            CannotDealDamageStatusEffect cddse = new CannotDealDamageStatusEffect();
            cddse.SourceCriteria.IsSpecificCard = dda.Target;
            cddse.NumberOfUses = 1;
            cddse.UntilCardLeavesPlay(dda.Target);
            IEnumerator applyStatus = AddStatusEffect(cddse);
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