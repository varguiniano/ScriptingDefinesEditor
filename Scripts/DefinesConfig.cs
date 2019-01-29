using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Varguiniano.ScriptingDefinesEditor
{
    /// <inheritdoc />
    /// <summary>
    /// Class that stores the defines this project uses.
    /// </summary>
    public class DefinesConfig : ScriptableObject
    {
        /// <summary>
        /// Accessor to translate the list to player settings language.
        /// </summary>
        public string DefinesString
        {
            get
            {
                var definesString = "";
                foreach (var pair in DefinesList)
                {
                    if (!pair.Status) continue;
                    definesString += string.IsNullOrEmpty(definesString) ? pair.Define : ";" + pair.Define;
                }

                return definesString;
            }
            set
            {
                foreach (var define in value.Split(';')
                    .Where(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)))
                    AddDefine(define, true);
            }
        }

        /// <summary>
        /// List that stores the list of defines and their status.
        /// </summary>
        public List<DefineStatusPair> DefinesList = new List<DefineStatusPair>();

        /// <summary>
        /// Adds or updates a define from the list.
        /// </summary>
        /// <param name="define">Define.</param>
        /// <param name="status">Status.</param>
        private void AddDefine(string define, bool status)
        {
            var set = false;
            foreach (var defineStatusPair in DefinesList)
                if (defineStatusPair.Define == define)
                {
                    defineStatusPair.Status = status;
                    set = true;
                }

            if (set) return;
            DefinesList.Add(new DefineStatusPair(define, status));
        }
    }

    /// <summary>
    /// Class that stores a define-status pair.
    /// </summary>
    [Serializable]
    public class DefineStatusPair
    {
        /// <summary>
        /// The define.
        /// </summary>
        public string Define;

        /// <summary>
        /// The define's status.
        /// </summary>
        public bool Status;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DefineStatusPair()
        {
            Define = "NEW_DEFINE";
            Status = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="define">Key.</param>
        /// <param name="status">Value.</param>
        public DefineStatusPair(string define, bool status)
        {
            Define = define;
            Status = status;
        }
    }
}