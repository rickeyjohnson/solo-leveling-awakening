using SLA.Combat;
using SLA.Data;
using UnityEngine;
using UnityEngine.AI;

namespace SLA.AI
{
    public enum EnemyState
    {
        Idle,
        Approach,
        Circle,
        Windup,
        Attack,
        Recover,
        Stagger,
        Dead,
    }

    /// <summary>
    /// The shared enemy state machine from the GDD's enemy AI diagram. A minimal C# version so
    /// enemies work on day one; richer behavior moves into Unity Behavior graphs later.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Health))]
    public class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private EnemyData _data;
        [SerializeField] private float _sightRange = 12f;

        private NavMeshAgent _agent;
        private Health _health;
        private Transform _player;
        private float _stateTimer;

        public EnemyState State { get; private set; } = EnemyState.Idle;
        public EnemyData Data => _data;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
            if (_data != null)
            {
                _health.Initialize(_data.maxHp, _data.armor);
                _agent.speed = _data.moveSpeed;
            }
        }

        private void OnEnable() => _health.Died += OnDied;

        private void OnDisable()
        {
            _health.Died -= OnDied;
            ReleaseToken();
        }

        private void Start()
        {
            var player = GameObject.FindWithTag("Player");
            _player = player != null ? player.transform : null;
        }

        private void Update()
        {
            if (State == EnemyState.Dead || _player == null || _data == null)
            {
                return;
            }

            if (_health.IsStaggered && State != EnemyState.Stagger)
            {
                Enter(EnemyState.Stagger);
            }

            _stateTimer += Time.deltaTime;
            float distance = Vector3.Distance(transform.position, _player.position);

            switch (State)
            {
                case EnemyState.Idle:
                    if (distance <= _sightRange) Enter(EnemyState.Approach);
                    break;

                case EnemyState.Approach:
                    _agent.SetDestination(_player.position);
                    if (distance <= _data.attackRange * 1.5f) Enter(EnemyState.Circle);
                    break;

                case EnemyState.Circle:
                    // TODO: strafe around the player at attack range.
                    _agent.SetDestination(_player.position);
                    if (distance <= _data.attackRange && TryGetToken()) Enter(EnemyState.Windup);
                    else if (distance > _data.attackRange * 2f) Enter(EnemyState.Approach);
                    break;

                case EnemyState.Windup:
                    // The tell: red weapon glow + sound cue start here (VFX/audio listen for state changes).
                    _agent.isStopped = true;
                    if (_stateTimer >= _data.windupSeconds) Enter(EnemyState.Attack);
                    break;

                case EnemyState.Attack:
                    // TODO: enable the weapon hitbox for the active frames.
                    if (_stateTimer >= 0.2f) Enter(EnemyState.Recover);
                    break;

                case EnemyState.Recover:
                    if (_stateTimer >= _data.recoverSeconds)
                    {
                        ReleaseToken();
                        _agent.isStopped = false;
                        Enter(EnemyState.Circle);
                    }
                    break;

                case EnemyState.Stagger:
                    _agent.isStopped = true;
                    if (!_health.IsStaggered)
                    {
                        _agent.isStopped = false;
                        Enter(EnemyState.Recover);
                    }
                    break;
            }
        }

        private void Enter(EnemyState next)
        {
            State = next;
            _stateTimer = 0f;
        }

        private bool TryGetToken()
        {
            var director = EnemyDirector.Current;
            return director == null || director.Tokens.TryAcquire(GetInstanceID());
        }

        private void ReleaseToken()
        {
            EnemyDirector.Current?.Tokens.Release(GetInstanceID());
        }

        private void OnDied()
        {
            ReleaseToken();
            Enter(EnemyState.Dead);
            if (_agent != null && _agent.isOnNavMesh)
            {
                _agent.isStopped = true;
            }
            // TODO: switch to ragdoll, drop loot, raise OnEnemyKilled.
        }
    }
}
