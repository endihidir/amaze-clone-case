using System;
using Cysharp.Threading.Tasks;

namespace UnityBase.Command
{
    public abstract class MoveCommand : ICommand
    {
        protected readonly IMoveEntity _moveEntity;
        public abstract bool IsInProgress { get; }
        public abstract bool CanPassNextCommandInstantly { get; }
        
        protected MoveCommand(IMoveEntity moveEntity) => _moveEntity = moveEntity;
        
        public abstract void Record();
        public abstract UniTask Execute();
        public abstract UniTask Undo(bool directly);
        public abstract UniTask Redo(bool directly);
        public abstract void Cancel();
        public abstract void Dispose();
        public static T Create<T>(IMoveEntity moveEntity) where T : MoveCommand => (T)Activator.CreateInstance(typeof(T), moveEntity);
    }
}