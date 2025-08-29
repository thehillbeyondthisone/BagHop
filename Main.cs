// Main.cs
using AOSharp.Core;

namespace NavMeshWizard
{
    public class Main : AOPluginEntry
    {
        public override void Run(string args)
        {
            NavMeshWizard plugin = new NavMeshWizard();
            plugin.Run(args);
        }
    }
}
