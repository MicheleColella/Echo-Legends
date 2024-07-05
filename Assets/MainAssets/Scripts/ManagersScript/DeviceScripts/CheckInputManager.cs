using UnityEngine;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class CheckInputManager : MonoBehaviour
    {
        public static CheckInputManager Instance { get; private set; }

        public enum InputState
        {
            MouseAndKeyboard,
            Gamepad,
            VirtualJoysticks,
            None
        }

        [SerializeField]
        private InputState currentState = InputState.None;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Make sure the manager persists across scenes
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetInputState(InputState newState)
        {
            currentState = newState;
            Debug.Log($"Input state set to: {newState}");
        }

        public InputState GetCurrentInputState()
        {
            return currentState;
        }
    }
}
