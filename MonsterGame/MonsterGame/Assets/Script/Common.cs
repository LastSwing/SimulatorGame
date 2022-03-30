using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Script
{
    public class Common
    {
        /// <summary>
        /// 跳转页面
        /// </summary>
        /// <param name="SceneName">场景名称</param>
        public static void SceneJump(string SceneName)
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
