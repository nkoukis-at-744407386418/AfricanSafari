using System;
using System.Collections.Generic;
using System.IO;

using System.Diagnostics;

using Unity.RemoteConfig;

using UnityEngine;


/// <summary>
/// BEL Namespace, Beautiful Expert Logs, get the most useful and well documented logs to track bugs.You can custom:<br></br>
/// - The level of verbosity,
/// - Where and how they will be saved,
/// - and the Html visuals,<br></br>
/// By default is in a preconfigured folder. You can find the Logs, in the “[Your-project's-name] / LogsLocal / [Your-project's-name].html"
/// </summary>
namespace VrGamesDev.Logs
{
    /// <summary>
    /// This struct saves the logs events while the system is fully loaded
    /// </summary>
    [System.Serializable]
    public struct VRG_LogsBuffer
    {
        /// <summary>
        /// The timestamp when the logs tried to save the information, you configure it in the <a href="index.html#VrGamesDev.Logs.VRG_LogsBuffer.VRG_LogsBuffer(string,%20string,%20ENUM_Verbose)">Basic constructor</a>
        /// </summary>
        public string time;

        /// <summary>
        /// The value of the message, you configure it in the <a href="index.html#VrGamesDev.Logs.VRG_LogsBuffer.VRG_LogsBuffer(string,%20string,%20ENUM_Verbose)">Basic constructor</a>
        /// </summary>
        public string value;

        /// <summary>
        /// The level of verbosing, you configure it in the <a href="index.html#VrGamesDev.Logs.VRG_LogsBuffer.VRG_LogsBuffer(string,%20string,%20ENUM_Verbose)">Basic constructor</a>
        /// </summary>
        public ENUM_Verbose verbose;



        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="timeLocal"><a href="index.html#VrGamesDev.Logs.VRG_LogsBuffer.time">Class property this.time</a></param>
        /// <param name="valueLocal"><a href="index.html#VrGamesDev.Logs.VRG_LogsBuffer.value">Class property this.value</a></param>
        /// <param name="verboseLocal"><a href="index.html#VrGamesDev.Logs.VRG_LogsBuffer.verbose">Class property this.verbose</a></param>
        public VRG_LogsBuffer(string timeLocal, string valueLocal, ENUM_Verbose verboseLocal)
        {
            this.time = timeLocal;
            this.value = valueLocal;
            this.verbose = verboseLocal;
        }
    }

    /// <summary>
    /// This class save to a file in a folder the logs for easier
    /// debug it is controlled by the Remote settings to start it needs the
    /// VRG_Remote singleton
    /// </summary>
    public class VRG_Logs : VRG_Base
    {
        /// <summary>
        /// This methods will not be logged, they will be excluded
        /// </summary>
        [Tooltip("This methods will not be logged, they will be excluded")]
        [SerializeField] private string[] m_Excludes = { "Logs", "InvokeMoveNext" };

        [Header("From: Settings")]

        /// <summary>
        /// FROM VRG_Remote: The verbose Level; 0 is muted
        /// </summary>
        [Tooltip("FROM VRG_Remote: The verbose Level; 0 is muted")]
        //[SerializeField]
        private string m_SettingsVerbose = "VRG_Logs.verbose";

        /// <summary>
        /// FROM VRG_Remote: The name of the file log is the project name, The append mode is defined as.-
        /// 0 = It doesn’t append new logs, just one log is saved (the last one).
        /// 1 = It does append, every run is logged one after another. (it gets bulky after a while)
        /// 2 = The name of the file log is the project name with the datetime, so every run is in a diferent file. (you will get a lot of files quick)
        /// </summary>
        [Tooltip("FROM VRG_Remote: The append mode")]
        //[SerializeField]
        private string m_SettingsMode = "VRG_Logs.mode";

        /// <summary>
        /// FROM VRG_Remote: This is the folder where the log(s) files will be saved, by default is “LogsLocal”.
        /// </summary>
        [Tooltip("FROM VRG_Remote: This is the folder where the log(s) files will be saved")]
        //[SerializeField]
        private string m_SettingsDirectory = "VRG_Logs.directory";




        [Header("From: HTML")]

        /// <summary>
        /// Font default color
        /// </summary>
        [Tooltip("Font default color")]
        [SerializeField] private string m_ColorFont = "#000000";

        /// <summary>
        /// Row default color
        /// </summary>
        [Tooltip("Row default color")]
        [SerializeField] private string m_ColorDefault = "#FFFFFF";

