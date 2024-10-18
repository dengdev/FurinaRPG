using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private LayerMask obstacleLayer; // �ϰ���ͼ��
     private float cameraMoveSpeed = 10.0f; // ����ƶ��ٶ�
     private float sensitivity = 0.8f; // �ӽ�������
     private float zoomSpeed = 2.5f; // ��������ٶ�
     private float minDistance = 1.2f; // ��ͷ�������
     private float maxDistance = 10f; // ��ͷ��Զ����

    private Transform _playerTarget;
    private Vector3 startOffset; // �����ʼƫ��
    private float _currentOffsetDistance=2.0f; // ��ǰƫ�ƾ���
    private float _pitch = 30.0f; // ������ (��ʼֵΪ30��)
    private float _yaw = 0.0f;   // ƫ����
    private float minOffsetDistance = 1.8f;

    private const float MinPitch = -10.0f;
    private const float MaxPitch = 88.0f;

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.Log("δ�ҵ����� 'Player' ��ǩ����Ϸ����");
            return;
        }

        _playerTarget = player.transform.Find("LookAtPoint");
        if (_playerTarget == null) {
            Debug.Log("��Ҷ�����δ�ҵ� 'LookAtPoint' �ڵ㡣");
            return;
        }

        // ���������ʼƫ��λ��
        UpdateStartOffset();
    }

    private void LateUpdate() {
        if (_playerTarget == null) {
            Debug.LogWarning("δ�ҵ����Ŀ�꣬�޷�����������档");
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
