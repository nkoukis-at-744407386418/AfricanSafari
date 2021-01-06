using System.Collections;
using UnityEngine;



namespace VrGamesDev.DDuA
{

    /// <summary>
    /// This class simply allows the user to load
    /// new scene using Addressables, including itself
    /// </summary>
    public class VRG_LoadScene : VRG_Base
    {
        /// <summary>
        /// TRUE: Useful if you are running Coroutines, you allow the current frame cycle to finish
        /// FALSE: You will load the scene inmediatly
        /// </summary>
        [Tooltip("Useful if you are running Coroutines, you allow the current frame cycle to finish")]
        [SerializeField] private bool m_NextFrame = false;

        /// <summary>
        /// Time to delay (in seconds) the load of the new scene
        /// </summary>        [Tooltip("Time to delay (in seconds) the load of the new scene")]
        [SerializeField] private float m_Delay = 0.0f;

        /// <summary>
        /// The name of the new scene to load, [RELOAD SCENE] = reload Itself
        /// </summary>
        [Tooltip("The name of the new scene to load, [RELOAD SCENE] = reload Itself")]
        [SerializeField] [SceneName] private string m_Scene = "[RELOAD SCENE]";

        /// <summary>
        /// When the scene is fully loaded this is the array of Gameobjects to activate
        /// </summary>
        [Tooltip("When the scene is fully loaded this is the array of Gameobjects to activate")]
        [SerializeField] private string m_Carousel = "";



        private void OnEnable ()
        {
            // play the waits and delays
            StartCoroutine(OnEnable_IEnumerator());
        }

        // when activated Do your thing, load the scene
        private IEnumerator OnEnable_IEnumerator()
        {
            WAS_echo("Inside: Play"); // logs to editor 

            // if need to wait then...
            if (this.m_NextFrame)
            {
                // ... wait until next frame
                yield return null;
            }

            // if need to wait then...
            if (this.m_Delay > 0)
            {
                // ... wait the delay seconds needed
                yield return new WaitForSeconds(this.m_Delay);
            }

            // if it is not null
            if (this.m_Carousel.Trim() != "")
            {
                // save the Carousel name
                VRG_Session.String("VRG_Scene", "Carousel", this.m_Carousel);
            }

            // inform the VRG_Core of the new scene
            VRG_Core.Scene(this.m_Scene);

            // ... wait until next frame
            yield return null;
        }
    }
}