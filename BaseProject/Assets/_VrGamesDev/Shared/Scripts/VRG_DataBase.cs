using UnityEngine;

namespace VrGamesDev
{
    /// ///#IGNORE
    public class SceneNameAttribute : PropertyAttribute { }

    /// ///#IGNORE
    public struct VRG_Remote_UserAttributes { }

    /// ///#IGNORE
    public struct VRG_Remote_AppAttributes { }

    /// <summary>
    /// Struct that save the local Booleans settings for the VRG_Remote, just for testing,
    /// use the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.2/manual/index.html">Unity Remote Config</a> module
    /// </summary>
    [System.Serializable]
    public struct VRG_RemoteBool
    {
        public string name;
        public bool value;
    }

    /// <summary>
    /// Struct that save the local Integers settings for the VRG_Remote, just for testing,
    /// Use the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.2/manual/index.html">Unity Remote Config</a> module
    /// </summary>
    [System.Serializable]
    public struct VRG_RemoteInt
    {
        public string name;
        public int value;
    }

    /// <summary>
    /// Struct that save the local Strings settings for the VRG_Remote, just for testing,
    /// Use the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.2/manual/index.html">Unity Remote Config</a> module
    /// </summary>
    [System.Serializable]
    public struct VRG_RemoteString
    {
        public string name;
        public string value;
    }

    /// <summary>
    /// Level of verbose, there are 7 level of verbose from silence up to full debug
    /// </summary>
    public enum ENUM_Verbose
    {
        /// <summary>
        /// ENUM_Verbose.NONE = Silence, none is sent to the editor log window,make sure you set all the classes on the release version of the game
        /// </summary>
        NONE = 0,

        /// <summary>
        /// ENUM_Verbose.<font color = red > ERROR </font> = When you need to show an error to the user and logs use this level of verbosing, it is for logical errors, or inconsistencies
        /// </summary>
        ERROR = 1,

        /// <summary>
        /// ENUM_Verbose.<font color = yellow > WARNING </font> = Something was cheesy, and you need to show a warning if something is wacky, or strange, or unexpected, but still usable
        /// </summary>
        WARNING = 2,

        /// <summary>
        /// ENUM_Verbose.<font color = orange > LOGS </font> = For general display to track some basic information to other members of your team and follow the basic flow of your game
        /// </summary>
        LOGS = 3,

        /// <summary>
        /// ENUM_Verbose.<font color = green > INFO </font> = More information, ins and outs, flow of the game and useful for debug using asynchronous activities
        /// </summary>
        INFO = 4,

        /// <summary>
        /// ENUM_Verbose.<font color=blue>STATUS</font> = Very verbose, I recommend to use this level of verbosing to indicate you changed status in a class, about vars or methods
        /// </summary>
        STATUS = 5,

        /// <summary>
        /// ENUM_Verbose.<font color = cyan > DEBUG </font> = The most verbose and slow, it basically shows EVERYTHING!!!
        /// </summary>
        DEBUG = 6,

        /// <summary>
        /// Total elements of this ENUM, useful for IF's 
        /// </summary>
        COUNT
    };

    /// #IGNORE
    public class VRG_DataBase : VRG_Base
    {
        /*
        [Space(10)]
        [Header("From: Enums")]
        [Tooltip("La tienda donde se va a publicar")]
        [SerializeField] public ENUM_Store m_Store;
        */

        public static string[] m_VerboseColors = { "white", "red", "yellow", "orange", "green", "blue", "cyan" };


        // singletoning Pattern
        public static VRG_DataBase Instance; private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // ... and I will not get destroyed
                DontDestroyOnLoad(this);
            }
            else
            {
                // I am not the one... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }


        public static string GetEnumColor(ENUM_Verbose valueLocal)
        {
            if (m_VerboseColors[(int)(valueLocal)] != null)
            { 
                return m_VerboseColors[(int)(valueLocal)];
            }
            else
            { 
                return m_VerboseColors[0];
            }
        }
    }
}
 