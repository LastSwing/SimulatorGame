using UnityEngine;
using UnityEngine.UI;

public class UIScrollIndex : MonoBehaviour
{
    public Text _textOld;
    public string text { get; set; }
    private UIScroller _scroller;
    private int _index;

    void Awake()
    {
    }

    void Start()
    {
        _textOld.text = text;
    }
     void Update()
    {
        
    }


    public int Index
    {
        get { return _index; }
        set
        {
            _index = value;
            transform.localPosition = _scroller.GetPosition(_index);
            gameObject.name = "Scroll" + (_index < 10 ? "0" + _index : _index.ToString());
        }
    }

    public UIScroller Scroller
    {
        set { _scroller = value; }
    }
}
