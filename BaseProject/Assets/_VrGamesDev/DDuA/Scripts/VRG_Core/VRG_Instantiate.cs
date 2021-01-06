using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



namespace VrGamesDev.DDuA
{
    /// <summary>
    /// This class Instantiate addressables that are already loaded and downloaded
    /// It also provides the GameObject that was instantiated
    /// </summary>
    public class VRG_Instantiate : VRG_Base
    {
        [Header("From: VRG_Instantiate")]
        [Tooltip("The prefix that will be attached to every new object created")]
        //[SerializeField]
        private string m_AddressablePrefix = "";

        [Tooltip("The suffix that will be attached to every new object created")]
        //[SerializeField]
        private string m_AddressableSuffix = "";

        [Tooltip("The counter of the Addressable that is being Instantiated")]
        //[SerializeField]
        private int m_InstantiateCurrent = 0;

        [Tooltip("The counter of the Addressable that could be installed")]
        //[SerializeField]
        private int m_Installed = 0;

        [Tooltip("The counter of the Addressable that could NOT be installed")]
        //[SerializeField]
        private int m_NotInstalled = 0;

        [Tooltip("The counter of the total Addressables that will try to get installed")]
        //[SerializeField]
        private long m_InstantiateSize = 0;

        [Tooltip("The counter of the total Addressables that will try to get installed")]
        //[SerializeField]
        protected List<VRG_Addressable> m_Addressables = new List<VRG_Addressable>();

        [Tooltip("The counter of the total Addressables that will try to get installed")]
        //[SerializeField]
        protected List<GameObject> m_GameObjects = new List<GameObject>();


        // This instantiate all its array of m_Addressables
        protected IEnumerator InstantiateAll()
        {
            // If we have at least 1 m_Addressables to process
            if (this.m_Addressables.Count > 0)
            {
                // Init all local variables
                int i = 0;
                string sLogs = "";
                string sLogs1 = "";

                // Reset the members of the class
                this.m_Installed = 0;
                this.m_NotInstalled = 0;
                this.m_InstantiateSize = 0;

                // cycle through the m_Addressables
                for (i = 0; i < this.m_Addressables.Count; i++)
                {
                    // check if we need to create the instance
                    if (this.m_Addressables[i].m_Create)
                    {
                        // increase the size of the array that will instantiate
                        this.m_InstantiateSize++;

                        // Since it is already being created, it is set to false so it isn't created twice
                        this.m_Addressables[i].m_Create = false;

                        // get the size and status for the VRG_Logs
                        sLogs1 = sLogs1
                            + this.m_InstantiateSize + ".- "
                            + this.m_Addressables[i].m_Name
                            + "<b> (" + this.m_Addressables[i].m_Size.Result + ")</b><br>";
                    }

                    // if not, just remove it from the List
                    else
                    {
                        this.m_Addressables.RemoveAt(i);
                        i--;
                    }
                }

                // save to logs what happened here
                sLogs = "<color=green>Trying to install <b>" + this.m_Addressables.Count + "</b> assets</color>";
                VRG_Core.SetStatus(sLogs);
                this.Logs
                (
                    sLogs1 + sLogs,
                    "VRG_Instantiate->InstantiateAll()",
                    ENUM_Verbose.STATUS
                );

                // let´s start to download all the stuff, starting from the beggining.
                this.m_InstantiateCurrent = 0;

                // Set Progress in the UI Bar
                VRG_Core.SetProgress(this.m_InstantiateCurrent, (float)this.m_InstantiateSize);

                // the m_Addressable that is currently downloaded and instantiated.
                int iInstance = 0;

                // try to update the progress until all the m_Addressables are created
                while (this.m_InstantiateCurrent < this.m_Addressables.Count)
                {
                    // If the instance global is the same as the current one, proceed
                    if (iInstance == this.m_InstantiateCurrent)
                    {
                        // save the handle
                        this.m_Addressables[this.m_InstantiateCurrent].m_Progress = Addressables.InstantiateAsync(this.m_Addressables[this.m_InstantiateCurrent].m_Name);

                        // to check for completation
                        this.m_Addressables[this.m_InstantiateCurrent].m_Progress.Completed += GetInstantiate;

                        // try the next one
                        iInstance++;

                        // save what happened here in the logs
                        sLogs = "Installing " + this.m_InstantiateCurrent + " <color=green>"
                            + this.m_Addressables[this.m_InstantiateCurrent].m_Name
                            + "</color> = " + (this.m_Addressables[this.m_InstantiateCurrent].m_Progress.PercentComplete * 100).ToString("F2") + "%";
                        VRG_Core.SetStatus(sLogs);
                        this.Logs
                        (
                            sLogs,
                            "VRG_Instantiate->InstantiateAll()",
                            ENUM_Verbose.STATUS
                        );
                    }

                    // Inform the progress TO the UI
                    VRG_Core.SetProgress
                    (
                        (float)
                        (
                            (
                                (float)this.m_InstantiateCurrent
                                + (float)this.m_Addressables[this.m_InstantiateCurrent].m_Progress.PercentComplete
                            )
                            / (float)this.m_InstantiateSize
                        ),
                        (float)this.m_InstantiateSize
                    );

                    // ... continue next frame
                    yield return null;
                }

                // since it is over, make sure the UI inform it
                VRG_Core.SetProgress(1, 1, true);

                ENUM_Verbose installedVerbose = ENUM_Verbose.INFO;
                // if something went wrong
                if (this.m_NotInstalled > 0)
                {
                    // save to logs
                    sLogs = "<color=red>Failed to install " + this.m_NotInstalled + " assets, try again</color>";
                    installedVerbose = ENUM_Verbose.ERROR;
                }
                else
                {
                    // inform that everything went ok
                    sLogs = "<color=blue>" + this.m_Installed + " assets installed</color>";
                }

                // Save to logs what happened here
                VRG_Core.SetStatus(sLogs);
                this.Logs
                (
                    sLogs,
                    "VRG_Instantiate->InstantiateAll()",
                    installedVerbose
                );

                // clear the List, since everything was created
                this.m_Addressables.Clear();
            }

            // End next frame
            yield return null;
        }

