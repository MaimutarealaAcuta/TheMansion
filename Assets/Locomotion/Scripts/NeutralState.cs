using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using UnityEngine;

public partial class StateMachine
{
    private class NeutralState : AbstractState
    {
        private readonly StateMachine stateMachine;

        public NeutralState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;

            ActionHandlers.Add(stateMachine.Actions.GetStunned, HandleStun);
            ActionHandlers.Add(stateMachine.Actions.Input, HandleInput);
        }

        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }

        private void HandleStun()
        {
            stateMachine.SwapState(stateMachine.States.Stunned);
        }

        private void HandleInput()
        {
            bool isCrouching = stateMachine.playerController.isCrouching;
            float crouchSpeed = stateMachine.playerController.crouchSpeed;
            bool IsSprinting = stateMachine.playerController.IsSprinting;
            float sprintSpeed = stateMachine.playerController.sprintSpeed;
            float walkSpeed = stateMachine.playerController.walkSpeed;
            ref Vector2 currentInput = ref stateMachine.playerController.currentInput;
            ref Vector3 moveDirection = ref stateMachine.playerController.moveDirection;
            Transform transform = stateMachine.playerController.transform;

            float movementSpeed = isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;
            currentInput = new Vector2(movementSpeed * Input.GetAxis("Vertical"), movementSpeed * Input.GetAxis("Horizontal"));
            float moveDirectionY = moveDirection.y;
            moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
            moveDirection.y = moveDirectionY;
            float avoidFloorDistance = -0.8f;
            float ladderGrabDistance = 0.7f;
            var direction = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;

            if (Physics.Raycast(transform.position + Vector3.up * avoidFloorDistance, direction, out RaycastHit raycastHit, ladderGrabDistance))
            {
                if (raycastHit.transform.TryGetComponent(out Ladder ladder))
                {
                    moveDirection.y = new Vector3(moveDirection.x, 0, moveDirection.z).magnitude;
                    moveDirection.x = 0;
                    moveDirection.z = 0;
                }
            }
        }
    }
}
