using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bpipe : MonoBehaviour
{
    Rigidbody2D rb;
    HingeJoint2D joint2D;
    // Start is called before the first frame update
    bool cut = true;
    public GameObject Place;//定位位置
    public List<GameObject> gameObjects = new List<GameObject>();//回收对象池
    int BulletTime = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint2D = GetComponent<HingeJoint2D>();
        JointMotor2D jointMotor2D = new JointMotor2D();
        jointMotor2D.motorSpeed = 50;
        jointMotor2D.maxMotorTorque = 10000;
        joint2D.motor = jointMotor2D;
    }
    void FixedUpdate()
    {
            #region 炮管旋转
            if (cut && transform.localEulerAngles.z < 30)
            {
                cut = false;
                JointMotor2D jointMotor2D = new JointMotor2D();
                jointMotor2D.motorSpeed = -50;
                jointMotor2D.maxMotorTorque = 10000;
                joint2D.motor = jointMotor2D;
            }
            if (!cut && transform.localEulerAngles.z > 150)
            {
                cut = true;
                JointMotor2D jointMotor2D = new JointMotor2D();
                jointMotor2D.motorSpeed = 50;
                jointMotor2D.maxMotorTorque = 10000;
                joint2D.motor = jointMotor2D;
            }
            #endregion

            if (BulletTime == 0)
            {
                GameObject gameObject1;
                if (gameObjects.Count > 0)
                {
                    gameObject1 = gameObjects[0];
                    gameObjects.Remove(gameObjects[0]);
                    gameObject1.SetActive(true);
                }
                else
                {
                    gameObject1 = Resources.Load("Prefabs/Bullet") as GameObject;
                    BaseHelper.AddChild(transform, gameObject1);
                }
                gameObject1.transform.localEulerAngles = Place.transform.localEulerAngles;
                gameObject1.transform.localPosition = Place.transform.localPosition;
                Rigidbody2D rigidbody2D = gameObject1.GetComponent<Rigidbody2D>();
                rigidbody2D = Place.GetComponent<Rigidbody2D>();
                BulletTime = 10;
            }
            else
                BulletTime--;
    }
}
