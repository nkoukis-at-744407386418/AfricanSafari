using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// It unparents the GameObject and makes the <em>m_Parent</em> gameObject the new parent
    /// </summary>
    public class VRG_Unparent : VRG_Base
    {
        /// <summary>
        /// The future parent when this object enables
        /// </summary>
        [Tooltip("The future parent when this object enables")]
        [SerializeField] protected GameObject m_Parent = null;

        
        void OnEnable()
        {
            if (this.m_Parent == null)
            {
                this.transform.SetParent(null);
            }
            else
            {
                this.transform.SetParent(this.m_Parent.transform);
            }
        }

    }
}
