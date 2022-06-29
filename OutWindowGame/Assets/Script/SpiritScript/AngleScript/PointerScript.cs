using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerScript : MonoBehaviour
{
    public int m_Value = 0;
    private bool _Down = false;
    private Vector2 m_Position = new Vector2(-73, 8);
    private Vector2 m_Scale = new Vector2(-73,92);

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SetDisplayValue", 0.01f, 0.01f);
    }
    // Update is called once per frame
    void Update()
    {
    }
    void FixedUpdate()
    {

    }
    //根据角度改变
    void SetDisplayValue()
    {
        if (m_Value == -180)
            _Down = true;
        else if (m_Value == 0)
            _Down = false;
        if (_Down)
        {
            m_Value += 1;
        }
        else
        {
            m_Value -= 1;
        }
        transform.localPosition = ComPosition(m_Position,m_Value*-1,90);
        transform.localRotation = Quaternion.Euler(0, 0, m_Value);
    }
    /// <summary>
    /// 根据给出的初始位置计算旋转角度后的位置
    /// </summary>
    /// <param name="m_Position">圆点</param>
    /// <param name="Angle">角度</param>
    /// <param name="Radius">半径</param>
    /// <returns></returns>
    private Vector2 ComPosition(Vector2 m_Position, int Angle, float Radius)
    {
        Vector2 vector = new Vector2(0, 0);
        if (Angle == 90f)
        {

        }
        vector.x = m_Position.x - Radius * Mathf.Cos((Angle) * Mathf.PI / 180 / 2);
        //vector.y = m_Position.y + Radius * Mathf.Sin(Angle * Mathf.PI / 180 / 2);
        vector.y = m_Scale.y - Radius / m_Scale.y/2* Angle;
        return vector;
    }
}
