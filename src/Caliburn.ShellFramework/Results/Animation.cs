namespace Caliburn.ShellFramework.Results
{
    public static class Animation
    {
        public static AnimationResult Begin(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Begin);
        }

        public static AnimationResult Pause(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Pause);
        }

        public static AnimationResult Resume(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Resume);
        }

        public static AnimationResult Stop(string key)
        {
            return new AnimationResult(key, AnimationResult.AnimationAction.Stop);
        }
    }
}