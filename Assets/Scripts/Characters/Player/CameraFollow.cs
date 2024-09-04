using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    [Header("Camera setting")]
    public float smoothSpeed = 5.0f; // 相机平滑移动速度
    public float rotationSpeed = 5.0f; // 相机旋转移动速度
    public float zoomSpeed = 2.0f; // 相机缩放速度
    public float minDistance = 2.0f; // 镜头最近距离
    public float maxDistance = 10.0f; // 镜头最远距离
    public LayerMask obstacleLayer;

    private Vector3 initialOffset; // 初始偏移量
    private float pitch = 0.0f; // 俯仰角 (沿X轴旋转)
    private float yaw = 0.0f;   // 偏航角 (沿Y轴旋转)

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

        // 重置视角 (中键按下)
        if (Input.GetMouseButtonDown(2)) ResetCameraView();

        // 计算目标位置并平滑的移动
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;

        transform.LookAt(target.position);
    }


    /// <summary>
    /// 控制相机旋转
    /// </summary>
    private void HandleCameraRotation()
    {
        // 鼠标右键控制相机旋转
        if (true)
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed; // 俯视角
            pitch = Mathf.Clamp(pitch, -90.0f, 90.0f);// 限制俯仰角度在(-90, 90)之间

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            offset = rotation * initialOffset.normalized * offset.magnitude;
        }
    }

    /// <summary>
    /// 重置相机视角
    /// </summary>
    private void ResetCameraView()
    {
        pitch = 0.0f;
        yaw = target.transform.eulerAngles.y; // 重置yaw角度，使相机位于目标正后方
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        offset = rotation * initialOffset.normalized * offset.magnitude; // 保持当前距离
    }

    /// <summary>
    /// 相机动态检测障碍物
    /// </summary>
    private void ObstaclesCheck()
    {
        // TODO:当相机旋转到障碍物里面时，就可以无视障碍物进行缩放了
        Vector3 direction = offset.normalized; // 保留相机偏移量方向
        float currentDistance = offset.magnitude; // 当前相机偏移量的大小
        RaycastHit hit;

        // 滚轮控制缩放视角距离
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            float desiredDistance = offset.magnitude * (1 - scroll * zoomSpeed);
            desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance); // 限制缩放距离

            // 射线检测障碍物，防止相机穿过障碍物
            Ray zoomRay = new Ray(transform.position, -direction);
            if (Physics.Raycast(zoomRay, out hit, desiredDistance, obstacleLayer))
            {
                // 如果检测到障碍物，调整相机位置到障碍物之前
                desiredDistance = Mathf.Clamp(hit.distance, minDistance, desiredDistance);
            }
            offset = offset.normalized * desiredDistance; // 更新 offset 的长度
        }

        Ray ray = new Ray(transform.position, direction); // 相机看向玩家

        if (Physics.Raycast(ray, out hit, currentDistance, obstacleLayer))
        {
            // 如果检测到障碍物，调整相机位置到障碍物之前
            currentDistance = Mathf.Clamp(hit.distance, minDistance, currentDistance);
            offset = direction * currentDistance;
        }
    }












    //若使用代码控制相机，控制视角旋转不方便

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
    //    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 37, 70); // 设置相机缩放的范围
    //}
}
