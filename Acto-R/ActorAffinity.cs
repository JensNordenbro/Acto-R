namespace ActoR
{
    public enum ActorAffinity
    {
        /// <summary>
        /// No thread affinity
        /// </summary>
        ThreadPoolThread, 
        /// <summary>
        /// Thread affinity
        /// </summary>
        LongRunningThread
    }
}