using System.Threading.Tasks;

namespace FileWatchWorker.Services.Interfaces
{
    public interface IFileServiceHandler
    {
        ValueTask FileDetected(string fileName);
    }
}
