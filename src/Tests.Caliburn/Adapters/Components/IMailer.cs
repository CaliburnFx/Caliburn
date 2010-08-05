using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Caliburn.Adapters.Components
{
	public interface IMailer
	{
		void SendMessage(string address, string message);
	}
}