        /// <summary>
        /// Row color to switch on update
        /// </summary>
        [Tooltip("Row color to switch on update")]
        [SerializeField] private string m_ColorSwitch = "#CDCDCD";


        /// <summary>
        /// The level of detail of the floating time
        /// </summary>
        [Tooltip("The level of detail of the floating time")]
        [SerializeField] private int m_FloatingPointDetail = 6;






        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]
        [Tooltip("Flag if it is fully inited")]
        //[SerializeField]
        private bool m_Inited = false;

        [Tooltip("The date time when the log started")]
        //[SerializeField]
        private string m_DateTime = "";

        [Tooltip("The date time when the log started")]
        //[SerializeField]
        private int m_Mode = 0;

        [Tooltip("This value is filled with m_SettingsDirectory from Remote")]
        //[SerializeField]
        private string m_DirectoryName = "Logs";

        [Tooltip("The stream buffer that save the log data while it is not inited")]
        //[SerializeField]
        private int m_CurrentRow = 0;

        [Tooltip("Latest update Time")]
        //[SerializeField]
        private string m_CurrentUpdateTime = "";

        [Tooltip("Latest update Color")]
        //[SerializeField]
        private string m_CurrentUpdateColor = "";

        [Tooltip("The stream buffer that save the log data while it is not inited")]
        //[SerializeField]
        private List<VRG_LogsBuffer> m_Buffer = new List<VRG_LogsBuffer>();

        [Tooltip("The Stream that save the file")]
        //[SerializeField]
        private StreamWriter m_OutputStream;


        /// <summary>
        /// Singleton pattern, Instance is the variable that save the data from every class.
        /// </summary>
        public static VRG_Logs Instance;

        private void Awake()
        {
            // I will check if I am the first singletong
            if (Instance == null)
            {
                // ... since i am the one, I declare myself as the one
                Instance = this;

                // ... and I will not get destroyed
                DontDestroyOnLoad(this);

                // Add a listener to apply settings when successfully retrieved: 
                ConfigManager.FetchCompleted += ApplyRemoteSettings;
            }
            else
            {
                // I am not the one... I will walk to the eternal darkness
                Destroy(this.gameObject);
            }
        }

        // close the stream and save the file
        private void OnDestroy()
        {
            // If the file was opened
            if (this.m_Verbose > ENUM_Verbose.NONE)
            {
                // close the table
                this.m_OutputStream.WriteLine
                (""
                    + "</table>"
                );

                // flush the stream to release it
                Instance.m_OutputStream.Flush();

                // close the file
                this.m_OutputStream.Close();

                // ... and destroy the stream
                this.m_OutputStream = null;
            }
        }

        // overrided method, called on the remote settings when it comes from the server
        protected override void RemoteSettings_Remote()
        {  
            // The verbose directory to save the Log files
            this.m_DirectoryName = VRG_Remote.GetString(this.m_SettingsDirectory);

            // The verbose setting, 0 is muted the higher the number the verboiser
            this.m_Verbose = (ENUM_Verbose)System.Enum.Parse(typeof(ENUM_Verbose), ((ENUM_Verbose)VRG_Remote.GetInt(this.m_SettingsVerbose)).ToString());

            // The mode setting, 0 not appended, 1 appended, 2 single file per log
            this.m_Mode = VRG_Remote.GetInt(this.m_SettingsMode);

            // if it is uninited, then procced and remove the listener
            if (!this.m_Inited)
            {
                // Remove the listener because it was successfully retrieved
                ConfigManager.FetchCompleted -= ApplyRemoteSettings;

                // init the log activity
                this.Init();
            }
        }

