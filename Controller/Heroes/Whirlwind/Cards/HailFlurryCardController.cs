using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class HailFlurryCardController : MicroWeatherCardController
    {
        //At the end of your turn, Whirlwind deals up to X targets 1 Cold damage each, where X is the number of Weather Effects in play.
        public HailFlurryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            //Whirlwind deals damage at the end of your turn
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EndOfTurnResponse(), new TriggerType[] { TriggerType.DealDamage });
        }

        private IEnumerator EndOfTurnResponse()
        {
            //Hit as many targets as there are weather effects
            IEnumerator hitTargets = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(this.GameController, this.CharacterCard), 1, DamageType.Cold, GetNumberOfTargets(), false, new int?(0), cardSource: this.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(hitTargets);
            }
            else
            {
                GameController.ExhaustCoroutine(hitTargets);
            }
            yield break;
        }

        private int GetNumberOfTargets()
        {
            return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsWeatherEffect).Count<Card>();
        }
    }
}