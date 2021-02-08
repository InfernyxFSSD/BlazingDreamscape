using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Wildfire
{
    public class IncineratingDrakeCardController : CardController
    {
        public IncineratingDrakeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => $"{this.Card.Title} will deal {(int)base.GetCardPropertyJournalEntryInteger(DamageToDeal)} Fire damage.").Condition = (() => base.Card.IsInPlayAndNotUnderCard);
        }

        public override void AddTriggers()
        {
            Func<Card, int?> damageToDeal = (Card target) => new int?(this.GetValueOfDamageDealt());
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => true, TargetType.SelectTarget, 0, DamageType.Fire, false, false, 1, 1, null, damageToDeal);
            base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.OnDestroyResponse), new TriggerType[] { TriggerType.PutIntoPlay }, null, null);
            base.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagAfterLeavesPlay("IncineratingDrakeDamageToDeal"), TriggerType.Hidden);
        }

        private int GetValueOfDamageDealt()
        {
            return (int)base.GetCardPropertyJournalEntryInteger(DamageToDeal);
        }

        public override IEnumerator Play()
        {
            int damageToDeal = base.FindCardsWhere((Card c) => c.DoKeywordsContain("elemental", false, false) && c.IsInPlayAndHasGameText && c.Location.IsPlayAreaOf(this.TurnTaker), false, null, false).Count<Card>() + 1;
            base.SetCardProperty(DamageToDeal, damageToDeal);
            yield break;
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dc)
        {
            IEnumerator findElem = base.SearchForCards(base.HeroTurnTakerController, false, true, new int?(1), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("elemental", false, false), "elemental", true, false, null, null, false), true, false, false, false, null, false, true, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findElem);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findElem);
            }
            yield break;
        }

        private const string DamageToDeal = "IncineratingDrakeDamageToDeal";
    }
}