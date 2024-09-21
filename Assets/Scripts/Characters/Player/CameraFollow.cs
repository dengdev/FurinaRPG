using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private LayerMask obstacleLayer; // 障碍物图层
    private float cameraMoveSpeed = 3.0f; // 相机移动速度
    private float sensitivity = 3.0f; // 视角灵敏度
    private float zoomSpeed = 5.0f; // 相机缩放速度
    private float minDistance = 1.5f; // 镜头最近距离
    private float maxDistance = 8.5f; // 镜头最远距离

    private Transform _playerTarget;
    private Vector3 startOffset; // 相机初始偏移
    private float _currentOffsetDistance=2.0f; // 当前偏移距离
    private float _pitch = 30.0f; // 俯仰角 (初始值为30度)
    private float _yaw = 0.0f;   // 偏航角
    private float minOffsetDistance = 1.8f;

    private const float MinPitch = -10.0f;
    private const float MaxPitch = 88.0f;

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("未找到带有 'Player' 标签的游戏对象。");
            return;
        }

        _playerTarget = player.transform.Find("LookAtPoint");
        if (_playerTarget == null) {
            Debug.Log("玩家对象中未找到 'LookAtPoint' 节点。");
            return;
        }

        // 设置相机初始偏移位置
        UpdateStartOffset();
    }

    private void LateUpdate() {
        if (_playerTarget == null) {
            Debug.LogWarning("未找到玩家目标，无法进行相机跟随。");
            return;
        }
        if (Input.GetMouseButtonDown(2)) { ResetCameraView(); }

        HandleCameraRotation();
        HandleCameraObstacles();
        HandleCameraZoom();

        Vector3 desiredPosition = _playerTarget.position + startOffset;
        if (Vector3.Distance(desiredPosition, _playerTarget.position) < minOffsetDistance) {
            desiredPosition = _playerTarget.position + (desiredPosition - _playerTarget.position).normalized * minOffsetDistance;
        }

        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, cameraMoveSpeed * Time.deltaTime);
        transform.position = smoothPosition;

        transform.LookAt(_playerTarget.position);
    }

    private void HandleCameraRotation() {
        _yaw += Input.GetAxis("Mouse X") * sensitivity;
        _pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        _pitch = Mathf.Clamp(_pitch, MinPitch, MaxPitch);

        UpdateStartOffset();
    }


    private void ResetCameraView() {
        if (_playerTarget == null) return;

        _pitch = 30.0f;
        _yaw = _playerTarget.eulerAngles.y;
        UpdateStartOffset();
    }

    private void HandleCameraObstacles() {
        Vector3 direction = startOffset.normalized;
        RaycastHit hit;

        Ray ray = new Ray(_playerTarget.position, direction);
        if (Physics.Raycast(ray, out hit, _currentOffsetDistance, obstacleLayer)) {
            _currentOffsetDistance = Mathf.Clamp(hit.distance-0.5f, minDistance, maxDistance);
            startOffset = direction * _currentOffsetDistance;
        }
    }

    private void HandleCameraZoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f) {
            _currentOffsetDistance *= 1 - scroll * zoomSpeed;
            _currentOffsetDistance = Mathf.Clamp(_currentOffsetDistance, minDistance, maxDistance);
            UpdateStartOffset();
        }
    }

    private void UpdateStartOffset() {
        float pitchRad = _pitch * Mathf.Deg2Rad;
        startOffset = new Vector3(
            _currentOffsetDistance * Mathf.Sin(pitchRad),
            _currentOffsetDistance * Mathf.Sin(pitchRad),
            -_currentOffsetDistance * Mathf.Cos(pitchRad)
        );

        Quaternion rotation = Quaternion.Euler(0, _yaw, 0);
        startOffset = rotation * startOffset;
    }
}
