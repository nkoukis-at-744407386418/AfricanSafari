using UnityEngine;
using System.Collections;

namespace VrGamesDev
{
    /// <summary>
    /// This class allows to activate, deactivate, toogle or destroy many items of the scene
    /// it will be delayed for some seconds, instant or the next frame
    /// </summary>
    public class VRG_Delayed : VRG_Base
    {
        /// <summary>
        /// The time it will wait to perform the actions
        /// </summary>
        [Tooltip("The time it will wait to perform the actions")]
        [SerializeField] private float m_Delay = 0.0f;

        /// <summary>
        /// If it will wait until the next frame, useful if used on coroutines
        /// </summary>
        [Tooltip("If it will wait until the next frame, useful if used on coroutines")]
        [SerializeField] private bool m_NextFrame = false;

        /// <summary>
        /// If it will turn off after it's done.
        /// </summary>
        [Tooltip("If it will turn off after it's done.")]
        [SerializeField] private bool m_SelfTurnOff = false;

        /// <summary>
        /// Array of the transform to toogle, if on, then it will become off, and viceversa
        /// </summary>
        [Tooltip("Array of the transform to toogle, if on, then it will become off, and viceversa")]
        [SerializeField] private Transform[] m_Toogle = null;

        /// <summary>
        /// Array of the transform to activate <em>setActive(true)</em>
        /// </summary>
        [Tooltip("asd")]
        [SerializeField] private Transform[] m_Activate = null;

        /// <summary>
        /// Array of the transform to deactivate <em>setActive(false)</em>
        /// </summary>
        [Tooltip("asd")]
        [SerializeField] private Transform[] m_Deactivate = null;

        /// <summary>
        /// Array of the transform to destroy
        /// </summary>
        [Tooltip("asd")]
        [SerializeField] private Transform[] m_Destroy = null;


        // start the do "do your thing" thing
        private void OnEnable()
        {
            StartCoroutine(this.OnEnable_IEnumerator());
        }

        // Enumerator proxy, it is activated OnEnable
        private IEnumerator OnEnable_IEnumerator()
        {
            // if it will wit until this current frame is over
            if (this.m_NextFrame)
            {
                yield return null;
            }

            // wait for delay seconds
            if (this.m_Delay > 0)
            {
                yield return new WaitForSeconds(this.m_Delay);
            }

            // toogle for the next 
            foreach (Transform child in this.m_Toogle)
            {
                if (child == null)
                {
                    WAS_FullName("this.m_Toogle NULL");
                }
                else
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
            }

            // activate 
            foreach (Transform child in this.m_Activate)
            {
                if (child == null)
                {
                    WAS_FullName("this.m_Activate NULL");
                }
                else
                {
                    child.gameObject.SetActive(true);
                }
            }

            // deactivate
            foreach (Transform child in this.m_Deactivate)
            {
                if (child == null)
                {
                    WAS_FullName("this.m_Deactivate NULL");
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }

            // destroy
            foreach (Transform child in this.m_Destroy)
            {
                if (child == null)
                {
                    WAS_FullName("this.m_Destroy NULL");
                }
                else
                {
                    Object.Destroy(child.gameObject, 0.0f);
                }
            }

            // turn off self
            if (m_SelfTurnOff)
            {
                this.transform.gameObject.SetActive(false);
            }

            // finish the next frame
            yield return null;
        }
    }
}