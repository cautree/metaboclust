﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaboliteLevels.Data.Session.Associational;
using MetaboliteLevels.Data.Session.General;
using MetaboliteLevels.Gui.Controls.Lists;
using MetaboliteLevels.Properties;
using MetaboliteLevels.Utilities;
using MGui.Datatypes;

namespace MetaboliteLevels.Data.Session.Main
{
    enum EAnnotation
    {
        Tentative = 0,
        Affirmative = 1,
        Confirmed=2,
    }

    /// <summary>
    /// An annotation;
    /// </summary>
    [Serializable]
    class Annotation : Associational
    {
        [XColumn()]
        public readonly Peak Peak;
        [XColumn()]
        public readonly Compound Compound;
        [XColumn()]
        public readonly Adduct Adduct;

        /// <summary>
        /// Superfluous information in source file
        /// </summary>
        public readonly MetaInfoCollection Meta = new MetaInfoCollection();

        [XColumn()]
        public readonly EAnnotation Status;                   

        /// <summary>
        /// Constructor
        /// </summary> 
        public Annotation(Peak peak, Compound compound, Adduct adduct, EAnnotation attributes)
        {
            this.Peak = peak;
            this.Compound = compound;
            this.Adduct = adduct;
            this.Status = attributes;
        }

        public override EPrevent SupportsHide => EPrevent.Hide;

        public override string DefaultDisplayName=> this.Compound.DisplayName;      

        public override EVisualClass AssociationalClass=>EVisualClass.Annotation;

        public override Image Icon
        {
            get
            {
                switch (this.Status)
                {
                    case EAnnotation.Tentative:
                        return Resources.IconAnnotationTentative;

                    case EAnnotation.Affirmative:
                        return Resources.IconAnnotationAffirmed;

                    case EAnnotation.Confirmed:
                        return Resources.IconAnnotationConfirmed;

                    default:
                        throw new SwitchException( this.Status );
                }
            }
        }           

        protected override void OnFindAssociations( ContentsRequest list )
        {
            switch (list.Type)
            {
                case EVisualClass.Adduct:
                    list.Text = "Adduct for annotation {0}";
                    list.Add( this.Adduct );
                    break;

                case EVisualClass.Annotation:
                    ((Associational)this.Peak).FindAssociations( list );
                    ((Associational)this.Compound).FindAssociations( list );
                    list.Text = "Annotations with same peaks/compounds to {0}";
                    break;

                case EVisualClass.Assignment:
                    ((Associational)this.Peak).FindAssociations( list );
                    break;

                case EVisualClass.Cluster:
                    ((Associational)this.Peak).FindAssociations( list );
                    break;

                case EVisualClass.Compound:
                    list.Text = "Compound for annotation {0}";
                    list.Add( this.Compound );
                    break;

                case EVisualClass.Pathway:
                    ((Associational)this.Peak).FindAssociations( list );
                    ((Associational)this.Compound).FindAssociations( list );
                    break;

                case EVisualClass.Peak:
                    list.Text = "Peak for annotation {0}";
                    list.Add( this.Peak );
                    break;
            }
        }

        public override void GetXColumns( CustomColumnRequest request )
        {
            ColumnCollection<Annotation> results = request.Results.Cast< Annotation>();

            request.Core._annotationsMeta.ReadAllColumns<Annotation>( z => z.Meta, results );
        }
    }
}
