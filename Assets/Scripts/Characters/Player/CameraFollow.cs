using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    [Header("Camera setting")]
    public float smoothSpeed = 5.0f; // ���ƽ���ƶ��ٶ�
    public float rotationSpeed = 5.0f; // �����ת�ƶ��ٶ�
    public float zoomSpeed = 2.0f; // ��������ٶ�
    public float minDistance = 2.0f; // ��ͷ�������
    public float maxDistance = 10.0f; // ��ͷ��Զ����
    public LayerMask obstacleLayer;

    private Vector3 initialOffset; // ��ʼƫ����
    private float pitch = 0.0f; // ������ (��X����ת)
    private float yaw = 0.0f;   // ƫ���� (��Y����ת)

    private void Start()
    {
        offset = transform.position - target.position;
        initialOffset = offset;
        yaw = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();

        ObstaclesCheck();

        // �����ӽ� (�м�����)
        if (Input.GetMouseButtonDown(2)) ResetCameraView();

        // ����Ŀ��λ�ò�ƽ�����ƶ�
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;

        transform.LookAt(target.position);
    }


    /// <summary>
    /// ���������ת
    /// </summary>
    private void HandleCameraRotation()
    {
        // ����Ҽ����������ת
        if (true)
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed; // ���ӽ�
            pitch = Mathf.Clamp(pitch, -90.0f, 90.0f);// ���Ƹ����Ƕ���(-90, 90)֮��

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            offset = rotation * initialOffset.normalized * offset.magnitude;
        }
    }

    /// <summary>
    /// ��������ӽ�
    /// </summary>
    private void ResetCameraView()
    {
        pitch = 0.0f;
        yaw = target.transform.eulerAngles.y; // ����yaw�Ƕȣ�ʹ���λ��Ŀ������
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        offset = rotation * initialOffset.normalized * offset.magnitude; // ���ֵ�ǰ����
    }

    /// <summary>
    /// �����̬����ϰ���
    /// </summary>
    private void ObstaclesCheck()
    {
        // TODO:�������ת���ϰ�������ʱ���Ϳ��������ϰ������������
        Vector3 direction = offset.normalized; // �������ƫ��������
        float currentDistance = offset.magnitude; // ��ǰ���ƫ�����Ĵ�С
        RaycastHit hit;

        // ���ֿ��������ӽǾ���
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float desiredDistance = offset.magnitude * (1 - scroll * zoomSpeed);
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance); // �������ž���

            // ���߼���ϰ����ֹ��������ϰ���
            Ray zoomRay = new Ray(transform.position, -direction);
            if (Physics.Raycast(zoomRay, out hit, desiredDistance, obstacleLayer))
            {
                // �����⵽�ϰ���������λ�õ��ϰ���֮ǰ
                desiredDistance = Mathf.Clamp(hit.distance, minDistance, desiredDistance);
            }
            offset = offset.normalized * desiredDistance; // ���� offset �ĳ���
        }

        Ray ray = new Ray(transform.position, direction); // ����������

        if (Physics.Raycast(ray, out hit, currentDistance, obstacleLayer))
        {
            // �����⵽�ϰ���������λ�õ��ϰ���֮ǰ
            currentDistance = Mathf.Clamp(hit.distance, minDistance, currentDistance);
            offset = direction * currentDistance;
        }
    }












    //��ʹ�ô����������������ӽ���ת������

    //private Vector3 offset;
    //private Transform playerTransform;

    //private float scroll;
    //private void Start()
    //{
    //    playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    //    offset = transform.position - playerTransform.position;
    //}
    //private void Update()
    //{
    //    transform.position = offset + playerTransform.position;
    //    scroll = Input.GetAxis("Mouse ScrollWheel");
    //    Camera.main.fieldOfView += scroll * 10;
    //    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 37, 70); // ����������ŵķ�Χ
    //}
}
