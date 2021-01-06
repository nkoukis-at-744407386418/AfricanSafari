using System.Collections;

using Unity.RemoteConfig;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class is the main loader it loads the VRG_core and the HAK systems
    /// it also waits if there is no internet until a conection is available
    /// </summary>
    public class VRG_Init : VRG_Base
    {
        [Tooltip("The VRG_Core class, this will be the addressable to update")]
        //[SerializeField]
        private string m_CoreClass = "VRG_Core";

        /// <summary>
        /// The delay wait for seconds to check for internet again
        /// </summary>
        [Header("From: Remote")]
        [Tooltip("The delay wait for seconds to check for internet again")]
        [SerializeField] private float m_CheckForInternetDelay = 1.0f;

        [Tooltip("The status of the internet, it will change TRUE when validated")]
        ///[SerializeField]
        private bool m_InternetStatus = false;



        /// <summary>
        /// UI component: NO internet message
        /// </summary>
        [Header("From: UI")]
        [Tooltip("UI component: NO internet message")]
        [SerializeField] private GameObject m_NoInternetBox = null;

        /// <summary>
        /// UI component: Status message, this display what is happening to the user
        /// </summary>
        [Tooltip("Status message, this display what is happening to the user")]
        [SerializeField] private Text m_Status = null;



        private new void Start()
        {
            // load parent start
            base.Start();

            // LOGS: we are starting the VRG_init
            this.Logs("Start", "VRG_Init->Start()", ENUM_Verbose.LOGS);

            // Add a listener to apply settings when successfully retrieved: 
            ConfigManager.FetchCompleted += ApplyRemoteSettings;

            // do not proceed until internet is detected
            this.CheckInternet();
        }

        // do not proceed until internet is detected
        private void CheckInternet()
        {
            // LOGS: we are checking... again
            this.Logs
            (
                "internetReachability = <b>" + Application.internetReachability.ToString() + "</b>",
                "VRG_Init->CheckInternet()",
                ENUM_Verbose.STATUS
            );

            // init the local variables
            string sReachabilityText = "";
            this.m_InternetStatus = false;

            //Check if the device cannot reach the internet
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //Not Reachable
                sReachabilityText = "No internet conection";
            }

            //Check if the device can reach the internet via a carrier data network
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                //Reachable via carrier data network.
                sReachabilityText = "Mobile network";
                this.m_InternetStatus = true;
            }

            //Check if the device can reach the internet via a LAN
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                //Reachable via Local Area Network;
                sReachabilityText = "Wifi";
                this.m_InternetStatus = true;
            }

            // if internet is available...
            if (this.m_InternetStatus)
            {
                // LOGS: now we have internet
                this.Logs
                (
                    "Internet = <b>" + sReachabilityText + "</b>",
                    "VRG_Init->CheckInternet()",
                    ENUM_Verbose.LOGS
                );

                // Fetch configuration setting from the remote service: 
                ConfigManager.FetchConfigs<VRG_Remote_UserAttributes, VRG_Remote_AppAttributes>(new VRG_Remote_UserAttributes(), new VRG_Remote_AppAttributes());                
            }

            // if not ...
            else
            {
                // check every seconds until a conection is found
                StartCoroutine(CheckInternet_IEnumerator());
            }
        }

        // enumerator to review if internet is available after 1 second
        private IEnumerator CheckInternet_IEnumerator()
        {
            // show No internet box message
            this.m_NoInternetBox.SetActive(true);

            // wait 1 second
            yield return new WaitForSeconds(this.m_CheckForInternetDelay);

            // check for internet again
            this.CheckInternet();
        }

        // virtual functioned fomr the apply settings
        protected override void RemoteSettings_Remote()
        {
            // just enter here if we are sure we have internet
            if (this.m_InternetStatus)
            {
                // turn off the dialog that inform no internet is available
                this.m_NoInternetBox.SetActive(false);

                // if the core class is not empty
                if (this.m_CoreClass.Trim() != "")
                {
                    // log I was here
                    this.Logs
                    (
                        "Setting: <b>" + this.m_CoreClass + "</b>",
                        "VRG_Init->RemoteSettings_Remote()",
                        ENUM_Verbose.STATUS
                    );

                    // Remove listener to apply settings since it was successfully retrieved
                    ConfigManager.FetchCompleted -= ApplyRemoteSettings;

                    // Instantiate the core class and on completed go to delegated
                    Addressables.InstantiateAsync(this.m_CoreClass).Completed += Core_Completed;
                }
            }
        }
        
        // delegated function to load the VRG_Core
        private void Core_Completed(AsyncOperationHandle<GameObject> obj)
        {
            // if the addressable was found and is a success
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                // log and inform i could load the Core
                this.Logs
                (
                    "<color=green>" + this.m_CoreClass + "</color> = <b>" + obj.Status + "</b>",
                    "VRG_Init->Core_Completed()",
                    ENUM_Verbose.LOGS
                );

                // Save the game object by the result
                GameObject go_Core = obj.Result;

                // delete the clone extention from the name
                go_Core.name = go_Core.name.ToString().Replace("(Clone)", "");

                // hide this module until the new scene
                this.gameObject.SetActive(false);
            }
            else
            {
                // inform I couldnt load the VRG_core Addressable
                this.Logs
                (
                    this.m_CoreClass + " = " + obj.Status,
                    "VRG_Init->Core_Completed()",
                    ENUM_Verbose.ERROR
                );

                // Display the No internet, to the user
                // si hubo un error, volver a cargar la pantalla de que se necesita internet
                this.m_NoInternetBox.SetActive(true);
            }
        }
    
        // VRG_Logs wrapper
        protected override void Logs(string valueLocal, string sourceLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            // I'm your father
            base.Logs(valueLocal, sourceLocal, ENUM_VerboseLocal);

            // if the status was declared 
            if (this.m_Status != null && (int)this.m_Verbose >= (int)ENUM_VerboseLocal)
            {
                // paint red if error
                if (ENUM_VerboseLocal == ENUM_Verbose.ERROR)
                {
                    valueLocal = "<color=red>" + valueLocal + "</color>";
                }
                // ... then copy the message to the user
                this.m_Status.text = valueLocal;
            }
        }
    }
}