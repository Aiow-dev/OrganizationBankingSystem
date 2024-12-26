using System;
using System.IO;

namespace OrganizationBankingSystem
{
    class EnvManager
    {
        public static void LoadEnvironment()
        {
            var root = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);
        }
    }
}
