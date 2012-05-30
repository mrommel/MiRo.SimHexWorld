using System.Threading;
using log4net;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public class AiPlayerData : AbstractPlayerData
    {
        public class AiPlayerWorker
        {
            ILog log = LogManager.GetLogger(typeof(AiPlayerWorker));

            AiPlayerData _parent;

            public AiPlayerWorker(AiPlayerData parent)
            {
                _parent = parent;
            }

            public void Run()
            {
                while (!_shouldStop)
                {
                    Thread.Sleep(5000);
                    log.DebugFormat("worker thread({0}): working...", _parent.Id);
                }

                log.DebugFormat("worker thread({0}): terminating gracefully.", _parent.Id);
            }

            public void RequestStop()
            {
                _shouldStop = true;
                log.DebugFormat("worker thread({0}): request stop.", _parent.Id);
            }
		
            // Volatile is used as hint to the compiler that this data
            // member will be accessed by multiple threads.
            private volatile bool _shouldStop;
        }

        Thread _thread;
        AiPlayerWorker _worker;
	
        public AiPlayerData(int id) 
            : base(id,false)
        {
		
        }

        public override void StartAiThreads()
        {
            _worker = new AiPlayerWorker(this);

            _thread = new Thread(new ThreadStart(_worker.Run));
            _thread.Start();
        }

        public override void Dispose()
        {
            if (_worker != null)
                _worker.RequestStop();

            // Spin for a while waiting for the started thread to die
            while (_thread.IsAlive);
        }
    }
}