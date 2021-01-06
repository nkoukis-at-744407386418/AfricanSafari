using Unity.RemoteConfig;

using UnityEngine;

using VrGamesDev.Logs;

/// <summary>
/// The main namespace that holds the base class, all the packages inherit from
/// the class <a href="index.html#VrGamesDev.VRG_Base">VRG_Base</a>.
///
/// It also contains general purpose utility functions to handle and mofidy objects
/// or simple process like destroy, unparent or rotate.
/// </summary>
namespace VrGamesDev
{
    /// <summary>
    /// Mother class of all VrGamesDev framework, it contains all the methods
    /// commonto every class, it control the flow for variables and the diferent
    /// modules for internal comunication.
    /// 
    /// it also is useful for debug and verbose control
    /// </summary>
    public class VRG_Base : MonoBehaviour
    {
        /// <summary>
        /// <p>Level of verbose, there are 7 level of verbose from silence up to full debug</p>
        /// <p>ENUM_Verbose.NONE = Silence, none is sent to the editor log window,make sure you set all the classes on the release version of the game</p>
        /// <p>ENUM_Verbose.<font color=red>ERROR</font> = When you need to show an error to the user and logs use this level of verbosing, it is for logical errors, or inconsistencies</p>
        /// <p>ENUM_Verbose.<font color=yellow>WARNING</font> = Something was cheesy, and you need to show a warning if something is wacky, or strange, or unexpected, but still usable</p>
        /// <p>ENUM_Verbose.<font color=orange>LOGS</font> = For general display to track some basic information to other members of your team and follow the basic flow of your game</p>
        /// <p>ENUM_Verbose.<font color=green>INFO</font> = More information, ins and outs, flow of the game and useful for debug using asynchronous activities</p>
        /// <p>ENUM_Verbose.<font color=blue>STATUS</font> = Very verbose, I recommend to use this level of verbosing to indicate you changed status in a class, about vars or methods</p>
        /// <p>ENUM_Verbose.<font color=cyan>DEBUG</font> = The most verbose and slow, it basically shows EVERYTHING!!!</p>
        /// </summary>
        [Tooltip("Level of verbose, NONE for silence")]
        [SerializeField] protected ENUM_Verbose m_Verbose = ENUM_Verbose.WARNING;

        /// <summary>
        /// The available status, ask for this property to know if it is available to use
        /// </summary>
        [Tooltip("The available status, ask for this property to know if it is available")]
        [SerializeField] protected bool m_IsReady = false;
        /// <summary>
        /// Public GET available status, ask for this property to know if it is available to use
        /// </summary>
        public bool isReady { get { return this.m_IsReady; } }

        /// <summary>
        /// It sets the variable <em>isReady</em> property to TRUE, all the VrGamesDev inheritance from this
        /// </summary>
        protected void Start()
        {

            /*
            #if VRGAMESDEV_LOGS
                print("WOW = VRGAMESDEV_LOGS");
            #else
                print("Pus no, no ta");
            #endif

            #if LITE
                  Debug.Log("**** LITE version ****");
            #endif
            */

            // by default it is ready, polimorph as needed
            this.SetReady(true);
        }

        /// <summary>
        /// Public method to change the <em>isReady</em> property
        /// </summary>
        /// <param name="valueLocal">Boolean, it is assigned to the <em>isReady</em> property, it will inform the object is no longer ready or it is </param>
        public virtual void SetReady(bool valueLocal)
        {
            this.m_IsReady = valueLocal;
        }

        /// <summary>
        /// Overload single, if no parameters are given, it will show the full name of the route of the object that holds this class
        /// </summary>
        protected void WAS_echo()
        {
            WAS_FullName();
        }

        /// <summary>
        /// Overload without ENUM_Verbose, it uses for default ENUM_Verbose.INFO to display information
        /// </summary>
        /// <param name="valueLocal">The string message to send to UNITY editor</param>
        protected void WAS_echo(string valueLocal)
        {
            WAS_echo(valueLocal, ENUM_Verbose.INFO);
        }

