using Unity.RemoteConfig;

using UnityEngine;

namespace VrGamesDev
{

    /// <summary>
    /// This class control the remote settings, Local or from the unity cloud server
    /// You can override the remote data if you use the local arrays
    /// </summary>
    public class VRG_Remote : VRG_Base
    {
        /// <summary>
        /// LOCAL: the Bool array that override the remote settings
        /// </summary>
        [Header("From: Remote")]
        [Tooltip("LOCAL: the Bool data that override the remote settings")]
        [SerializeField] private VRG_RemoteBool[] m_Bools = null;

        /// <summary>
        /// LOCAL: the Int data that override the remote settings
        /// </summary>
        [Tooltip("LOCAL: the Int data that override the remote settings")]
        [SerializeField] private VRG_RemoteInt[] m_Ints = null;

        /// <summary>
        /// LOCAL: the String data that override the remote settings
        /// </summary>
        [Tooltip("LOCAL: the String data that override the remote settings")]
        [SerializeField] private VRG_RemoteString[] m_Strings = null;

        
        // singletoning Pattern
        public static VRG_Remote Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // ... and I will live forever
                DontDestroyOnLoad(this);
            }
            else
            {
                // I am not the one the prophecy said ... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }

        // this function returns the value of the setting searched, remote or local
        public static bool GetBool(string valueLocal)
        {
            // by default is false
            bool bRegresa = false;

            // if you find it in local, go to false
            bool bRemote = true;

            // search in our bool array
            foreach (VRG_RemoteBool child in Instance.m_Bools)
            {
                // if found
                if (child.name == valueLocal)
                {
                    // overwrite the remote
                    bRemote = false;

                    // return the value
                    bRegresa = child.value;

                    // since we found it, well... stop searching
                    break;
                }
            }

            // if it wasn't localed
            if (bRemote)
            {
                // ... get it from the Remote app server
                bRegresa = ConfigManager.appConfig.GetBool(valueLocal);
            }

            // return the value filled
            return bRegresa;
        }

        // this function returns the value of the setting searched, remote or local
        public static int GetInt(string valueLocal)
        {
            // by default is false
            int iRegresa = 0;

            // if you find it in local, go to false
            bool bRemote = true;

            // search in our bool array
            foreach (VRG_RemoteInt child in Instance.m_Ints)
            {
                // if found
                if (child.name == valueLocal)
                {
                    // overwrite the remote
                    bRemote = false;

                    // return the value
                    iRegresa = child.value;

                    // since we found it, well... stop searching
                    break;
                }
            }

            // if it wasn't localed
            if (bRemote)
            {
                // ... get it from the Remote app server
                iRegresa = ConfigManager.appConfig.GetInt(valueLocal);
            }

            // return the value filled
            return iRegresa;
        }

        // this function returns the value of the setting searched, remote or local
        public static string GetString(string valueLocal)
        {
            // by default is false
            string sRegresa = "";

            // if you find it in local, go to false
            bool bRemote = true;

            // search in our bool array
            foreach (VRG_RemoteString child in Instance.m_Strings)
            {
                // if found
                if (child.name == valueLocal)
                {
                    // overwrite the remote
                    bRemote = false;

                    // return the value
                    sRegresa = child.value;

                    // since we found it, well... stop searching
                    break;
                }
            }

            // if remote is our source
            if (bRemote)
            {
                // ... get it from the Remote app server
                sRegresa = ConfigManager.appConfig.GetString(valueLocal);
            }

            // return the value filled
            return sRegresa;
        }

    }
}