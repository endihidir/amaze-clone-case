using UnityBase.Command;

namespace UnityBase.Service
{
    public interface ICommandDataService
    {
        public void AddRecorder(string groupName, ICommandRecorder commandRecorder);
        public void RemoveRecorder(string groupName, ICommandRecorder commandRecorder);
        public void ExecuteAllRecords(string groupName, ICommand command);
        public void UndoAllRecords(string groupName, bool directly = false);
        public void RedoAllRecords(string groupName, bool directly = false);
        public void RecordAllCommands(string groupName, ICommand command);
        public void ExecuteAllRecordedCommands(string groupName, bool directly = false);
    }
}