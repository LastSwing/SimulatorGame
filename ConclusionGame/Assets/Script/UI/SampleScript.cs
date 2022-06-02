using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleScript : MonoBehaviour
{
    public Livelibrary livelibrary;//事件
    public string EventName;//事件名称
    private Execute execute;//事件执行状态
    // Start is called before the first frame update
    void Start()
    {
        EventName = "测试事件1";
        TriggerEvent(EventName);
        /* 调用Thread测试            
        List<Fileid> files = new List<Fileid>();
        for (int i = 0; i < 4; i++)
        {
            Fileid fileid = new Fileid();
            fileid.Id = Guid.NewGuid();
            fileid.PathName = "测试"+i;
            files.Add(fileid);
        }
         NewTheard(files); */
        /* 调用Dialogue测试 
        if (livelibrary.Dialogue.TryGetValue(Guid.Empty, out Dictionary<Guid, string> value))
         {
             foreach (var item in value)
             {
                 NewDialog(item.Key,item.Value);
                 break;
             }
         }       */
        /*         调用Fileid调试
        Fileid fileid = new Fileid();
        foreach (var item in livelibrary.Fileid)
        {
            if (item.Fileidtype == FileidType.字段)
            {
                fileid = item;
                break;
            }
        }
        NewFileids(fileid);
        */

    }
    //Update is called once per frame
    void Update()
    {
        if (livelibrary.Name == null) return;
        //第一次执行
        if (execute == null)
        {
            execute = new Execute();
            if (livelibrary.Dialogue.TryGetValue(Guid.Empty, out Dictionary<Guid, string> Dialog))
            {
                foreach (var item in Dialog)
                {
                    execute.CGuid = Guid.Empty;
                    execute.Guid = item.Key;
                    execute.Update = false;
                    execute.Fileidtype = FileidType.对话;
                    execute.FormerlyID = new List<Guid>();
                    execute.FormerlyID.Add(item.Key);
                    execute.Repeat = false;
                    NewDialog(item.Key, item.Value);
                    break;
                }
            }
        }
        //上次一执行已完成
        else if (execute.Update)
        {
            List<Fileid> files = new List<Fileid>();
            execute.Update = false;
            #region 对话跳选择
            //查找是否有子选择
            for (int i = 0; i < livelibrary.Fileid.Count; i++)
            {
                //找到对话子选择
                if (livelibrary.Fileid[i].ParentId == execute.Guid && livelibrary.Fileid[i].Fileidtype != FileidType.判断对话)
                {
                    if (execute.FormerlyID.Contains(livelibrary.Fileid[i].Id)) continue;//已经执行过，跳过
                    execute.FormerlyID.Add(livelibrary.Fileid[i].Id);
                    files.Add(livelibrary.Fileid[i]);
                }
            }
            // 选择分支
            if (files.Count > 1)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    //对话中有多个执行分支
                    if (files[i].Fileidtype != FileidType.选择 || files[i].Fileidtype != FileidType.判断对话)
                    {
                        //先执行当前分支，其他分支退回
                        foreach (var item in files)
                        {
                            if (item.Id != files[i].Id)
                                execute.FormerlyID.Remove(item.Id);
                        }
                        execute.CGuid = execute.Guid;
                        execute.Fileidtype = files[i].Fileidtype;
                        execute.Guid = files[i].Id;
                        //执行分支
                        switch (files[i].Fileidtype)
                        {
                            case FileidType.动画:
                                NewAnimation();
                                break;
                            case FileidType.结局:
                                NewEnding();
                                break;
                            case FileidType.背景音乐:
                                NewBackground();
                                break;
                            case FileidType.字段:
                                NewFileids(files[i]);
                                break;
                        }
                        break;
                    }
                }
                execute.CGuid = files[0].ParentId;
                execute.Fileidtype = FileidType.选择;
                execute.Guid = files[0].Id;
                execute.Repeat = files[0].InsertByte == 0 ? true : false;
                NewTheard(files);
            }
            //其他分支
            else if (files.Count == 1)
            {
                execute.CGuid = execute.Guid;
                execute.Fileidtype = files[0].Fileidtype;
                execute.Guid = files[0].Id;
                //执行分支
                switch (files[0].Fileidtype)
                {
                    case FileidType.动画:
                        NewAnimation();
                        break;
                    case FileidType.结局:
                        NewEnding();
                        break;
                    case FileidType.背景音乐:
                        NewBackground();
                        break;
                    case FileidType.字段:
                        NewFileids(files[0]);
                        break;
                }
            }
            #endregion
            //没有分支，执行下一个对话
            else
            {
                bool State = false;//判断是否执行
                #region 选择跳对话
                foreach (var item in livelibrary.Fileid)
                {
                    //找到选择支
                    if (execute.Guid == item.Id)
                    {
                        //找到选择支下的子对话，播放
                        foreach (var items in livelibrary.Dialogue)
                        {
                            if (items.Key == execute.Guid)
                            {
                                execute.CGuid = items.Key;
                                foreach (var Dialog in items.Value)
                                {
                                    if (State) break;//已经执行对话支返回
                                    if (execute.FormerlyID.Contains(Dialog.Key)) continue;//已经播放的跳过
                                    execute.FormerlyID.Add(Dialog.Key);
                                    execute.Guid = Dialog.Key;
                                    State = true;
                                    NewDialog(Dialog.Key, Dialog.Value);//执行下一句
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 对话跳对话
                while (!State)
                {
                    //查找对话支
                    foreach (var item in livelibrary.Dialogue)
                    {
                        if (State) break;//已经执行对话支返回
                        //找到对话支
                        if (item.Key == execute.CGuid)
                        {
                            //找到对话
                            foreach (var Dialog in item.Value)
                            {
                                if (State) break;//已经执行对话支返回
                                if (execute.FormerlyID.Contains(Dialog.Key)) continue;//已经播放的跳过
                                execute.FormerlyID.Add(Dialog.Key);
                                execute.Guid = Dialog.Key;
                                State = true;
                                NewDialog(Dialog.Key, Dialog.Value);//执行下一句
                            }
                            //查询是否是重复项，是则跳回上级选项
                            if (!State)
                            {
                                Guid ParentId = Guid.Empty;
                                List<Fileid> fileids = new List<Fileid>();
                                foreach (var fileid in livelibrary.Fileid)
                                {
                                    if (fileid.Id == execute.CGuid)
                                    {
                                        ParentId = fileid.ParentId;
                                        break;
                                    }
                                }
                                foreach (var item1 in livelibrary.Fileid)
                                {
                                    if (item1.ParentId == ParentId)
                                    {
                                        if (execute.Fileidtype == FileidType.选择 && item1.InsertByte == 0)
                                        {
                                            fileids.Add(item1);
                                        }
                                    }
                                }
                                //执行跳回上级选择
                                if (fileids.Count > 1)
                                {
                                    string jihe = "";
                                    foreach (var file in fileids)
                                    {
                                        jihe += file.Id + "|";
                                    }
                                    //将已播放过的子对话释放
                                    foreach (var it in livelibrary.Dialogue)
                                    {
                                        if (jihe.Contains(it.Key.ToString()))
                                        {
                                            foreach (var value in it.Value)
                                                execute.FormerlyID.Remove(value.Key);
                                        }
                                    }
                                    execute.Fileidtype = FileidType.选择;
                                    execute.Guid = fileids[0].Id;
                                    execute.CGuid = fileids[0].ParentId;
                                    execute.Repeat = fileids[0].InsertByte == 0 ? true : false;
                                    State = true;
                                    NewTheard(fileids);
                                }
                            }
                            //对话集已没有其他对话，跳到上级选择去寻找对话
                            if (!State)
                            {
                                foreach (var fileid in livelibrary.Fileid)
                                {
                                    if (fileid.Id == execute.CGuid)
                                    {
                                        execute.CGuid = fileid.ParentId;
                                    }
                                }
                                foreach (var dia in livelibrary.Dialogue)
                                {
                                    foreach (var dia1 in dia.Value)
                                    {
                                        if (dia1.Key == execute.CGuid)
                                            execute.CGuid = dia.Key;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    void TriggerEvent(string eventName)
    {
        List<Event> events = JsonHelper.JsonToEvent();
        foreach (Event e in events)
        {
            if (e.EventName == eventName)
            {
                livelibrary = JsonHelper.JsonToLivelibrary(e.EventPath+".txt", Application.dataPath + "/Data");
            }
        }
    }

    #region 字段
    /// <summary>
    /// 字段对话
    /// </summary>
    /// <param name="fileid"></param>
    void NewFileids(Fileid fileid)
    {
        GameObject Fileid = Resources.Load("Prefab/Fileid") as GameObject;
        FileidScript fileidScript = Fileid.GetComponent<FileidScript>();
        //图像赋值
        fileidScript.Image.sprite = PersonImage.ImageToName("", "宋西楼", 450, 550);
        // 对话开始分割赋值
        string BenginText = "";
        BenginText = fileid.PathName.Substring(0, fileid.InsertByte);
        fileidScript.NewText.text = BaseHelper.EnterStr(BenginText, 10);
        //对话结束赋值
        string EndText = "";
        EndText = fileid.PathName.Substring(fileid.InsertByte, fileid.PathName.Length - fileid.InsertByte);
        fileidScript.EndText.text = BaseHelper.EnterStr(EndText, 6);
        //字段集产生的button
        List<Vector2> vector2s = BaseHelper.Meanlocation(730, 146, fileid.Fileids.Length);
        fileidScript.Text1.text = fileid.Fileids.Length > 0 ? fileid.Fileids[0] : "";
        fileidScript.Text2.text = fileid.Fileids.Length > 1 ? fileid.Fileids[1] : "";
        fileidScript.Text3.text = fileid.Fileids.Length > 2 ? fileid.Fileids[2] : "";
        fileidScript.Text4.text = fileid.Fileids.Length > 3 ? fileid.Fileids[3] : "";
        fileidScript.Text5.text = fileid.Fileids.Length > 4 ? fileid.Fileids[4] : "";
        fileidScript.NewText.name = fileid.Id.ToString();
        BaseHelper.AddChild(this.transform, Fileid);
        GameObject.Find($"Canvas/Panel/Fileid(Clone)/GameObject/Buttons/Button1").transform.localPosition = vector2s.Count > 0 ? vector2s[0] : new Vector2(-9999, 0);
        GameObject.Find($"Canvas/Panel/Fileid(Clone)/GameObject/Buttons/Button2").transform.localPosition = vector2s.Count > 1 ? vector2s[1] : new Vector2(-9999, 0);
        GameObject.Find($"Canvas/Panel/Fileid(Clone)/GameObject/Buttons/Button3").transform.localPosition = vector2s.Count > 2 ? vector2s[2] : new Vector2(-9999, 0);
        GameObject.Find($"Canvas/Panel/Fileid(Clone)/GameObject/Buttons/Button4").transform.localPosition = vector2s.Count > 3 ? vector2s[3] : new Vector2(-9999, 0);
        GameObject.Find($"Canvas/Panel/Fileid(Clone)/GameObject/Buttons/Button5").transform.localPosition = vector2s.Count > 4 ? vector2s[4] : new Vector2(-9999, 0);
    }

    void FileidClick(string fileid)
    {
        Debug.Log(fileid);
        foreach (var item in livelibrary.Fileid)
        {
            if (item.PathName == fileid && item.ParentId == execute.Guid)
            {
                execute.CGuid = item.Id;
            }
        }
        Destroy(GameObject.Find("Canvas/Panel/Fileid(Clone)"));
        execute.Update = true;
    }
    #endregion
    #region 对话
    /// <summary>
    /// 创建对话
    /// </summary>
    /// <param name="keyValues">Dictionary<></param>
    void NewDialog(Guid guid,string Name)
    {
        GameObject Dialog = Resources.Load("Prefab/Dialog") as GameObject;
        DialogScript dialogScript = Dialog.GetComponent<DialogScript>();
        dialogScript.Button.name = guid.ToString();
        dialogScript.image.sprite = PersonImage.ImageToName("",Name.Split('|')[0],360,480);
        dialogScript.Text.text = Name.Split('|')[1];
        BaseHelper.AddChild(this.transform, Dialog);
    }
    void DialogClick(Guid guid)
    {
        Debug.Log(guid.ToString());
        Destroy(GameObject.Find("Canvas/Panel/Dialog(Clone)"));
        execute.Update = true;
    }
    #endregion
    #region 选择
    /// <summary>
    /// 创建选择
    /// </summary>
    /// <param name="fileids"></param>
    void NewTheard(List<Fileid> fileids)
    {
        List<Vector2> vector2s = new List<Vector2>();
        vector2s = BaseHelper.Meanlocation(1920, 480, fileids.Count);
        GameObject Thread = Resources.Load("Prefab/ThreadView") as GameObject;
        ThreadScript threadScript = Thread.GetComponent<ThreadScript>();
        threadScript.Text0.text = fileids.Count > 0 ? BaseHelper.EnterStr(fileids[0].PathName, 7) : "";
        threadScript.Text1.text = fileids.Count > 1 ? BaseHelper.EnterStr(fileids[1].PathName, 7) : "";
        threadScript.Text2.text = fileids.Count > 2 ? BaseHelper.EnterStr(fileids[2].PathName, 7) : "";
        threadScript.Text3.text = fileids.Count > 3 ? BaseHelper.EnterStr(fileids[3].PathName, 7) : "";
        threadScript.Thread0.name = fileids.Count > 0 ? fileids[0].Id.ToString(): threadScript.Thread0.name;
        threadScript.Thread1.name = fileids.Count > 1 ? fileids[1].Id.ToString() : threadScript.Thread1.name;
        threadScript.Thread2.name = fileids.Count > 2 ? fileids[2].Id.ToString() : threadScript.Thread2.name;
        threadScript.Thread3.name = fileids.Count > 3 ? fileids[3].Id.ToString() : threadScript.Thread3.name;
        threadScript.Skip.name = Guid.Empty.ToString();
        BaseHelper.AddChild(this.transform, Thread);
        if (fileids.Count > 0)
        {
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/{fileids[0].Id}").transform.localPosition = vector2s.Count > 0 ? vector2s[0] : new Vector2(-9999, 0);
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/{Guid.Empty}").transform.localPosition = fileids[0].InsertByte == 0 ? new Vector2(807, -405) : new Vector2(-9999, 0);
        }
        else
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/Thread0").transform.localPosition = vector2s.Count > 0 ? vector2s[0] : new Vector2(-9999, 0);
        if (fileids.Count > 1)
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/{fileids[1].Id}").transform.localPosition = vector2s.Count > 1 ? vector2s[1] : new Vector2(-9999, 0);
        else
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/Thread1").transform.localPosition = vector2s.Count > 1 ? vector2s[1] : new Vector2(-9999, 0);
        if (fileids.Count > 2)
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/{fileids[2].Id}").transform.localPosition = vector2s.Count > 2 ? vector2s[2] : new Vector2(-9999, 0);
        else
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/Thread2").transform.localPosition = vector2s.Count > 2 ? vector2s[2] : new Vector2(-9999, 0);
        if (fileids.Count > 3)
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/{fileids[3].Id}").transform.localPosition = vector2s.Count > 3 ? vector2s[3] : new Vector2(-9999, 0);
        else
            GameObject.Find($"Canvas/Panel/ThreadView(Clone)/Thread3").transform.localPosition = vector2s.Count > 3 ? vector2s[3] : new Vector2(-9999, 0);
    }

    /// <summary>
    /// 选择点击事件
    /// </summary>
    /// <param name="guid"></param>
    void ThreadClick(Guid guid)
    {
        //退出
        if (guid == Guid.Empty)
        {
            execute.Repeat = false;
            execute.Guid = execute.CGuid;
            //退出时返回上一步ID
            foreach (var item in livelibrary.Dialogue)
            {
                foreach (var items in item.Value)
                {
                    if(execute.Guid == items.Key)
                        execute.CGuid = item.Key;
                }
            }
            //退出时将所有选择项纳入执行过ID
            foreach (var item in livelibrary.Fileid)
            {
                if (item.ParentId == execute.Guid && item.Fileidtype == FileidType.选择 && !execute.FormerlyID.Contains(item.Id))
                    execute.FormerlyID.Add(item.Id);
            }
        }
        else
            execute.Guid = guid;
        Debug.Log(guid.ToString());
        var aa = GameObject.Find($"Canvas/Panel/ThreadView(Clone)");
        if (aa != null)
            Destroy(aa);
        execute.Update = true;
    }
    #endregion
    #region 动画
    /// <summary>
    /// 动画
    /// </summary>
    void NewAnimation()
    { 
        execute.Update = true;
    }
    #endregion
    #region 结局
    void NewEnding()
    { 
        execute.Update = false;
        Debug.Log("结束");
    }
    #endregion
    #region 背景音乐
    /// <summary>
    /// 背景音乐
    /// </summary>
    void NewBackground()
    {
        execute.Update = true;
    }
    #endregion
}
