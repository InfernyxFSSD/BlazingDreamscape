using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SybithosInfernyx.Kyoss
{
	// Token: 0x020003ED RID: 1005
	public abstract class KyossCardController : CardController
	{
		// Token: 0x060021E9 RID: 8681 RVA: 0x00024368 File Offset: 0x00022568
		public KyossCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x060021EA RID: 8682 RVA: 0x00055426 File Offset: 0x00053626
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