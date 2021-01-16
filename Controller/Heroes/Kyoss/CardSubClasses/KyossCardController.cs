using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	public abstract class KyossCardController : CardController
	{
		public KyossCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		protected TokenPool NukePool
		{
			get
			{
				return base.CharacterCard.FindTokenPool("NukePool");
			}
		}

		protected Card Nuke
		{
			get
			{
				if (this._nuke == null)
				{
					this._nuke = base.FindCard("EmpowermentOfFriendship", true);
				}
				return this._nuke;
			}
		}

		private Card _nuke;
	}
}