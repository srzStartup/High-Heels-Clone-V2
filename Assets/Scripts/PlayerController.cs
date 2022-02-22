using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform _ground;

    [SerializeField] private float _forwardSpeed;
    [SerializeField] private float _sidewardSpeed;

    private Transform _collector;
    private Camera _camera;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _collector = transform.Find("Collector");

        EventManager.HeelsHeightChanged += OnHeelsHeightChanged;
    }

    #region MonoBehaviour Methods
    private void Start()
    {
        Transform oneOfTheHeels = FindObjectOfType<HeelsManager>().heels[0].transform;
        StartOnMidPoint(oneOfTheHeels.GetComponent<Renderer>().bounds.size.y);
    }

    private void LateUpdate()
    {
        MoveLateral();
        Run();
    }

    #endregion

    #region Movement

    private void Run()
    {
        transform.Translate(0, 0, Time.deltaTime * _forwardSpeed);
    }

    private void MoveLateral()
    {
        _animator.SetFloat("_speed", _forwardSpeed);

        if (!Input.GetButton("Fire1")) return;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = _camera.transform.position.z;

        Ray ray = _camera.ScreenPointToRay(mousePosition);
        float maxDistance = _ground.GetComponent<Renderer>().bounds.size.z;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            Vector3 hitPoint = hit.point;
            hitPoint.y = transform.position.y;
            hitPoint.z = transform.position.z;

            transform.position = Vector3.MoveTowards(transform.position,
                                                     hitPoint,
                                                     Time.deltaTime * _sidewardSpeed);
        }
    }

    #endregion

    #region Event Listeners

    private void OnHeelsHeightChanged(object sender, HeelsHeightChangedEventArgs args)
    {
        float diff = args.diff;
        if (args.changeType.Equals(HeelsHeightChangeType.Decrease)) diff *= -1;

        transform.position = new Vector3(transform.position.x,
                                         _ground.position.y + diff,
                                         transform.position.z);

        _collector.GetComponent<BoxCollider>()
            .center = new Vector3(0, -diff, 0);
    }

    #endregion

    private void StartOnMidPoint(float level)
    {
        Bounds bounds = _ground.GetComponent<Renderer>().bounds;
        float midPointX = Calc.CalculateMidPoint(bounds, AxisType.X, transform.position.x);

        transform.position = new Vector3(
            midPointX,
            _ground.position.y + level,
            transform.position.z
        );
    }
}
