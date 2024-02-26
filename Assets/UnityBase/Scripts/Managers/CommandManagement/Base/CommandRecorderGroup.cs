using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityBase.Command
{
    public sealed class CommandRecorderGroup : IDisposable
    {
        private List<ICommandRecorder> _commandRecorders = new List<ICommandRecorder>();
    
        public void Add(ICommandRecorder commandRecorder)
        {
            if (!_commandRecorders.Contains(commandRecorder))
            {
                _commandRecorders.Add(commandRecorder);
            }
            else
            {
                Debug.LogError("You try to add same command twice!");
            }
        }

        public void Remove(ICommandRecorder commandRecorder)
        {
            if (_commandRecorders.Contains(commandRecorder))
            {
                _commandRecorders.Remove(commandRecorder);
            }
            else
            {
                Debug.LogError("There is no command to remove!");
            }
        }

        public void ExecuteAllRecords(ICommand command) => _commandRecorders.ForEach(x=> x?.Execute(command));
        public void UndoAllRecords(bool directly) => _commandRecorders.ForEach(x=> x?.Undo(directly));
        public void RedoAllRecords(bool directly) => _commandRecorders.ForEach(x=> x?.Redo(directly));
        public void RecordAllCommands(ICommand command) => _commandRecorders.ForEach(x=> x?.RecordCommand(command));
        public void ExecuteAllRecordedCommands(bool directly) => _commandRecorders.ForEach(x=> x?.ExecuteRecordedCommands(directly));

        public void Dispose()
        {
            _commandRecorders?.ForEach(x=> x?.Dispose());
            _commandRecorders?.Clear();
        }
    }
}