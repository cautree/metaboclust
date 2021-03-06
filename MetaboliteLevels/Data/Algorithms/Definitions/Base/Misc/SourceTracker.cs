﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaboliteLevels.Data.Algorithms.General;

namespace MetaboliteLevels.Data.Algorithms.Definitions.Base.Misc
{
    /// <summary>
    /// Used to track the data used by a configuration.
    /// 
    /// Whilst the configuration itself shouldn't change without a call to ClearResults, the information it points to might itself change.
    /// This class tracks the underlying information.
    /// </summary>
    [Serializable]
    internal class SourceTracker
    {
        // We track by holding (weak) references to the RESULTS to see if they have changed.
        // Note we track the RESULTS stored in the objects, not the objects themselves (which are assumed to be the same!)

        /// <summary>
        /// Tracks <see cref="ArgsBase.SourceMatrix"/>.
        /// </summary>
        private readonly WeakReference _sourceMatrix;

        /// <summary>
        /// Tracks <see cref="ArgsBase.Parameters"/>.
        /// </summary>
        private readonly WeakReference[][] _parameters;

        /// <summary>
        /// CONSTRUCTOR
        /// Remembers the data specified by <paramref name="source"/>.
        /// </summary>    
        public SourceTracker( ArgsBase source )
        {
            var prov = source.SourceProvider?.Provide;
            this._sourceMatrix = prov != null ? new WeakReference( prov ) : null;

            AlgoParameterCollection para = source.GetAlgorithmOrThrow().Parameters;

            this._parameters = new WeakReference[para.Count][];

            for (int n = 0; n < para.Count; ++n)
            {
                this._parameters[n] = para[n].Type.TrackChanges( source.Parameters[n] );
            }
        }                                                   

        /// <summary>
        /// Checks for update. <c>this</c> and <paramref name="source"/> should be the same if nothing has changed!
        /// </summary>
        /// <param name="source">A SourceTracker created with the same source object this was created with.</param>
        /// <returns>true if an update is required, otherwise false.</returns>
        public bool NeedsUpdate( SourceTracker source )
        {
            // Check SOURCE MATRIX
            if (HasChanged( this._sourceMatrix, source._sourceMatrix ))
            {
                return true;
            }

            // Check PARAMETERS
            this.Assert( this._parameters.Length == source._parameters.Length, "_parameters.Length" );
                                               
            for (int iParam = 0; iParam < this._parameters.Length; ++iParam)
            {
                WeakReference[] oldArray = this._parameters[iParam];
                WeakReference[] newArray = source._parameters[iParam];

                if (oldArray == null)
                {
                    this.Assert( newArray == null, "null" );
                    continue;
                }

                this.Assert( oldArray.Length == newArray.Length, "oldArray.Length" );

                for (int iArray = 0; iArray < oldArray.Length; ++iArray)
                {
                    if (HasChanged( oldArray[iArray], newArray[iArray] ))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Assert( bool validation, string id )
        {
            if (!validation)
            {
                throw new InvalidOperationException( "SourceTracker.NeedsUpdate called using parameters are different to those originally used (assertion = " + id + "). ClearResults should have been called if parameters were changed, avoiding this call." );
            }
        }

        /// <summary>
        /// Determines if the value has changed since it was stored.
        /// </summary>                                              
        private static bool HasChanged( WeakReference old, WeakReference @new )
        {
            if (old == null)
            {
                return @new != null;
            }

            if (@new == null)
            {
                return true;
            }               

            object newT = @new.Target;

            if (newT == null)
            {
                throw new InvalidOperationException( "HasChanged called but the NEW object's reference has already expired." );
            }

            object oldT = old.Target;

            if (oldT == null)
            {
                // It's gone and we don't know what it was, it must have changed!
                return true;
            }

            return !object.ReferenceEquals( oldT, newT );
        }
    }
}
