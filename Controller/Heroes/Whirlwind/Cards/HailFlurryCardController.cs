using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Whirlwind
{
    public class HailFlurryCardController : CardController
    {
        //At the end of your turn, Whirlwind deals up to X targets 1 Cold damage each, where X is the number of Microstorms in play.
        public HailFlurryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //How many Microstorms are in play?
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm"), "microstorm", false, singular: "microstorm", plural: "microstorms"));
        }
        public override void AddTriggers()
        {
            //Whirlwind deals damage at the end of your turn
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EndOfTurnResponse(), new TriggerType[] { TriggerType.DealDamage });
        }

        private IEnumerator EndOfTurnResponse()
        {
            //Count number of Microstorms in play, then hit that many targets
            int targetCount = FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("microstorm") && c.IsInPlayAndHasGameText)).Count();
            IEnumerator hitTargets = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Cold, targetCount, false, 0, cardSource: GetCardSource());
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
    }
}