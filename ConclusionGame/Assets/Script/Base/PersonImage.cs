using System.Collections;
using System.IO;
using UnityEngine;

public static class PersonImage
{
    static Texture2D Dm_Tex;

    /// <summary>
    /// 返回角色对应图像或读取图像
    /// </summary>
    /// <param name="ImagePath">路径-不输入默认为角色路径</param>
    /// <param name="name">角色名称或图像名称</param>
    /// <param name="width">图片宽度</param>
    /// <param name="height">图片高度</param>
    /// <returns></returns>
    public static Sprite ImageToName(string ImagePath,string name,int width,int height)
    {
        if(ImagePath == "")
         ImagePath = Application.dataPath + @"\Resources\Image\Person";
        string ImageName = "";
        switch (name)
        {
            case "宋西楼":
                ImageName = "_nan.png";
                break;
            case "唐小月":
                ImageName = "_717.png";
                break;
            case "瀚宇星人":
                ImageName = "_nan1.png";
                break;
            case "冕星人":
                ImageName = "_nan1.png";
                break;
            case "锘星人":
                ImageName = "_nan1.png";
                break;
            default:
                ImageName = name;
                break;
        }
        LoadFromFile(width, height, ImagePath, ImageName);
        return Sprite.Create(Dm_Tex, new Rect(0, 0, Dm_Tex.width, Dm_Tex.height), new Vector2(0, 0));
    }

    private static void LoadFromFile(int width,int height, string path, string _name)
    {
        Dm_Tex = new Texture2D(width, height);
        Dm_Tex.LoadImage(ReadPNG(path +@"\"+ _name));
    }
    private static byte[] ReadPNG(string path)
    {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] binary = new byte[fileStream.Length];
        fileStream.Read(binary, 0, (int)fileStream.Length);

        fileStream.Close();

        fileStream.Dispose();

        fileStream = null;

        return binary;
    }
}