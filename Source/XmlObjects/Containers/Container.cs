using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using XmlExtensions.Setting;

namespace XmlExtensions
{
    public class Container : ErrorHandler
    { // TODO: Add proper stack traces

        protected bool initialized = false;
        public string modId;

        protected bool allowSearch = true;
        protected List<string> searchTags;

        protected Color? color = null;

        protected SettingsMenuDef menuDef;

        protected internal bool filtered = false;

        internal bool PreOpenContainer()
        {
            filtered = false;
            return PreOpen();
        }

        internal bool PostCloseContainer()
        {
            filtered = false;
            return PostClose();
        }

        /// <summary>
        /// This method gets called right when the user open the settings menu
        /// </summary>
        /// <returns></returns>
        internal virtual bool PreOpen()
        {
            return true;
        }

        /// <summary>
        /// This method gets called right when the user closes the settings menu
        /// </summary>
        /// <returns></returns>
        internal virtual bool PostClose()
        {
            return true;
        }

        /// <summary>
        /// This method will be run exactly one time after the game finishes booting
        /// You may run any initialization or pre-computation code here
        /// </summary>
        /// <returns>Return <c>false</c> if there was an error, <c>true</c> otherwise.</returns>
        protected virtual bool Init()
        {
            return true;
        }

        public virtual bool Initialize(SettingsMenuDef menuDef)
        {
            if (!initialized)
            {
                initialized = true;
                this.menuDef ??= menuDef;
                modId = menuDef.modId;
                try
                {
                    return Init();
                }
                catch (Exception e)
                {
                    Error("Failed to initialize:\n" + e.Message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Applies the <c>Init()</c> method on every container in the list, error handling done automatically<br/>If the name of the list is provided, it will be used for error reporting
        /// </summary>
        /// <param name="menuDef">The SettingsMenuDef that this caontiner is stored in</param>
        /// <param name="modId">The modId of the active mod in the settings menu</param>
        /// <param name="containers">The list of settings</param>
        /// <param name="name">The name of the list (for error reporting purposes)</param>
        /// <returns>Returns <c>false</c> if there was an error, <c>true</c> otherwise</returns>
        protected bool InitializeContainers(SettingsMenuDef menuDef, IEnumerable<Container> containers, string name = null)
        {
            if (containers != null)
            {
                int c = 0;
                foreach (Container container in containers)
                {
                    c++;
                    if (color != null && container.color == null)
                    {
                        container.color = color;
                    }
                    if (searchTags != null)
                    {
                        if (container.searchTags == null)
                        {
                            searchTags = [];
                        }
                        foreach (string searchTag in searchTags)
                        {
                            container.searchTags.Add(searchTag);
                        }
                    }
                    if (!container.Initialize(menuDef))
                    {
                        if (name != null)
                        {
                            Error("Failed to initialize a " + container.GetType().ToString() + " in <" + name + "> at position=" + c.ToString());
                        }
                        else
                        {
                            Error("Failed to initialize a " + container.GetType().ToString() + " at position = " + c.ToString());
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool PreOpenContainers(IEnumerable<Container> containers, string name = null)
        {
            if (containers != null)
            {
                int c = 0;
                foreach (Container container in containers)
                {
                    c++;
                    if (!container.PreOpenContainer())
                    {
                        if (name != null)
                        {
                            Error("Failed to preopen a " + container.GetType().ToString() + " in <" + name + "> at position=" + c.ToString());
                        }
                        else
                        {
                            Error("Failed to preopen a " + container.GetType().ToString() + " at position = " + c.ToString());
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        protected bool PostCloseContainers(IEnumerable<Container> containers, string name = null)
        {
            if (containers != null)
            {
                int c = 0;
                foreach (Container container in containers)
                {
                    c++;
                    if (!container.PostCloseContainer())
                    {
                        if (name != null)
                        {
                            Error("Failed to postclose a " + container.GetType().ToString() + " in <" + name + "> at position=" + c.ToString());
                        }
                        else
                        {
                            Error("Failed to postclose a " + container.GetType().ToString() + " at position = " + c.ToString());
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Sets the currently displayed menu to the one given
        /// </summary>
        /// <param name="defName">the defName of the SettingsMenuDef you want to display</param>
        protected void SetActiveMenu(string defName)
        {
            if (BaseSettingsWindow.activeMenu != null)
            {
                BaseSettingsWindow.SetActiveMenu(defName);
            }
        }


        protected void WarnUsingObselete(Type[] alternatives)
        {
            XmlMod.WarnUsingObselete(modId, this, alternatives);
        }
    }
}