        // it will not start saving until it is properly inited
        private void Init()
        {
            // just init the stream if the remote config sets this to true
            if (this.m_Verbose > ENUM_Verbose.NONE && this.m_DirectoryName.Trim() != "")
            {
                // create the directory
                Directory.CreateDirectory(this.m_DirectoryName);

                // save the date time when this started
                this.m_DateTime = string.Format
                (
                    "{0:yyyy-MM-dd H:mm:ss}",
                    DateTime.Now
                );

                //this.m_DateTime = "2000-01-01 00:00:01";

                bool bAppend = true;
                string sDateTime = "";

                if (this.m_Mode <= 0)
                {
                    bAppend = false;
                }

                if (this.m_Mode >= 2)
                {
                    sDateTime = " - " + this.m_DateTime.Replace(":", "-");
                }

                // Open the log file to append the new log to it.
                this.m_OutputStream = new StreamWriter
                (
                    this.m_DirectoryName + "/" + Application.productName + sDateTime +".html",
                    bAppend
                );

                /*
                // Open the log file to append the new log to it.
                this.m_OutputStream = new StreamWriter
                (
                    string.Format
                    (
                        "{0} - {1:yyyy-MM-dd H-mm-ss}.html",
                        this.m_DirectoryName + "/" + Application.productName,
                        DateTime.Now
                    ),
                    true
                );
                */

                // verbose level in words

                string sVerboseInWords = ENUM_Verbose.DEBUG.ToString();

                if (this.m_Verbose < ENUM_Verbose.DEBUG)
                {
                    sVerboseInWords = this.m_Verbose.ToString();
                }

                // init the HTML table
                this.m_OutputStream.WriteLine
                (""
                    + "<table>"
                        + "<tr bgcolor=#AAAAAA><td colspan=5>"
                        + "<center><strong>"
                        + this.m_DateTime
                        + "</strong> - <font color=" + VRG_DataBase.GetEnumColor((ENUM_Verbose)System.Enum.Parse(typeof(ENUM_Verbose), sVerboseInWords)) +">"
                        + "VERBOSE LEVEL: "+ sVerboseInWords + "</center></font></td></tr>"
                        + "<tr bgcolor=black style=\"color:white\">"
                            + "<td width=40px><center><b>" + "Id" + "</b></center></td>"
                            + "<td width=90px><center><b>" + "Verbose" + "<b></center></td>"
                            + "<td width=130px><center><b>" + "TimeStamp" + "</b></center></td>"
                            + "<td width=320px><center><b>" + "Class" + "</b></center></td>"
                            + "<td width=450px><center><b>" + "Message" + "</b></center></td>"
                        + "</tr>"
                );

                // cycle through the buffer
                foreach (VRG_LogsBuffer child in this.m_Buffer)
                {
                    // the verbose is the one defined
                    if (child.verbose <= this.m_Verbose)
                    {
                        // if the time is new
                        if (child.time != Instance.m_CurrentUpdateTime)
                        {
                            // save the new time
                            Instance.m_CurrentUpdateTime = child.time;

                            // change the color
                            if (Instance.m_CurrentUpdateColor == Instance.m_ColorDefault)
                            {
                                // ... switch color
                                Instance.m_CurrentUpdateColor = Instance.m_ColorSwitch;
                            }
                            else
                            {
                                // ... default color
                                Instance.m_CurrentUpdateColor = Instance.m_ColorDefault;
                            }
                        }



                        // write the buffer if some logs where sent to the file before it was ready and inited
                        this.m_OutputStream.WriteLine(""
                            + "<tr bgcolor=" + Instance.m_CurrentUpdateColor + ">"
                            + "<td bgcolor=black><font color=white><center>" + Instance.m_CurrentRow++ +
                            child.value);
                    }
                }

                // Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.
                this.m_OutputStream.Flush();

                // Clears the buffer
                //this.m_Buffer. = "";
            }

            // Ok, officially inited
            this.m_Inited = true;
        }