        /// <summary>
        /// Overload display, it accepts a ENUM_VerboseLocal to custom the level of display
        /// </summary>
        /// <param name="valueLocal">The string message to send to UNITY editor</param>
        /// <param name="ENUM_VerboseLocal">Custom Verbose level, by default is ENUM_VERBOSE.INFO</param>
        protected void WAS_echo(string valueLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            // Solo desplegarlo si pasa el nivel de verbosing
            if (this.m_Verbose >= ENUM_VerboseLocal)
            {
                // Se manda el print con todo el formato de WAS_echo
                print("<color=" + VRG_DataBase.GetEnumColor(ENUM_VerboseLocal) + "><b>" + Time.timeSinceLevelLoad.ToString("F2") + ") " + this.transform.name + ":</b> </color> " + valueLocal); ;
            }
        }

        /// <summary>
        /// Warning en amarillo ignora el verbose
        /// </summary>
        /// <param name="valueLocal">The string message to send to UNITY editor</param>
        protected void WAS_Warning(string valueLocal)
        {
            // Se manda el print con todo el formato de WAS_echo
            print("<color=yellow><b>WARNING:</b></color> " + valueLocal
                + "\n <b>Class: </b> " + this.GetType()
                + "\n <b>Object: </b> " + this.GetParentRoute() + this.transform.name);
        }


        /// <summary>
        /// This method send a error to the unity editor, use it only when you know something will crash the game
        /// </summary>
        /// <param name="valueLocal"></param>
        protected void WAS_Error(string valueLocal)
        {
            // Se manda el print con todo el formato de WAS_echo
            UnityEngine.Debug.LogError(
                "<color=red><b>ERROR:</b></color> " + valueLocal
                + "\n <b>Class: </b> " + this.GetType()
                + "\n <b>Object: </b> " + this.GetParentRoute() + this.transform.name);
        }

        /// <summary>
        /// Overload de Full name without Params by default it provides "Full Path" of the object calling it
        /// </summary>
        protected void WAS_FullName()
        {
            WAS_FullName("Full Path");
        }

        /// <summary>
        /// Overload de Full name it provides "Full Path" of the object calling it and the <em>valueLocal</em> message
        /// </summary>
        /// <param name="valueLocal">The string message to send to UNITY editor</param>
        protected void WAS_FullName(string valueLocal)
        {
            WAS_FullName(valueLocal, ENUM_Verbose.NONE);
        }

        /// <summary>
        /// Overload de Full name it provides "Full Path" of the object calling it and the <em>valueLocal</em> message, only if the Verbose is equal or higher than the one from object
        /// </summary>
        /// <param name="valueLocal">The string message to send to UNITY editor</param>
        /// <param name="ENUM_VerboseLocal">Custom Verbose level, by default is ENUM_VERBOSE.INFO</param>
        protected void WAS_FullName(string valueLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            // obtengo la ruta hasta el padre
            string sMyParent = this.GetParentRoute();

            // Solo desplegarlo si pasa el nivel de verbosing
            if (this.m_Verbose >= ENUM_VerboseLocal)
            {
                // Se manda el print con todo el formato de WAS_echo
                print("<color=Cyan><b>" + valueLocal + ": </b></color> \n" + Time.timeSinceLevelLoad.ToString("F3") + ") " + this.GetType() + "\n" + sMyParent + this.transform.name);
            }
        }

        // Toda la ruta del padre hasta el root
        private string GetParentRoute()
        {
            // Variable para guardar la ruta del padre
            string sMyParent = "";

            // in case this is called in the OnDestroy();
            if (this)
            {
                // Si es un gameobject y tiene transform
                if (this.transform)
                {
                    // guardando el padre de este objeto
                    Transform MyFather = this.transform.parent;

                    // mientras tenga padres, seguirlo buscando
                    while (MyFather)
                    {
                        // si tengo padre guardarlo en la variable = MyFather
                        if (MyFather)
                        {
                            // Se guarda la variable recursiva para todos los padres
                            sMyParent = MyFather.name + " -> " + sMyParent;

                            // reasigno el papa del papa para seguir evaluando
                            MyFather = MyFather.transform.parent;
                        }
                    }
                }
            }

            // regreso el valor para su uso
            return sMyParent;
        }

