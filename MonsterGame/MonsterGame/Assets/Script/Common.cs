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
        public static void SceneJump(string SceneName)
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
