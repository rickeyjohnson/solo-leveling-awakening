using SLA.Core.Events;
using UnityEngine;

namespace SLA.Exercise
{
    /// <summary>
    /// Feeds the headset pose into a <see cref="SquatCounter"/> and raises OnRepCounted.
    /// Head height is measured relative to the XR Origin (the floor in a floor-level origin).
    /// </summary>
    public class SquatCounterDriver : MonoBehaviour
    {
        [Tooltip("The headset camera. Defaults to Camera.main.")]
        [SerializeField] private Transform _head;
        [Tooltip("The XR Origin (floor). Defaults to the head's root.")]
        [SerializeField] private Transform _floor;
        [SerializeField] private IntEventChannel _onRepCounted;
        [SerializeField] private float _calibrationSeconds = 2f;

        private readonly SquatCounter _counter = new SquatCounter();
        private float _calibrationStart = -1f;
        private float _heightSum;
        private int _heightSamples;
        private Vector3 _startSpot;

        public int Count => _counter.Count;
        public bool IsCalibrated => _counter.IsCalibrated;

        private void Awake()
        {
            if (_head == null && Camera.main != null)
            {
                _head = Camera.main.transform;
            }
            if (_floor == null && _head != null)
            {
                _floor = _head.root;
            }
        }

        /// <summary>Starts a 2-second standing-height calibration.</summary>
        public void BeginCalibration()
        {
            _calibrationStart = Time.time;
            _heightSum = 0f;
            _heightSamples = 0;
        }

        private void Update()
        {
            if (_head == null || _floor == null)
            {
                return;
            }

            float height = _head.position.y - _floor.position.y;

            if (_calibrationStart >= 0f)
            {
                _heightSum += height;
                _heightSamples++;
                if (Time.time - _calibrationStart >= _calibrationSeconds)
                {
                    _counter.Calibrate(_heightSum / Mathf.Max(1, _heightSamples), Time.time);
                    _startSpot = _head.position;
                    _calibrationStart = -1f;
                }
                return;
            }

            if (!_counter.IsCalibrated)
            {
                return;
            }

            float pitchDown = Vector3.Angle(Vector3.ProjectOnPlane(_head.forward, Vector3.up), _head.forward);
            if (_head.forward.y > 0f)
            {
                pitchDown = 0f;
            }

            Vector3 offset = _head.position - _startSpot;
            offset.y = 0f;

            if (_counter.Sample(Time.time, height, pitchDown, offset.magnitude))
            {
                _onRepCounted?.Raise(_counter.Count);
            }
        }
    }
}
