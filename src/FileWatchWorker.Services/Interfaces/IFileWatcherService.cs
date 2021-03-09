namespace FileWatchWorker.Services.Interfaces
{
    public interface IFileWatcherService
    {
        void StartWatcher();
        void StopWatcher();
    }
}
