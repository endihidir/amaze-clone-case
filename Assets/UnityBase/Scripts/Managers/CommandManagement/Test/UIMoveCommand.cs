using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityBase.Extensions;
using UnityEngine;

namespace UnityBase.Command
{
    public class UIMoveCommand : MoveCommand
    {
        private Vector3 _newPosition;
        private Vector3 _oldPosition;
        private bool _isInProgress;
        private CancellationTokenSource _cancellationTokenSource;
        public override bool IsInProgress => _isInProgress;
        public override bool CanPassNextCommandInstantly => _moveEntity?.CanPassNextMovementInstantly ?? false;
        
        public UIMoveCommand(IMoveEntity moveEntity) : base(moveEntity) { }

        public override void Record() => _newPosition = _moveEntity.NewPosition;

        public override async UniTask Execute()
        {
            _oldPosition = _moveEntity.Transform.position;
            
            _newPosition = _moveEntity.NewPosition;

            await MoveObjectAsync(_newPosition);
        }

        public override async UniTask Undo(bool directly)
        {
            if (directly)
            {
                _moveEntity.Transform.position = _oldPosition;
                return;    
            }
            
            await MoveObjectAsync(_oldPosition);
        }

        public override async UniTask Redo(bool directly)
        {
            if (directly)
            {
                _moveEntity.Transform.transform.position = _newPosition;
                return;
            }
            
            await MoveObjectAsync(_newPosition);
        }

        public override void Cancel() => _cancellationTokenSource?.Cancel();

        public override void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private async UniTask MoveObjectAsync(Vector3 targetPosition)
        {
            _isInProgress = true;
            
            CancellationTokenExtentions.Refresh(ref _cancellationTokenSource);

            try
            {
                var transform = _moveEntity.Transform;
                var speed = Vector3.Distance(transform.position, targetPosition) / (_moveEntity.Duration / 2);

                while (transform.position.Distance(targetPosition) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                    
                    await UniTask.Yield(PlayerLoopTiming.Update, _cancellationTokenSource.Token);
                }

                transform.position = targetPosition;
                _isInProgress = false;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}