        ///#IGNORE
        public virtual void OnOver() { }
        ///#IGNORE
        public virtual void OnOut() { }
        ///#IGNORE
        public virtual void OnClick() { }

        /// <summary>
        /// Delegated virtual function to wait and check for the remote settings and data
        /// </summary>
        /// <param name="configResponse">A regular <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.1/api/Unity.RemoteConfig.ConfigResponse.html">configResponse</a></param>
        protected virtual void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            /*
            protected override void ApplyRemoteSettings(ConfigResponse configResponse)
            {
                base.ApplyRemoteSettings(configResponse);
            }        
            // Add a listener to apply settings when successfully retrieved: 
            ConfigManager.FetchCompleted += ApplyRemoteSettings;
            */

            // LOGS: In case of debug log detail
            this.Logs
            (
                "configResponse.requestOrigin = <b>" + configResponse.requestOrigin.ToString() + "</b>",
                this.GetType() + "->ApplyRemoteSettings()",
                ENUM_Verbose.DEBUG
            );

            // Conditionally update settings, depending on the response's origin:
            switch (configResponse.requestOrigin)
            {
                case ConfigOrigin.Default:
                    this.RemoteSettings_Default();
                    break;

                case ConfigOrigin.Cached:
                    this.RemoteSettings_Cached();
                    break;

                case ConfigOrigin.Remote:
                    this.RemoteSettings_Remote();
                    break;
            }
        }

        /// <summary>
        /// When the ApplyRemoteSettings resolve the petition, the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.1/api/Unity.RemoteConfig.ConfigOrigin.html">Default option</a>
        /// </summary>
        protected virtual void RemoteSettings_Default() { }

        /// <summary>
        /// When the ApplyRemoteSettings resolve the petition, the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.1/api/Unity.RemoteConfig.ConfigOrigin.html">Cached option</a>
        /// </summary>
        protected virtual void RemoteSettings_Cached() { }

        /// <summary>
        /// When the ApplyRemoteSettings resolve the petition, the <a href="https://docs.unity3d.com/Packages/com.unity.remote-config@1.1/api/Unity.RemoteConfig.ConfigOrigin.html">Remote option</a>
        /// </summary>
        protected virtual void RemoteSettings_Remote() { }

        /// <summary>
        /// VRG_Logs wrapper, this method, send any message to the VRG_Logs singleton
        /// </summary>
        /// <param name="valueLocal">The message to log</param>
        /// <param name="sourceLocal">From where the messag was sent, Use the format <strong><em>Class->Method()</em></strong></param>
        /// <param name="ENUM_VerboseLocal">Custom Verbose level, to be logged it should be equal or higher than the one defined in the VRG_Remote</param>
        protected virtual void Logs(string valueLocal, string sourceLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            // call the print for editor
            WAS_echo(valueLocal, ENUM_VerboseLocal);

            // just save the logs if there is a route
            if (sourceLocal.Trim() != "")
            {
                // change the font for easier visualization in the editor
                valueLocal = valueLocal.Replace("<color", "<font color");
                valueLocal = valueLocal.Replace("</color>", "</font>");

                // if it is an error, turn the message red
                if (ENUM_VerboseLocal == ENUM_Verbose.ERROR)
                {
                    valueLocal = "<font color=red bgcolor=black>" + valueLocal + "</font>";
                }

                // if it is a WARNING, turn the message yellow
                if (ENUM_VerboseLocal == ENUM_Verbose.WARNING)
                {
                    valueLocal = "<font color=yellow bgcolor=black>" + valueLocal + "</font>";
                }

                // inform logs
                VRG_Logs.Do(valueLocal, sourceLocal, ENUM_VerboseLocal);

            }
        }
    }
}
