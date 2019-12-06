using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.Unicode; // = Encoding.UTF8;

			Console.WriteLine("×éîĹƍƞƟƜƭȭȕȦʀɸɡʦϾϙЀЗЕԫԁ");

			string meinText = "×éîĹƍƞƟƜƭȭȕȦʀɸ ßÄÖÜäöü";
			byte[] meinTextBytes = Encoding.Unicode.GetBytes(meinText);
			string meinText2 = Encoding.Unicode.GetString(meinTextBytes);

			Console.WriteLine(meinText2);

			if (meinText == meinText2)
			{
				Console.WriteLine("Ok");
			}
			else
			{
				Console.WriteLine("Nicht ok");
			}


			Console.ReadKey();
		}
	}
}
