using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerNetworkController : NetworkBehaviour
{

    public float walkSpeed = 4;
    public float runSpeed = 6;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float currentSpeedH;

    Transform cameraT;

    public GameObject bullet;
    public Transform turret;
    public Transform canon;

    // Use this for initialization
    void Start()
    {
        if (hasAuthority == false)
        {
            return;
        }
        cameraT = Camera.main.transform;
        CameraController cameraController = cameraT.GetComponent<CameraController>();
        cameraController.AssignTarget(transform);
    }


    // Update is called once per frame
    void Update()
    {
        if (hasAuthority == false)
        {
            return;
        }


        bool running = Input.GetKey(KeyCode.LeftShift);

        Vector2 input = new Vector2(0, Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;

        if (inputDir != Vector2.zero)
        {
            if (inputDir.y < 0)
            {
                targetRotation += 180;
            }
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }


        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        transform.Translate((inputDir.y * transform.forward) * currentSpeed * Time.deltaTime, Space.World);

        transform.Translate((Input.GetAxisRaw("Horizontal") * transform.right) * 3 * Time.deltaTime, Space.World);

        Debug.Log(cameraT.eulerAngles.y);
        turret.eulerAngles = new Vector3(turret.eulerAngles.x, cameraT.eulerAngles.y, turret.eulerAngles.z);
        canon.localEulerAngles = new Vector3(cameraT.eulerAngles.x, canon.localEulerAngles.y, canon.localEulerAngles.z);

        CmdUpdatePlayerPosition(transform.position, transform.eulerAngles, turret.eulerAngles, canon.localEulerAngles);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject remotePlayer = (GameObject)Instantiate(bullet, canon.transform.position, Quaternion.identity);
            BulletController bulletController = remotePlayer.GetComponent<BulletController>();
            bulletController.SetDirection(canon.forward);

            CmdSpawnBullet(canon.transform.position, canon.forward);
        }

    }

    private void GetCameraAimingDirection()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            print("Object: " + hit.transform.name);
        else
            print("Raycast hit nothing");
    }

    [Command]
    void CmdSpawnBullet(Vector3 p, Vector3 direction)
    {
        // Server Side
        RpcSpawnBullet(p, direction);
    }

    [ClientRpc]
    void RpcSpawnBullet(Vector3 p, Vector3 d)
    {
        // Client Side
        if (hasAuthority)
        {
            return;
        }
        GameObject remotePlayer = (GameObject)Instantiate(bullet, p, Quaternion.identity);
        BulletController bulletController = remotePlayer.GetComponent<BulletController>();
        bulletController.SetDirection(canon.forward);
    }


    [Command]
    void CmdUpdatePlayerPosition(Vector3 p, Vector3 r, Vector3 turretRotation, Vector3 cannonRotation)
    {
        // Server Side
        transform.position = p;
        transform.eulerAngles = r;
        turret.eulerAngles = turretRotation;
        canon.localEulerAngles = cannonRotation;

        RpcUpdatePlayerPosition(p, r, turretRotation, cannonRotation);
    }

    [ClientRpc]
    void RpcUpdatePlayerPosition(Vector3 p, Vector3 r, Vector3 turretRotation, Vector3 cannonRotation)
    {
        // Client Side

        if (hasAuthority)
        {
            return;
        }


        transform.position = p;
        transform.eulerAngles = r;
        turret.eulerAngles = turretRotation;
        canon.localEulerAngles = cannonRotation;

    }


}