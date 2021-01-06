using System;
using UnityEditor;
using UnityEngine;

namespace VrGamesDev
{
    /*
     * This class review the scenes loaded into the Build Settings
     * and display into an array to select for easier typing and 
     * less prone to mistakes
     */

    ///#IGNORE
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        // lets start in -1, in case the build settings are empty
        int m_SceneIndex = -1;

        // the names of all the scenes that will be displayed
        GUIContent[] m_SceneNames;

        // the path spliiter take the extension and the path out
        readonly string[] m_ScenePathSplitters = { "/", ".unity" };

        // it works when the OnGui event 
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // if it is empty...
            if (EditorBuildSettings.scenes.Length == 0)
            {
                // ..., well stop
                return;
            }

            // if the index is the one
            if (m_SceneIndex == -1)
            {
                // call the setup with SerializedProperty
                Setup(property);
            }

            // if my current index is the same as the previous
            int oldIndex = m_SceneIndex;

            // popup with all the scenes names ordered
            m_SceneIndex = EditorGUI.Popup(position, label, m_SceneIndex, m_SceneNames);

            // if it is diferent
            if (oldIndex != m_SceneIndex)
            {
                // the current selected
                property.stringValue = m_SceneNames[m_SceneIndex].text;
            }
        }

        // setup the property into the array
        void Setup(SerializedProperty property)
        {
            // the name of the scene analized
            string sSceneName;

            // local scenes according to the Build settings
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            // copy from the content
            m_SceneNames = new GUIContent[scenes.Length+1];

            // cycle through them to 
            for (int i = 0; i < m_SceneNames.Length-1; i++)
            {
                // get local path to split it into the subject name
                string path = scenes[i].path;

                // split it with the m_ScenePathSplitters
                string[] splitPath = path.Split(this.m_ScenePathSplitters, StringSplitOptions.RemoveEmptyEntries);

                // if the name exist
                if (splitPath.Length > 0)
                {
                    // asign it to the name splited without extension
                    sSceneName = splitPath[splitPath.Length - 1];
                }
                else
                {
                    // it is a deleted scene, 
                    sSceneName = "(Deleted Scene)";
                }

                // create into the gui and the names
                m_SceneNames[i] = new GUIContent(sSceneName);
            }

            // always add [RELOAD SCENE] as an option
            m_SceneNames[scenes.Length] = new GUIContent("[RELOAD SCENE]");

            // if there is a mistake and no scenes are added
            if (m_SceneNames.Length == 1)
            {
                // inform the player that no scenes are into the build settings
                m_SceneNames = new[] { new GUIContent("[No Scenes In Build Settings]") };
            }

            // look for the name searched to select it
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                // was it found?
                bool sceneNameFound = false;

                // cycle through the array of scenes
                for (int i = 0; i < m_SceneNames.Length; i++)
                {
                    // if it is the same
                    if (m_SceneNames[i].text == property.stringValue)
                    {
                        // select it
                        m_SceneIndex = i;

                        // because i found it
                        sceneNameFound = true;

                        // ... and stop the search, target found
                        break;
                    }
                }

                // if the scene is not found
                if (!sceneNameFound)
                {
                    // select the first one
                    m_SceneIndex = 0;
                }
            }
            else
            {
                // select the first one
                m_SceneIndex = 0;
            }

            // assign the string to the value of the scene selected.
            property.stringValue = m_SceneNames[m_SceneIndex].text;
        }
    }
}