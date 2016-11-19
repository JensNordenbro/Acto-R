using System.Threading.Tasks;

namespace ActoR
{
    static class TaskExtenstensions
    {
        public static Task MuteResult(this Task t)
        {
            return t.ContinueWith(task => { });
        }
    }
}
