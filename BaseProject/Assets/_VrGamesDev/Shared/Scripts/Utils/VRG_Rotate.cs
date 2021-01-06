using UnityEngine;

namespace VrGamesDev
{
    /// <summary>
    /// Class that rotates a GameObject in X,Y,X, simple and effective
    /// 0 for no rotation
    /// Positive and negative values, will rotate
    /// </summary>
    public class VRG_Rotate : VRG_Base
    {
        /// <summary>
        /// If it needs to start random in X axis
        /// </summary>
        [Tooltip("If it needs to start random in X axis")]
        [SerializeField] private bool m_XRandom = false;

        /// <summary>
        /// If it needs to start random in Y axis
        /// </summary>
        [Tooltip("If it needs to start random in Y axis")]
        [SerializeField] private bool m_YRandom = false;

        /// <summary>
        /// If it needs to start random in Z axis
        /// </summary>
        [Tooltip("If it needs to start random in Z axis")]
        [SerializeField] private bool m_ZRandom = false;

        /// <summary>
        /// Rotation speed in X axis
        /// </summary>
        [Tooltip("Rotation speed in X axis")]
        [SerializeField] private float m_XSpeed = 100;

        /// <summary>
        /// Rotation speed in Y axis
        /// </summary>
        [Tooltip("Rotation speed in Y axis")]
        [SerializeField] private float m_YSpeed = 100;

        /// <summary>
        /// Rotation speed in Z axis
        /// </summary>
        [Tooltip("Rotation speed in Z axis")]
        [SerializeField] private float m_ZSpeed = 100;

        [Header("From: Debug - DO NOT EDIT -")]
        [Tooltip("Current coordinate in X axis")]
        private float m_X = 0.0f;

        [Tooltip("Current coordinate in Y axis")]
        private float m_Y = 0.0f;

        [Tooltip("Current coordinate in Z axis")]
        private float m_Z = 0.0f;


        private void OnEnable()
        {
            // Call father
            base.Start();

            // init the current coords
            this.m_X = this.transform.rotation.x;
            this.m_Y = this.transform.rotation.y;
            this.m_Z = this.transform.rotation.z;

            // decide if it has a random rotation start coordinate
            if (this.m_XRandom) this.m_X = Random.Range(0.0f, 359.9f);
            if (this.m_YRandom) this.m_Y = Random.Range(0.0f, 359.9f);
            if (this.m_ZRandom) this.m_Z = Random.Range(0.0f, 359.9f);
        }

        // rotate speed to each of the coordinates
        private void Update()
        {
            // if the X speed is different that 0
            if (this.m_XSpeed != 0)
            {
                // increment the speed over time
                this.m_X += (Time.deltaTime * this.m_XSpeed);

                // if it is over 360
                if (this.m_X >= 360.0f) this.m_X -= 360.0f;

                // if it is below 0
                if (this.m_X <= 0.0f) this.m_X += 360.0f;
            }

            // if the Y speed is different that 0
            if (this.m_YSpeed != 0)
            {
                // increment the speed over time
                this.m_Y += (Time.deltaTime * this.m_YSpeed);

                // if it is over 360
                if (this.m_Y >= 360.0f) this.m_Y -= 360.0f;

                // if it is below 0
                if (this.m_Y <= 0.0f) this.m_Y += 360.0f;
            }

            // if the Z speed is different that 0
            if (this.m_ZSpeed != 0)
            {
                // increment the speed over time
                this.m_Z += (Time.deltaTime * this.m_ZSpeed);

                // if it is over 360
                if (this.m_Z >= 360.0f) this.m_Z -= 360.0f;

                // if it is below 0
                if (this.m_Z <= 0.0f) this.m_Z += 360.0f;
            }

            // assign the new rotation
            this.transform.eulerAngles = new Vector3(this.m_X, this.m_Y, this.m_Z);
        }
    }
}