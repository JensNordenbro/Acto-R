using System;

namespace ConsoleApplication
{
    public partial class Program
    {

        public class MyResult
        {
            public int i = new Random().Next();
            public int j = new Random().Next();

            public string text = "info";

            string internalText = "internal_info";

            public override string ToString() => $"(i:{i}, j:{j}, text:{text}, internalText:{internalText})";
        }
    }
}
