using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace SybithosInfernyx.Blitz
{
    public class CombustionStaffCardController : FocusCardController
    {
        public CombustionStaffCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = base.GetPowerNumeral(0, 1);
            IncreaseDamageStatusEffect idse = new IncreaseDamageStatusEffect(powerNumeral);
            idse.SourceCriteria.HasAnyOfTheseKeywords = new List<string> { "elemental" };
            idse.UntilStartOfNextTurn(this.TurnTaker);
            idse.SourceCriteria.IsAtLocation = this.TurnTaker.PlayArea;
            IEnumerator increaseDamage = base.AddStatusEffect(idse, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseDamage);
            }
        }
    }
}
