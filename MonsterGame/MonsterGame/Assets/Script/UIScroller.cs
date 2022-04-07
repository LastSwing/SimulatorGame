using UnityEngine;
using System.Collections.Generic;

public class UIScroller : MonoBehaviour
{

    //Item的宽高
    public int cellWidth = 1080;
    public int cellHeight = 150;
    public GameObject itemPrefab;
    public RectTransform _content;
    private int _index = -1;
    private List<UIScrollIndex> _itemList;
    private int _dataCount;

    private Queue<UIScrollIndex> _unUsedQueue;  //将未显示出来的Item存入未使用队列里面，等待需要使用的时候直接取出

    void Start()
    {
        _itemList = new List<UIScrollIndex>();
        _unUsedQueue = new Queue<UIScrollIndex>();
        DataCount = 150;
        OnValueChange(SecretArea.LoadSecret);
    }

    public void OnValueChange(List<SecretClass> secrets)
    {
        int index = GetPosIndex();
        if (_index != index && index > -1)
        {
            _index = index;
            for (int i = _itemList.Count; i > 0; i--)
            {
                UIScrollIndex item = _itemList[i - 1];
                if (item.Index < index || (item.Index >= index + secrets.Count))
                {
                    _itemList.Remove(item);
                    _unUsedQueue.Enqueue(item);
                }
            }
            for (int i = _index; i < _index + secrets.Count; i++)
            {
                if (i < 0) continue;
                if (i > _dataCount - 1) continue;
                bool isOk = false;
                foreach (UIScrollIndex item in _itemList)
                {
                    if (item.Index == i) isOk = true;
                }
                if (isOk) continue;
                SecretClass s1 = secrets[i];
                SecretClass s2 = SecretArea.Upgrade(s1);
                CreateItem(i, s2.Name+"\n"+s2.Value);
            }
        }
    }

    private void CreateItem(int index,string text)
    {
        UIScrollIndex itemBase;
        if (_unUsedQueue.Count > 0)
        {
            itemBase = _unUsedQueue.Dequeue();
        }
        else
        {
            itemBase = GameTools.AddChild(_content, itemPrefab).GetComponent<UIScrollIndex>();
        }

        itemBase.Scroller = this;
        itemBase.Index = index;
        itemBase.text = text;
        _itemList.Add(itemBase);
    }

    private int GetPosIndex()
    {
        return Mathf.FloorToInt(_content.anchoredPosition.y / (cellHeight));
    }

    public Vector3 GetPosition(int i)
    {
        return new Vector3(0f, i * -(cellHeight), 0f);
    }

    public int DataCount
    {
        get { return _dataCount; }
        set
        {
            _dataCount = value;
            UpdateTotalWidth();
        }
    }

    private void UpdateTotalWidth()
    {
        _content.sizeDelta = new Vector2(_content.sizeDelta.x, cellHeight * _dataCount);
    }
}
public static class GameTools
{
    static public GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent, false);
            go.layer = parent.gameObject.layer;
        }
        return go;
    }
}