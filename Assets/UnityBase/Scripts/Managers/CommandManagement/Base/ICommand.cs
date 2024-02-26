using Cysharp.Threading.Tasks;

namespace UnityBase.Command
{
    public interface ICommand
    {
        public bool IsInProgress { get; }
        public bool CanPassNextCommandInstantly { get; }
        public void Record();
        public UniTask Execute();
        public UniTask Undo(bool directly);
        public UniTask Redo(bool directly);
        public void Cancel();
        public void Dispose();
    }
}