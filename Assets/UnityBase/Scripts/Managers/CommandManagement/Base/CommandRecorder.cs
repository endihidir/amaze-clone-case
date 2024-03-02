using System;
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

        public void Execute(ICommand command, Action onComplete)
        {
            if (IsCommandInProgress()) return;

            _activeCommand?.Cancel();

            _activeCommand = command;

            _activeCommand.Execute(onComplete);

            _undoStack.Push(_activeCommand);

            _redoStack.Clear();
        }

        public void Undo(bool directly, Action onComplete)
        {
            if (IsCommandInProgress()) return;

            if (IsEqualZero(_undoStack.Count)) return;

            UpdateActiveCommand(_undoStack);

            if (IsActiveCommandNotValidate(_redoStack)) return;

            _activeCommand.Undo(directly, onComplete);

            _redoStack.Push(_activeCommand);
        }

        public void Redo(bool directly, Action onComplete)
        {
            if (IsCommandInProgress()) return;

            if (IsEqualZero(_redoStack.Count)) return;

            UpdateActiveCommand(_redoStack);

            if (IsActiveCommandNotValidate(_undoStack)) return;

            _activeCommand.Redo(directly, onComplete);

            _undoStack.Push(_activeCommand);
        }

        public void RecordCommand(ICommand command)
        {
            command.Record();

            _savedCommandQueue.Enqueue(command);

            _savedCommandCounter++;
        }

        public void ExecuteRecordedCommands(bool directly = false)
        {
            if (_isSavedExecutionStarted) return;

            _isSavedExecutionStarted = true;

            int counter = 0;

            for (int i = 0; i < _savedCommandCounter; i++)
            {
                var isThereCommand = _savedCommandQueue.TryDequeue(out var command);

                if (isThereCommand)
                {
                    command.Redo(directly,()=> IsAllRecordedCommandExecuted(ref counter));
                }
            }
        }


        private void UpdateActiveCommand(Stack<ICommand> stack)
        {
            _activeCommand?.Cancel();
            _activeCommand = stack.Pop();
        }
        private void IsAllRecordedCommandExecuted(ref int counter)
        {
            counter++;
            if (counter != _savedCommandCounter) return;
            
            _savedCommandCounter = 0;
            _isSavedExecutionStarted = false;
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