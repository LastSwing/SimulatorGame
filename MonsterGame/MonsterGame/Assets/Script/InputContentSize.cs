using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputContentSize : MonoBehaviour
{
    //文本区
    public Text text;
    //输入框
    public InputField input;
    //内容区
    public RectTransform contents, ipt;

    private void FixedUpdate()
    {
        input = GameObject.Find("ipt_Atk").GetComponent<InputField>();
        contents = GameObject.Find("sv_Content").GetComponent<RectTransform>();
        ipt = GameObject.Find("ipt_Atk").GetComponent<RectTransform>();
        text = GameObject.Find("ipt_Atk/Text").GetComponent<Text>();
        //得到内容的大小
        Vector2 size = contents.sizeDelta;
        Vector2 size1 = ipt.sizeDelta;
        //获取行数，不要使用Text组件来分割，不知道什么原因无法完整分割。
        string[] texts = input.text.Split('\n');
        //设置高度：文字行数乘以文字大小，同样保留空白区
        size.y = (texts.Length + 3) * text.fontSize;
        size1.y = (texts.Length + 3) * text.fontSize;
        //以下是防止内容变小
        //判断当前高度是否小于原高度，如果小于的话则不设置
        if (size.y < 491f)
        {
            size.y = 491f;
            size1.y = 491f;
        }
        //赋值
        contents.sizeDelta = new Vector2(size.x, size.y);
        ipt.sizeDelta = new Vector2(size1.x, size1.y);
    }
    /// <summary>
    /// 数组中最大的值
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    private static int GetMax(int[] array)
    {
        int max = 0;
        for (int i = 0; i < array.Length; i++)
        {
            max = max > array[i] ? max : array[i];

        }
        return max;
    }

}
