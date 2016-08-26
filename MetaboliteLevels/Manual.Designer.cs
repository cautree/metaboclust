﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MetaboliteLevels {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Manual {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Manual() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MetaboliteLevels.Manual", typeof(Manual).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///
        ///Specify information on LC-MS adducts.
        ///
        ///OPTIONAL: If not specified then no automatic identifications will be performed.
        ///
        ///EXPECTS: CSVFILE
        ///
        ///{}
        ///Text. Row names. Ignored. Mandatory.
        ///
        ///{ADDUCTFILE_NAME_HEADER}
        ///Text. The name of the adduct. Optional.
        ///
        ///{ADDUCTFILE_CHARGE_HEADER}
        ///Numeric. The charge of the adduct (-1 = negative mode LC-MS, 1 = positive mode LS-MS). Required.
        ///
        ///{ADDUCTFILE_MASS_DIFFERENCE_HEADER}
        ///The mass of the adduct. Required.
        ///
        ///{META}
        ///Any additional data the user wishes to provide, unus [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Adducts {
            get {
                return ResourceManager.GetString("Adducts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///
        ///Specify an alternative dataset.
        ///
        ///This may be accessed later for quick viewing, for instance to view unprocessed or raw data.
        ///Only peaks also occurring in the standard dataset are read, these may appear in any order.
        ///
        ///OPTIONAL: If not specified no alternative data will be available.
        ///
        ///EXPECTS: CSVFILE
        ///
        ///{}
        ///Text. Observation names. Must be unique. Mandatory
        ///
        ///{=Text. Peak names. Must be unique. Mandatory.}
        ///One row per observation.
        ///One column per peak.
        ///.
        /// </summary>
        internal static string AlternativeValues {
            get {
                return ResourceManager.GetString("AlternativeValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to kmeans(v, 3)$cluster.
        /// </summary>
        internal static string ClusteringRScriptExample {
            get {
                return ResourceManager.GetString("ClusteringRScriptExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Clusters based on R script.
        ///The script should use the variable &quot;s&quot; (the distance matrix) and return cluster IDs (integer numeric vector)..
        /// </summary>
        internal static string ClusterRScript {
            get {
                return ResourceManager.GetString("ClusterRScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///
        ///Specifies the compounds available for identification. 
        ///
        ///OPTIONAL: If not specified then no automatic identifications will be performed. Note that you can use &quot;identifications&quot; field to specify known identifications.
        ///
        ///EXPECTS: One or more compound libraries. Either a PathwayTools database folder OR a CSV file.
        ///A list of available libraries will be displayed if the library folder has been configured, otherwise the source must be specified manually.
        ///
        ///DATABASE LAYOUT: PathwayTools database folder.
        ///C [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Compounds {
            get {
                return ResourceManager.GetString("Compounds", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///
        ///Names of the experimental conditions.
        ///
        ///OPTIONAL: If not specified conditions will be referred to by their index.
        ///
        ///EXPECTS: CSVFILE
        ///
        ///{}
        ///Text. Row names. Ignored. Mandatory.
        ///
        ///{CONDITIONFILE_ID_HEADER}
        ///Text. The ID corresponding to the &quot;type&quot; column of the observation info file.
        ///
        ///{CONDITIONFILE_NAME_HEADER}
        ///The name of the experimental condition.
        /// </summary>
        internal static string ConditionNames {
            get {
                return ResourceManager.GetString("ConditionNames", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The indices of the control condition(s), these should correspond to the &quot;type&quot; column of your observation file..
        /// </summary>
        internal static string ControlConditions {
            get {
                return ResourceManager.GetString("ControlConditions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {productname} version {version}.
        ///Martin Rusilowicz.
        ///
        ///Additional icons made by Freepik, Google, Appzgear, Vectorgraphit, Icomoon from www.flaticon.com are licensed by CC BY 3.0.
        ///
        ///Download the latest version and source code: https://bitbucket.org/mjr129/metabolitelevels.
        /// </summary>
        internal static string Copyright {
            get {
                return ResourceManager.GetString("Copyright", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the data you&apos;d like to load into the application. Select the &quot;help&quot; button at the bottom to show the side-bar, describing the data you need to provide for each input..
        /// </summary>
        internal static string DataLoadQueryHelp {
            get {
                return ResourceManager.GetString("DataLoadQueryHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This process starts with a &quot;seed&quot; variable to act as an exemplar. The distance of all other variables to this variable is then calculated. The most distant variable is then used as a second exemplar. The variable most distant from their closest exemplar acts as a third exemplar and so forth. The process is repeated until all variables fall within a certain distance range of the exemplars, or optionally until a certain number of exemplars are generated. These exemplars then act as the centres in a standard k [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DKMeansPlusPlus {
            get {
                return ResourceManager.GetString("DKMeansPlusPlus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The indices of the experimental condition(s), these should correspond to the &quot;type&quot; column of your observation file..
        /// </summary>
        internal static string ExperimentalConditions {
            get {
                return ResourceManager.GetString("ExperimentalConditions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///
        ///Use to specify peak-compound annotations manually.
        ///
        ///OPTIONAL: This information is not required.
        ///
        ///NOTE: Purely textual or numeric annotations can be added to the peaks by including this information as extra columns in the peak-info file. See the &quot;𝚂𝚎𝚕𝚎𝚌𝚝 𝙳𝚊𝚝𝚊&quot; page for more details.
        ///
        ///NOTEL Automatic identifications can be performed by selecting the &quot;𝙿𝚎𝚛𝚏𝚘𝚛𝚖 𝚖/𝚣 𝚋𝚊𝚜𝚎𝚍 𝚊𝚞𝚝𝚘𝚖𝚊𝚝𝚒𝚌 𝚒𝚍𝚎𝚗𝚝𝚒𝚏𝚒𝚌𝚊𝚝𝚒𝚘𝚗&quot; option.
        ///
        ///EXPECTS: CSVFILE
        ///
        ///{}
        ///Text. Row names. [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Identifications {
            get {
                return ResourceManager.GetString("Identifications", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///
        ///Specify the intentisities for each peak (row) and observation (column).
        ///
        ///MANDATORY: An intensity matrix must be provided.
        ///
        ///EXPECTS: CSVFILE
        ///
        ///{}
        ///Text. Observation names. Must be unique. Mandatory
        ///
        ///{=Text. Peak names. Must be unique. Mandatory.}
        ///One row per observation.
        ///One column per peak..
        /// </summary>
        internal static string Intensities {
            get {
                return ResourceManager.GetString("Intensities", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specifies the LC-MS mode.
        ///
        ///This is used to determine which adducts are available during compound identification.
        ///
        ///If no compound identification is required then select &quot;none&quot;. This may be the case when using NMR data or if compounds have already been identified.
        ///If the data contains both positive and negative LC data then this should be set to &quot;mixed&quot;, in which case the mode for each peak must be specified in the peak info file.
        ///
        ///EXPECTS: &quot;positive&quot;, &quot;negative&quot;, &quot;mixed&quot; OR &quot;none&quot;..
        /// </summary>
        internal static string LcMsMode {
            get {
                return ResourceManager.GetString("LcMsMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///    
        ///Specify information about the experimental observations.
        ///
        ///MANDATORY: A file must be provided to describe the observations
        ///
        ///EXPECTS: CSVFILE
        ///
        ///{}
        ///Text. The name of the observation. Must be unique. Mandatory.
        ///
        ///{OBSFILE_TIME_HEADER}
        ///Integer. The timestep of the obervation. Optional.
        ///
        ///{OBSFILE_REPLICATE_HEADER}
        ///Integer. The replicate index of the observation. Optional.
        ///
        ///{OBSFILE_GROUP_HEADER}
        ///Text. The experimental group of the observation. Optional.
        ///
        ///{OBSFILE_BATCH_HEADER}
        ///Integer. Batch. Option [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Observations {
            get {
                return ResourceManager.GetString("Observations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If your new session has a very similar setup to a previous session you can select that as your template here. Select your old session from the drop-down list or browse for it to use it as the template. Alternatively start with a clean setup by selecting the blank template..
        /// </summary>
        internal static string RecentSessions {
            get {
                return ResourceManager.GetString("RecentSessions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SCRIPTING:
        ///
        ///Enter your R script into the window. You should follow the outline above to use the inputs to generate the necessary output.
        ///
        ///RENAMING INPUTS:
        ///
        ///Script input parameters can be renamed as by entering a special line starting &quot;##&quot; at the start of the document, such as:
        ///
        ///    ## my.x = value.matrix
        ///
        ///Here &quot;value.matrix&quot; is the ID of the parameter (as listed above) and &quot;my.x&quot; is the new name you wish to assign to it.
        ///
        ///DEFAULT INPUTS:
        ///
        ///If you don&apos;t rename any inputs they will be given the default names s [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RScript {
            get {
                return ResourceManager.GetString("RScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specify the code for your algorithm.
        ///
        ///You can use &quot;##&quot; to give names to script parameters, for example the number of clusters &quot;k&quot; in k-means might be specified as:
        ///
        ///## k = integer
        ///
        ///The available parameter types are: integer, double
        ///
        ///Standard inputs are also provided, for instance clustering requires a value matrix
        ///
        ///## x = value.matrix
        ///
        ///The available inputs for the type of algorithm you are editing are shown in the caption below the title bar.
        ///If you don&apos;t specify an input they will be given the names shown  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string RScriptEditor {
            get {
                return ResourceManager.GetString("RScriptEditor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data can be saved in sereral formats.
        ///MS-NRFB is the fastest and most compact format and is recommended for speed and reliability, however this format is unfortunately unable to cope with large object graph sizes (i.e. datasets with lots of discrete objects), the MJR-SBF format provides a fallback in this case.
        ///.
        /// </summary>
        internal static string SaveData {
            get {
                return ResourceManager.GetString("SaveData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Use to restore cluster assignments.
        ///
        ///NO-TEXT please report bug in Manual.resx::SavedSession! .
        /// </summary>
        internal static string SavedSession {
            get {
                return ResourceManager.GetString("SavedSession", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Give your session a descriptive name to identify it.
        ///
        ///You can always change the name later.
        ///
        ///MANDATORY: This information is required
        ///
        ///EXPECTS: Any text
        ///
        ///    .
        /// </summary>
        internal static string Session {
            get {
                return ResourceManager.GetString("Session", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to kmeans(v, 3)$cluster
        ///
        ///[Unable to load session]
        ///The selected session could not be loaded.
        ///
        ///* The file may be missing or corrupt.
        ///* The file may have been saved using a version of the application which isn&apos;t compatible with the current version.
        ///
        ///Please recreate the session from the original files..
        /// </summary>
        internal static string StatisticsRScriptExample {
            get {
                return ResourceManager.GetString("StatisticsRScriptExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The selected session could not be loaded.
        ///
        ///* The file may be missing or corrupt.
        ///* The file may have been saved using a version of the application which isn&apos;t compatible with the current version.
        ///
        ///Please recreate the session from the original files..
        /// </summary>
        internal static string UnableToLoadSession {
            get {
                return ResourceManager.GetString("UnableToLoadSession", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FILEFORMAT
        ///
        ///Specify information about the dependent variables in the data matrix.
        ///
        ///NOTE: To avoid ambiguity the software refers to the dependent variables as &quot;peaks&quot; and their values for a particular observation as &quot;intensities&quot;, in reality these can represent any dependent variable.
        ///
        ///MANDATORY: Peak information must be provided.
        ///
        ///{}
        ///Text. The name of the variable. Must be unique. Mandatory.
        ///    
        ///{VARFILE_MZ_HEADER}
        ///Numeric. m/z value for the peak. Optional.
        ///
        ///{VARFILE_MODE_HEADER}
        ///Numeric. LC-MS mode for th [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Variables {
            get {
                return ResourceManager.GetString("Variables", resourceCulture);
            }
        }
    }
}
