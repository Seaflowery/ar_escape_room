using UnityEngine;

public class Drawer : MonoBehaviour
{
    [Header("Which transform moves (leave empty to move this object)")]
    [SerializeField] private Transform movingPart;

    [Header("Preset action = move by this local offset")]
    [SerializeField] private Vector3 localOpenOffset = new Vector3(0.25f, 0f, 0f);

    [SerializeField] private float moveSpeed = 0.6f;

    [Header("Behavior")]
    [SerializeField] private bool openOnly = true; // true: only opens once, false: toggles open/close

    private Vector3 _closedLocalPos;
    private Vector3 _openLocalPos;
    private Vector3 _targetLocalPos;
    private bool _isOpen;

    private void Awake()
    {
        if (movingPart == null) movingPart = transform;

        _closedLocalPos = movingPart.localPosition;
        _openLocalPos = _closedLocalPos + localOpenOffset;
        _targetLocalPos = _closedLocalPos;
        _isOpen = false;
    }

    private void Update()
    {
        if (movingPart.localPosition != _targetLocalPos)
        {
            movingPart.localPosition = Vector3.MoveTowards(
                movingPart.localPosition,
                _targetLocalPos,
                moveSpeed * Time.deltaTime
            );
        }
    }

    // Call this from Lever
    public void Activate()
    {
        if (openOnly) Open();
        else Toggle();
    }

    public void Open()
    {
        _isOpen = true;
        _targetLocalPos = _openLocalPos;
    }

    public void Close()
    {
        _isOpen = false;
        _targetLocalPos = _closedLocalPos;
    }

    public void Toggle()
    {
        if (_isOpen) Close();
        else Open();
    }
}