        /// <summary>
        /// Do the work you are supposed to do, Save logs to the file, easy and clean 
        /// </summary>
        /// <param name="valueLocal">The string message to send to the log file</param>
        /// <param name="fromWhereLocal">Helps to understand who summon the log</param>
        /// <param name="ENUM_VerboseLocal">Custom Verbose level, the higher the less likely it will be to be saved</param>
        public static void Do(string valueLocal, string fromWhereLocal, ENUM_Verbose ENUM_VerboseLocal)
        {
            // if it is not properly inited, do nothing, remember this is a singleton
            if (Instance != null)
            {
                // check the verbose local against the Remote setting verbose
                if (Instance.m_Verbose >= ENUM_VerboseLocal || !Instance.m_Inited)
                {
                    // if you are running this from the editor, it will provide the line and class and file for easier debug
                    #if UNITY_EDITOR_OSX

                    bool bContinue;
                    for (int i = 1; i < 20; i++)
                    {
                        // the stackframe holds the info of how it is running 
                        StackFrame stackFrame = new StackFrame(i, true);

                        if (stackFrame.GetILOffset() != StackFrame.OFFSET_UNKNOWN && stackFrame.GetILOffset() > 0)
                        {
                            // get the info from the file, the method and the line number
                            string[] sWholeRoute = stackFrame.GetFileName().Split('/');
                            string[] sPunto = sWholeRoute[sWholeRoute.Length - 1].Split('.');
                            string sMethod = stackFrame.GetMethod().Name;
                            string sLineMethod = stackFrame.GetFileLineNumber().ToString();

                            // by default the first one is the one
                            bContinue = true;

                            // check if this method is excluded
                            foreach (string child in Instance.m_Excludes)
                            {
                                // if it is
                                if (sMethod == child)
                                {
                                    // exclude
                                    bContinue = false;

                                    // since it was found break and try next
                                    break;
                                }
                            }

                            // it was called from ...
                            if (bContinue)
                            {
                                // save the original sent
                                string sWhere = fromWhereLocal;

                                // get the stackframe origin
                                fromWhereLocal = sPunto[0] + "->" + sMethod + "(" + sLineMethod + ")";

                                // if it is different the found that the sent
                                if (sPunto[0] + "->" + sMethod != sWhere.Split('(')[0])
                                {
                                    // If it is from the class base
                                    if (sPunto[0] == "VRG_Base" && sMethod == "ApplyRemoteSettings")
                                    {
                                        // Repleace the namespace for easier reading format
                                        fromWhereLocal = sWhere.Replace("VrGamesDev.", "").Split('(')[0] + "(" + sLineMethod + ")";
                                    }

                                    // not the hak base
                                    else
                                    {
                                        // if it is an iterator
                                        if (sMethod == "MoveNext")
                                        {
                                            // use the sent
                                            fromWhereLocal = sWhere.Split('(')[0] + "(" + sLineMethod + ")";
                                        }
                                        else
                                        {
                                            // inform the disparity
                                            fromWhereLocal += "<font color=red> | " + sWhere + "</font>";
                                        }                                       
                                    }
                                }

                                // since i found a class, stop searching
                                break;
                            }
                        }
                        else
                        {
                            // null, so the search is done
                            break;
                        }
                    }
                    #endif

                    // format the time to compare and format the color rows
                    string sCurrentTime = Time.fixedUnscaledTime.ToString("F" + Instance.m_FloatingPointDetail.ToString());

                    // if the time is new
                    if (sCurrentTime != Instance.m_CurrentUpdateTime)
                    {
                        // save the new time
                        Instance.m_CurrentUpdateTime = sCurrentTime;

                        // change the color
                        if (Instance.m_CurrentUpdateColor == Instance.m_ColorDefault)
                        {
                            // ... switch color
                            Instance.m_CurrentUpdateColor = Instance.m_ColorSwitch;
                        }
                        else
                        {
                            // ... default color
                            Instance.m_CurrentUpdateColor = Instance.m_ColorDefault;
                        }
                    }

                    // if it is inited properly
                    if (Instance.m_Inited) 
                    {
                        // in case we are saving to the disk from Remote
                        if (Instance.m_Verbose > ENUM_Verbose.NONE)
                        {
                            // crate the row and column HTML and fill with the info provided
                            Instance.m_OutputStream.WriteLine(""
                        + "<tr bgcolor=" + Instance.m_CurrentUpdateColor + ">"
                            + "<td bgcolor=black><font color=white><center>" + Instance.m_CurrentRow++ + "</center></font></td>"
                            + "<td bgcolor=#AAAAAA><font color=" + VRG_DataBase.GetEnumColor(ENUM_VerboseLocal) + "><center>" + ENUM_VerboseLocal.ToString() + "</center></font></td>"
                            + "<td><font color=" + Instance.m_ColorFont + "><center><i> " + Instance.m_CurrentUpdateTime + "</i></center></font></td>"
                            + "<td><font color=" + Instance.m_ColorFont + ">" + fromWhereLocal + "</font></td>"
                            + "<td><font color=" + Instance.m_ColorFont + ">" + valueLocal + "</font></td>"
                        + "</tr>");

                            // ... and flush it
                            Instance.m_OutputStream.Flush();
                        }
                    }

                    // save it to buffer
                    else
                    {
                        // save the data to the Buffer in case we will change our mind or it is not ready
                        Instance.m_Buffer.Add(new VRG_LogsBuffer(Instance.m_CurrentUpdateTime, "</center></font></td>"
                            + "<td bgcolor=#AAAAAA><font color=" + VRG_DataBase.GetEnumColor(ENUM_VerboseLocal) + "><center>" + ENUM_VerboseLocal.ToString() + "</center></font></td>"
                            + "<td><font color=" + Instance.m_ColorFont + "><center><i> " + Instance.m_CurrentUpdateTime + "</i></center></font></td>"
                            + "<td><font color=" + Instance.m_ColorFont + ">" + fromWhereLocal + "</font></td>"
                            + "<td><font color=" + Instance.m_ColorFont + ">" + valueLocal + "</font></td>"
                        + "</tr>", ENUM_VerboseLocal));
                    }
                }
            }
        }
    }
}