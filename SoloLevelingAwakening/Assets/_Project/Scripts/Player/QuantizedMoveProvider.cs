using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

namespace SLA.Player
{
    /// <summary>
    /// Smooth thumbstick movement with CMF-2's quantized speeds: inside the stick's dead zone the player
    /// stands still; any deflection past it moves at exactly walk speed, or sprint speed after a click of
    /// the movement stick, until the stick returns to centre. Direction is analog and head-relative
    /// (the base class's forward source is the camera). Walls: the rig's CharacterController slides
    /// along them (CMF-4). Speeds come from the comfort preset through ComfortSettings, never from stats.
    /// </summary>
    [AddComponentMenu("SLA/Quantized Move Provider")]
    public class QuantizedMoveProvider : ContinuousMoveProvider
    {
        private const string LeftStickClick = "<XRController>{LeftHand}/{Primary2DAxisClick}";
        private const string RightStickClick = "<XRController>{RightHand}/{Primary2DAxisClick}";

        [SerializeField] private XRInputButtonReader _leftHandSprintInput = StickClick("Left Hand Sprint", LeftStickClick);
        [SerializeField] private XRInputButtonReader _rightHandSprintInput = StickClick("Right Hand Sprint", RightStickClick);

        private readonly SprintToggle _sprint = new SprintToggle();
        private float _walkMetersPerSecond;
        private float _sprintMetersPerSecond;
        private bool _sprintOnLeftHand = true;

        public bool IsSprinting => _sprint.IsSprinting;

        /// <summary>The click that toggles sprint is on the movement hand (left unless left-handed, CMF-8).</summary>
        public bool SprintOnLeftHand
        {
            get => _sprintOnLeftHand;
            set
            {
                if (_sprintOnLeftHand != value)
                {
                    _sprintOnLeftHand = value;
                    _sprint.Reset();
                }
            }
        }

        public void SetSpeeds(float walkMetersPerSecond, float sprintMetersPerSecond)
        {
            _walkMetersPerSecond = walkMetersPerSecond;
            _sprintMetersPerSecond = sprintMetersPerSecond;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _leftHandSprintInput.EnableDirectActionIfModeUsed();
            _rightHandSprintInput.EnableDirectActionIfModeUsed();
            _sprint.Reset();
        }

        protected override void OnDisable()
        {
            // XRI only ends a move from Update, so a provider switched off mid-move (a lock, a preset change) would
            // stay "moving" and keep the vignette up.
            TryEndLocomotion();
            base.OnDisable();
            _leftHandSprintInput.DisableDirectActionIfModeUsed();
            _rightHandSprintInput.DisableDirectActionIfModeUsed();
            _sprint.Reset();
        }

        protected override Vector3 ComputeDesiredMove(Vector2 input)
        {
            var sprintInput = _sprintOnLeftHand ? _leftHandSprintInput : _rightHandSprintInput;
            bool sprinting = _sprint.Update(sprintInput.ReadWasPerformedThisFrame(), QuantizedMove.IsPastDeadZone(input));

            // The base class scales a unit-length input by moveSpeed, so speed is exactly walk or sprint.
            moveSpeed = QuantizedMove.SpeedMetersPerSecond(input, sprinting, _walkMetersPerSecond, _sprintMetersPerSecond);
            return base.ComputeDesiredMove(QuantizedMove.Direction(input));
        }

        private static XRInputButtonReader StickClick(string name, string bindingPath)
        {
            var reader = new XRInputButtonReader(name, null, false, XRInputButtonReader.InputSourceMode.InputAction);
            reader.inputActionPerformed.AddBinding(bindingPath);
            reader.inputActionValue.AddBinding(bindingPath);
            return reader;
        }
    }
}