        // This is a delegated Async that is fired when the Addressable is fully loaded
        protected void GetInstantiate(AsyncOperationHandle obj)
        {
            // since it is over and fully downloaded, we can start the next one
            this.m_InstantiateCurrent++;

            // cycle all the m_Addressables
            foreach (VRG_Addressable child in this.m_Addressables)
            {
                // if my child is the same as the one received
                if (obj.Equals(child.m_Progress))
                {
                    // if it could get downloaded with success
                    if (obj.Status == AsyncOperationStatus.Succeeded)
                    {
                        // increase the installed counter
                        this.m_Installed++;

                        // save this instance ...
                        GameObject mDownloaded = (GameObject)obj.Result;

                        // ... into the gameobjets Array
                        this.m_GameObjects.Add(mDownloaded);

                        // just remove the clone suffix
                        mDownloaded.name = mDownloaded.name.ToString().Replace("(Clone)", "");

                        // save it for the logs
                        string sName = mDownloaded.name;

                        // add the prefix and suffix
                        mDownloaded.name = this.m_AddressablePrefix + mDownloaded.name + this.m_AddressableSuffix;

                        // turn it off after it is finished
                        mDownloaded.SetActive(child.m_Active);


                        // inform what happened to the logs
                        this.Logs
                        (
                            "<color=green>" + sName + "</color> Loaded",
                            "VRG_Instantiate->GetInstantiate()",
                            ENUM_Verbose.INFO
                        );
                    }

                    // if not, then ...
                    else
                    {
                        // increase the not installed counter
                        this.m_NotInstalled++;

                        // it will used in 2 functions
                        string sLogs = "<color=red>" + child.m_Name + " = " + obj.Status + "</color>";

                        // inform to the logs
                        this.Logs
                        (
                            sLogs,
                            "VRG_Instantiate->GetInstantiate()",
                            ENUM_Verbose.ERROR
                        );

                        // ... and the player
                        VRG_Core.SetStatus(sLogs);
                    }

                    // anyway stop the search
                    break;
                }
            }
        }

        /// <summary>
        /// Retrieve the GameObject from the "valueLocal" addressable
        /// </summary>
        /// <param name="valueLocal">The address of the game object</param>
        /// <returns>A game object reference</returns>
        public GameObject GetGO(string valueLocal)
        {
            // in case the GameObject doesn't exist return null
            GameObject GO = null;

            // cycle the m_GameObjects to find the valueLocal
            foreach (GameObject child in this.m_GameObjects)
            {
                // if the child is the same
                if (child.name == valueLocal)
                {
                    // assign to the return
                    GO = child;

                    // ... and stop the search, we got what we needed
                    break;
                }
            }

            // return the game object
            return GO;
        }


    }
}