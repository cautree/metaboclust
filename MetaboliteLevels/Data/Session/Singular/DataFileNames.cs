﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MetaboliteLevels.Data.Session.Associational;
using MetaboliteLevels.Data.Session.General;
using MetaboliteLevels.Types.General;
using MetaboliteLevels.Utilities;
using MGui.Helpers;

namespace MetaboliteLevels.Data.Session.Singular
{
    [Serializable]
    class DataFileNames
    {
        // files
        public string Data;                             // data file
        public string ObservationInfo;                  // obs info
        public string PeakInfo;                         // peak info
        public List<CompoundLibrary> CompoundLibraies;  // pathways file
        public List<string> AdductLibraries;            // adducts file
        public string Identifications;                  // annotations file
        public string AltData;                          // alt data file
        public string ConditionInfo;                    // condition info file

        // modes
        public ELcmsMode LcmsMode;               // lc-ms mode
        public bool AutomaticIdentifications;   // for annotations - whether to auto-id
        public EAnnotation AutomaticIdentificationsStatus;   // for annotations - initial status
        public EAnnotation ManualIdentificationsStatus;   // for annotations - default status
        public bool PeakPeakMatching;   // for annotations - whether to compare peaks to peaks
        public decimal AutomaticIdentificationsToleranceValue; // for annotations - m/z tolerance
        public ETolerance AutomaticIdentificationsToleranceUnit; // for annotations - m/z tolerance unit

        // stats
        public List<string> ConditionsOfInterestString;  // for stats - conditions of interest
        public List<string> ControlConditionsString;  // for stats - conditions of interest
        public EAutoCreateOptions _standardAutoCreateOptions;  // for stats - what methods to apply

        // session info
        public string Title;                    // session title
        public string ShortTitle;               // session short title (optional)
        public string Comments;                 // session comments

        // session file
        public string Session;                  // session (saved data)
        public bool ForceSaveAs;                // only allow "save as" not "save" (set if saving might lose data)
        public bool? SaveOnClose;               // save automatically on close
        public Version AppVersion;              // version of app saved using      

        /// <summary>
        /// Gets the title
        /// </summary>
        internal string GetDescription()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                return "Untitled session";
            }

            return Title;
        }

        /// <summary>
        /// String summary.
        /// </summary>
        /// <returns></returns>
        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("--- Application information ---");
            sb.AppendLine("Title: " + UiControls.Title);
            sb.AppendLine("Version: " + UiControls.Version);

            sb.AppendLine("--- Session information ---");
            sb.AppendLine("Title: " + Title);
            sb.AppendLine("ShortTitle: " + ShortTitle);
            sb.AppendLine("Comments: " + ShortTitle);

            sb.AppendLine("--- Session file ---");
            sb.AppendLine("Session: " + Session);
            sb.AppendLine("File Version: " + AppVersion);
            sb.AppendLine("Version warning: " + ForceSaveAs);

            sb.AppendLine("--- Data files ---");
            sb.AppendLine("LcmsMode: " + LcmsMode);
            sb.AppendLine("Data: " + Data);
            sb.AppendLine("ObservationInfo: " + ObservationInfo);
            sb.AppendLine("PeakInfo: " + PeakInfo);
            sb.AppendLine("AltData: " + AltData);

            sb.AppendLine("--- Compound information ---");
            sb.AppendLine("Compounds: " + StringHelper.ArrayToString(CompoundLibraies));
            sb.AppendLine("Adducts: " + StringHelper.ArrayToString(AdductLibraries));
            sb.AppendLine("Identifications: " + Identifications);
            sb.AppendLine("AutomaticIdentifications: " + AutomaticIdentifications);

            sb.AppendLine("--- Experimental information ---");
            sb.AppendLine("ConditionInfo: " + ConditionInfo);
            sb.AppendLine("ConditionsOfInterest: " + StringHelper.ArrayToString(ConditionsOfInterestString));
            sb.AppendLine("ControlConditions: " + StringHelper.ArrayToString(ControlConditionsString));
            sb.AppendLine("_standardAutoCreateOptions: " + _standardAutoCreateOptions.ToString());

            return sb.ToString();
        }

        internal string GetShortTitle()
        {
            if (!string.IsNullOrWhiteSpace(ShortTitle))
            {
                return ShortTitle;
            }
            else if (Title.Length <= 24)
            {
                return Title;
            }
            else if (Title.Contains(' '))
            {
                return Title.Substring(0, Title.LastIndexOf(' ', 24));
            }
            else
            {
                return Title.Substring(0, 24);
            }
        }
    }

    enum EDefaultTrendGenerator
    {
        [Name("None (recommended)")]
        None,

        [Name( "Median of replicates" )]
        MedianOfReplicates,

        [Name( "Mean of replicates" )]
        MeanOfReplicates,
    }
}
