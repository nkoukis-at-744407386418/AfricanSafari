using System.Collections;

using UnityEngine;
namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class control the time to wait for seconds duration
    /// of the carousel and return to the main loop
    /// </summary>
    public class VRG_Carousel : VRG_Base
    {
        /// <summary>
        /// On TRUE: When the scene is ready, load instantly
        /// On FALSE: Load when the m_Delay is over
        /// </summary>
        [SerializeField] private bool m_LoadSceneWhenReady = false;

        /// <summary>
        /// The delay in seconds to wait before it try to load the scene
        /// 0 seconds is infinite delay time
        /// </summary>
        [Tooltip("From Remote: The setting that will decide if it is saved or not ")]
        [SerializeField] private float m_Delay = 3.0f;

        // on enable call a yield wait
        private void OnEnable()
        {
            StartCoroutine(this.OnEnable_IEnumerator());
        }

        // enumerator from enable
        private IEnumerator OnEnable_IEnumerator()
        {
            //if the scene is needed to be loaded
            VRG_Core.SetSceneAutoLoad(this.m_LoadSceneWhenReady);

            // if there is a delay
            if (this.m_Delay > 0)
            {
                // ... wait for it
                yield return new WaitForSeconds(this.m_Delay);

                // check the scene for a new carousel or load next scene if ready
                VRG_Core.CheckScene();

                // finish next frame
                yield return null;

                // turn off
                this.gameObject.SetActive(false);

            }

            // finish next frame
            yield return null;
        }
    }
}
