using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Arctyx
{
    public class AuraLaceCardController : CardController
    {
        //While there is an Aura card in your play area, whenever Arctyx is dealt damage by a non-hero target, increase the next damage dealt to that target by Arctyx by 1.
        //Power: Discard 2 cards. Search your deck or trash for an aura and put it into play. Shuffle your trash into your deck.

        public AuraLaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura") && c.Location == TurnTaker.PlayArea));
        }

        public override void AddTriggers()
        {
            //If you're hit by a non-hero target and have an Aura in your play area, increase your next damage dealt to that target
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsTarget && !dd.DamageSource.IsHero && dd.Target == CharacterCard && dd.DidDealDamage && dd.Amount > 0 && FindCardsWhere((Card c) => c.IsInPlay && c.DoKeywordsContain("aura") && c.Location == TurnTaker.PlayArea).Count<Card>() > 0, new Func<DealDamageAction, IEnumerator>(AddStatusEffectResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.DamageTaken);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard 2 cards...
            int powerNumeral = GetPowerNumeral(0, 2);
            IEnumerator discardCards = GameController.SelectAndDiscardCards(DecisionMaker, powerNumeral, false, powerNumeral, selectionType: SelectionType.DiscardCard, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discardCards);
            }
            else
            {
                GameController.ExhaustCoroutine(discardCards);
            }
            LinqCardCriteria auraCards = new LinqCardCriteria((Card c) => c.DoKeywordsContain("aura"), "aura");
            //Search your deck or trash for an Aura...
            IEnumerator searchForCards = SearchForCards(DecisionMaker, true, true, new int?(1), 1, auraCards, true, false, false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(searchForCards);
            }
            else
            {
                GameController.ExhaustCoroutine(searchForCards);
            }
            List<Card> list = new List<Card>();
            list.AddRange(TurnTaker.Trash.Cards);
            AddInhibitorException((GameAction ga) => ga is ShuffleCardsAction);
            //Shuffle your trash into your deck
            IEnumerator shuffleCards = GameController.ShuffleCardsIntoLocation(DecisionMaker, list, TurnTaker.Deck, cardSource: GetCardSource(null));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(shuffleCards);
            }
            else
            {
                GameController.ExhaustCoroutine(shuffleCards);
            }
            yield break;
        }

        private IEnumerator AddStatusEffectResponse(DealDamageAction dd)
        {
            //Status that increases the next damage dealt to the target that hit Arctyx
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(1);
            idse.SourceCriteria.IsSpecificCard = CharacterCard;
            idse.TargetCriteria.IsSpecificCard = dd.DamageSource.Card;
            idse.NumberOfUses = 1;
            IEnumerator applyStatus = AddStatusEffect(idse);
            if(UseUnityCoroutines)
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