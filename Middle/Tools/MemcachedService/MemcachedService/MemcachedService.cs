using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace MemcachedService
{
    public partial class MemcachedService : ServiceBase
    {
        private Process memcachedProcess;

        public MemcachedService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            memcachedProcess = Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Memcached",
                "Memcached.exe"));
        }

        protected override void OnStop()
        {
            memcachedProcess.Kill();
        }
    }
}
