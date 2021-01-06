using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// This class makes a class not destroyable on load <a href="https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html"><em><strong>Object.DontDestroyOnLoad(this);</strong></em></a>
    /// </summary>
    public class VRG_DontDestroyOnLoad : VRG_Base
    {
        private new void Start()
        {
            // I'm your father
            base.Start();

            // ... and I will live forever
            DontDestroyOnLoad(this);
        }
    }
}