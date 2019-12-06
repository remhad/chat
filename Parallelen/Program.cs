using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallelen
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] names = { "thomas", "peter", "anton", "berta", "cäser", "dora" };
			//for (int i = 0; i < 10; i++)
			//{
			//	Console.WriteLine(names[i].ToUpper());
			//	if(names[i] == "berta")
			//	{
			//		break;
			//	}
			//}

			while (true)
			{
				// Parallele Forschleife// loopstate- Schleifenzustand
				Parallel.For(0, names.Length, (i, loopState) =>
				{
					Console.WriteLine("{0}: {1}", i, names[i].ToUpper());
					if (names[i] == "berta")
					{
						loopState.Break();
					}
				});


				Console.ReadLine();
			}
		}
	}
}
