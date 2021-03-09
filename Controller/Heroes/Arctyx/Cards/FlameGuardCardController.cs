using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class FlameGuardCardController : FlameCardController
    {
        //When this card enters play, destroy any other flame cards in play.
        //The first time Arctyx is dealt damage by each non-hero target each turn, Arctyx deals that target 2 Fire damage.
        //Power: A hero other than Arctyx may use a power now.

        public FlameGuardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Arctyx retaliates for 2 fire
            AddCounterDamageTrigger((DealDamageAction dda) => dda.Target == CharacterCard && dda.DamageSource.IsTarget && !dda.DamageSource.IsHero, () => CharacterCard, () => CharacterCard, true, 2, DamageType.Fire);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //A hero other than Arctyx may use a power
            IEnumerator usePower = GameController.SelectHeroToUsePower(DecisionMaker, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt != this.TurnTaker), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(usePower);
            }
            else
            {
                GameController.ExhaustCoroutine(usePower);
            }
            yield break;
        }
    }
}