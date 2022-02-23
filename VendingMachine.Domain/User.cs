using System;
using System.Collections.Generic;
using System.Text;

namespace VendingMachine.Domain
{
	public class User
	{
		public override string ToString() => $"{nameof(User)} {this.Wallet}";

		public Wallet Wallet { get; }

		public User(Wallet wallet)
		{
			this.Wallet = wallet;
		}
	}
}
