using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace BlazingDreamscape.Kyoss21
{
    public class VariableProjectorCardController : CardController
    {
        //When this card enters play, place it next to a hero. That hero gains the following power:
        //Power: Select a damage type. This hero deals two targets 2 damage of that type.

        public VariableProjectorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            AddTriggers();
            AddAsPowerContributor();
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //When this card enters play, put it next to a hero
            IEnumerator selectHero = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard, "hero character"), storedResults, isPutIntoPlay, decisionSources);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selectHero);
            }
            else
            {
                GameController.ExhaustCoroutine(selectHero);
            }
            yield break;
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cc)
        {
            if (GetCardThisCardIsNextTo(true) != null && cc.Card == GetCardThisCardIsNextTo(true))
            {
                //If this card is next to a hero, they have this power
                List<Power> list = new List<Power>();
                Power pew = new Power(cc.DecisionMaker, cc, "Select a damage type. This hero deals two targets 2 damage of that type.", SelectAndPewPewResponse(cc), 0, null, GetCardSource());
                list.Add(pew);
                return list;
            }
            return null;
        }

        private IEnumerator SelectAndPewPewResponse(CardController cc)
        {
            int powerNumeral = GetPowerNumeral(0, 2);
            HeroTurnTakerController hero = cc.HeroTurnTakerController;
            //Select a damage type
            List<SelectDamageTypeDecision> chosenType = new List<SelectDamageTypeDecision>();
            IEnumerator chooseType = GameController.SelectDamageType(hero, chosenType, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseType);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseType);
            }
            DamageType? damageType = GetSelectedDamageType(chosenType);
            if (damageType != null)
            {
                //Hit two targets for 2 damage of that type
                IEnumerator plink = GameController.SelectTargetsAndDealDamage(hero, new DamageSource(GameController, hero.CharacterCard), powerNumeral, damageType.Value, 2, false, 2, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(plink);
                }
                else
                {
                    GameController.ExhaustCoroutine(plink);
                }
            }
            yield break;
        }
    }
}