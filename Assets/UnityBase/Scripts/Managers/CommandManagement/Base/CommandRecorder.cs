using System.Collections.Generic;

namespace UnityBase.Command
{
    public class CommandRecorder : ICommandRecorder
    {
        private ICommand _activeCommand;
        private Stack<ICommand> _undoStack = new Stack<ICommand>();
        private Stack<ICommand> _redoStack = new Stack<ICommand>();

        private Queue<ICommand> _savedCommandQueue = new Queue<ICommand>();
        private int _savedCommandCounter;
        private bool _isSavedExecutionStarted;

        public void Execute(ICommand command)
        {
            if (IsCommandInProgress()) return;

            _activeCommand?.Cancel();

            _activeCommand = command;

            _activeCommand.Execute();

            _undoStack.Push(_activeCommand);

            _redoStack.Clear();
        }

        public void Undo(bool directly = false)
        {
            if (IsCommandInProgress()) return;

            if (IsEqualZero(_undoStack.Count)) return;

            UpdateActiveCommand(_undoStack);

            if (IsActiveCommandNotValidate(_redoStack)) return;

            _activeCommand.Undo(directly);

            _redoStack.Push(_activeCommand);
        }

        public void Redo(bool directly = false)
        {
            if (IsCommandInProgress()) return;

            if (IsEqualZero(_redoStack.Count)) return;

            UpdateActiveCommand(_redoStack);

            if (IsActiveCommandNotValidate(_undoStack)) return;

            _activeCommand.Redo(directly);

            _undoStack.Push(_activeCommand);
        }

        public void RecordCommand(ICommand command)
        {
            command.Record();

            _savedCommandQueue.Enqueue(command);

            _savedCommandCounter++;
        }

        public async void ExecuteRecordedCommands(bool directly = false)
        {
            if (_isSavedExecutionStarted) return;

            _isSavedExecutionStarted = true;

            for (int i = 0; i < _savedCommandCounter; i++)
            {
                var isThereCommand = _savedCommandQueue.TryDequeue(out var command);

                if (isThereCommand)
                {
                    await command.Redo(directly);
                }
            }

            _savedCommandCounter = 0;

            _isSavedExecutionStarted = false;
        }

        private void UpdateActiveCommand(Stack<ICommand> stack)
        {
            _activeCommand?.Cancel();
            _activeCommand = stack.Pop();
        }

        private bool IsCommandInProgress()
        {
            if (_activeCommand is null) return false;
            return _activeCommand.IsInProgress && !_activeCommand.CanPassNextCommandInstantly;
        } 
        
        private bool IsEqualZero(int count) => count < 1;
        private bool IsActiveCommandNotValidate(Stack<ICommand> stack) => _activeCommand is null || stack.Contains(_activeCommand);
        
        public void Dispose()
        {
            _undoStack = null;
            _redoStack = null;
            _savedCommandQueue = null;
            _activeCommand = null;
        }
    }
}