using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AtkDetailContentSize : MonoBehaviour
{
    //文本区
    public Text text;
    //输入框
    public InputField input;
    //内容区
    public RectTransform contents, ipt;

    private void FixedUpdate()
    {
        input = GameObject.Find("ipt_Detail").GetComponent<InputField>();
        contents = GameObject.Find("svc_Detail").GetComponent<RectTransform>();
        ipt = GameObject.Find("ipt_Detail").GetComponent<RectTransform>();
        text = GameObject.Find("ipt_Detail/Text").GetComponent<Text>();
        //得到内容的大小
        Vector2 size = contents.sizeDelta;
        Vector2 size1 = ipt.sizeDelta;
        //获取行数，不要使用Text组件来分割，不知道什么原因无法完整分割。
        string[] texts = input.text.Split('\n');
        //设置高度：文字行数乘以文字大小，同样保留空白区
        size.y = (texts.Length) * text.fontSize * 2;
        size1.y = (texts.Length) * text.fontSize  * 2;
        //以下是防止内容变小
        //判断当前高度是否小于原高度，如果小于的话则不设置
        if (size.y < 1670f)
        {
            size.y = 1670f;
            size1.y = 1670f;
        }
        //赋值
        contents.sizeDelta = new Vector2(size.x, size.y);
        ipt.sizeDelta = new Vector2(size1.x, size1.y);
    }
}
