using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class DoppelmemoryCardController : FragmentCardController
    {
        public DoppelmemoryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected const string HasHitSomethingThisTurn = "HasHitSomethingThisTurn";

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero
            IEnumerator doppelWho = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && c.IsHeroCharacterCard && c.IsInPlayAndHasGameText), storedResults, isPutIntoPlay, decisionSources);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(doppelWho);
            }
            else
            {
                GameController.ExhaustCoroutine(doppelWho);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //The first time each turn the hero next to this card deals damage, this card deals them the same amount of Psychic damage
            base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DidDealDamage && dd.DamageSource.IsSameCard(base.GetCardThisCardIsNextTo(true)) && !base.HasBeenSetToTrueThisTurn(HasHitSomethingThisTurn), new Func<DealDamageAction, IEnumerator>(this.DealHeroDamageResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified);
            //When the hero draws a card, play the top of the villain deck
            base.AddTrigger<DrawCardAction>((DrawCardAction dc) => dc.DidDrawCard && base.GetCardThisCardIsNextTo(true) != null && dc.DrawnCard.Owner.CharacterCard == base.GetCardThisCardIsNextTo(true), new Func<DrawCardAction, IEnumerator>(base.PlayTheTopCardOfTheVillainDeckWithMessageResponse), TriggerType.PlayCard, TriggerTiming.After, ActionDescription.Unspecified);

        }

        private IEnumerator DealHeroDamageResponse(DealDamageAction dd)
        {
            base.SetCardPropertyToTrueIfRealAction(HasHitSomethingThisTurn);
            IEnumerator boopHero = base.DealDamage(this.Card, base.GetCardThisCardIsNextTo(), dd.Amount, DamageType.Psychic, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(boopHero);
            }
            else
            {
                GameController.ExhaustCoroutine(boopHero);
            }
            yield break;
        }
    }
}