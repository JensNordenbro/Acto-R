using System.Threading.Tasks;

namespace ConsoleApplication
{
    public partial class Program
    {

        public interface ITest
        {
            Task<int> Hello();
            Task<string> World();

            Task<MyResult> GetResult();

            Task<int> DelayWorld();
        }
    }
}
