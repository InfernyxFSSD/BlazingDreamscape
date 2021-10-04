using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.NightmareHorde
{
    public class NightmareHordeCharacterCardController : VillainCharacterCardController
    {
        public NightmareHordeCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override bool CanBeDestroyed
        {
            get
            {
                return base.CharacterCard.IsFlipped;
            }
        }

        public override void AddSideTriggers()
        {
            //Front - Shambling Fragments
            if(!base.Card.IsFlipped)
            {
                //When a fragment is destroyed, boop the highest HP hero target for 2 psychic
                base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.WasCardDestroyed && d.CardToDestroy.Card.DoKeywordsContain("fragment"), (DestroyCardAction d) => base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => new int?(2), DamageType.Psychic), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.Unspecified));
                //Start of villain turn, hit each non-villain for 1 psychic
                base.AddSideTrigger(base.AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => !c.IsVillain, TargetType.All, 1, DamageType.Psychic));
            }
            //Back - Ravenous Memories
            else
            {
                //When a fragment is destroyed, break a hero card
                base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.WasCardDestroyed && d.CardToDestroy.Card.DoKeywordsContain("fragment"), (DestroyCardAction d) => base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && c.IsInPlayAndHasGameText && !c.IsHeroCharacterCard), 1, optional: false, cardSource: GetCardSource()), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified));
                //Start of villain turn, hit each non-villain for 2 psychic
                base.AddSideTrigger(base.AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => !c.IsVillain, TargetType.All, 2, DamageType.Psychic));
                //End of villain turn, play the top card of the villain deck
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.PlayTheTopCardOfTheVillainDeckWithMessageResponse), new TriggerType[] { TriggerType.PlayCard }));
            }
            base.AddDefeatedIfDestroyedTriggers(false);
        }

        //When Nightmare Horde would be destroyed, flip it
        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (!base.Card.IsFlipped)
            {
                IEnumerator flipit = base.GameController.FlipCard(this, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(flipit);
                }
                else
                {
                    GameController.ExhaustCoroutine(flipit);
                }
            }
            yield break;
        }

        //When flipped to this side, change HP, break cards or heroes hit themselves
        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator afterFlip = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(afterFlip);
            }
            else
            {
                GameController.ExhaustCoroutine(afterFlip);
            }
            IEnumerator changeHP = base.GameController.ChangeMaximumHP(base.Card, 60, true, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(changeHP);
            }
            else
            {
                GameController.ExhaustCoroutine(changeHP);
            }
            Func<HeroTurnTakerController, IEnumerable<Function>> functions = (HeroTurnTakerController httc) => new Function[2]
            {
                new Function(httc, "Deal yourself 2 irreducible Psychic damage", SelectionType.DealDamageSelf, () => DealDamage(httc.CharacterCard, httc.CharacterCard, 2, DamageType.Psychic, isIrreducible: true), forcedActionMessage: httc.Name + " does not have at least 1 destroyable card in play, and therefore must deal themself damage."),
                new Function(httc, "Destroy 1 card", SelectionType.DestroyCard, () => base.GameController.SelectAndDestroyCards(httc, new LinqCardCriteria((Card c) => c.Owner == httc.TurnTaker && !c.IsCharacter, "owner by " + httc.Name, useCardsSuffix: false, useCardsPrefix: true), 1, optional: false, cardSource: GetCardSource()), FindCardsWhere((Card c) => c.IsInPlay && !c.IsCharacter && c.Owner == httc.HeroTurnTaker).Count() >= 1)
            };
            IEnumerator damageOrDestroy = EachPlayerSelectsFunction((HeroTurnTakerController httc) => !httc.IsIncapacitatedOrOutOfGame, functions);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageOrDestroy);
            }
            else
            {
                GameController.ExhaustCoroutine(damageOrDestroy);
            }
            yield break;
        }
    }
}
