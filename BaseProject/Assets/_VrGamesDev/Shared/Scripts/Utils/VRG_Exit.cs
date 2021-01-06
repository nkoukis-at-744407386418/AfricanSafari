using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// Exit the aplication
    /// </summary>
    public class VRG_Exit : VRG_Base
    {
        private new void Start()
        {
            // May the force be with you
            Application.Quit();
        }
    }
}