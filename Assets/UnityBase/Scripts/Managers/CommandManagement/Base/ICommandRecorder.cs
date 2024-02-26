using System;

namespace UnityBase.Command
{
    public interface ICommandRecorder : IDisposable
    {
        public void Execute(ICommand command);
        public void Undo(bool directly = false);
        public void Redo(bool directly = false);
        public void RecordCommand(ICommand command);
        public void ExecuteRecordedCommands(bool directly = false);
        public new void Dispose();
    }
}