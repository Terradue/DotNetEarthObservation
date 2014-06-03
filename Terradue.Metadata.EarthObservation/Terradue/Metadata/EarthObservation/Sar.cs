﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.38967
//    <NameSpace>Terradue.Metadata.EarthObservation</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>True</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>True</HidePrivateFieldInIDE><EnableSummaryComment>True</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>True</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>True</GenerateDataContracts><CodeBaseTag>Net40</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>True</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace Terradue.Metadata.EarthObservation
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Runtime.Serialization;


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "EarthObservationType", Namespace = "http://www.opengis.net/sar/2.0")]
    [System.Xml.Serialization.XmlRootAttribute("EarthObservation", Namespace = "http://www.opengis.net/sar/2.0", IsNullable = false)]
    [System.Runtime.Serialization.DataContractAttribute(Name = "EarthObservationType1", Namespace = "http://www.opengis.net/sar/2.0")]
    public partial class EarthObservationType1 : EarthObservationType, System.ComponentModel.INotifyPropertyChanged
    {

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "AcquisitionType", Namespace = "http://www.opengis.net/sar/2.0")]
    [System.Xml.Serialization.XmlRootAttribute("Acquisition", Namespace = "http://www.opengis.net/sar/2.0", IsNullable = false)]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AcquisitionType1", Namespace = "http://www.opengis.net/sar/2.0")]
    public partial class AcquisitionType1 : AcquisitionType, System.ComponentModel.INotifyPropertyChanged
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string polarisationModeField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string polarisationChannelsField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private string antennaLookDirectionField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private AngleType minimumIncidenceAngleField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private AngleType maximumIncidenceAngleField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private AngleType incidenceAngleVariationField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private MeasureType dopplerFrequencyField;

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string polarisationMode
        {
            get
            {
                return this.polarisationModeField;
            }
            set
            {
                if ((this.polarisationModeField != null))
                {
                    if ((polarisationModeField.Equals(value) != true))
                    {
                        this.polarisationModeField = value;
                        this.OnPropertyChanged("polarisationMode");
                    }
                }
                else
                {
                    this.polarisationModeField = value;
                    this.OnPropertyChanged("polarisationMode");
                }
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string polarisationChannels
        {
            get
            {
                return this.polarisationChannelsField;
            }
            set
            {
                if ((this.polarisationChannelsField != null))
                {
                    if ((polarisationChannelsField.Equals(value) != true))
                    {
                        this.polarisationChannelsField = value;
                        this.OnPropertyChanged("polarisationChannels");
                    }
                }
                else
                {
                    this.polarisationChannelsField = value;
                    this.OnPropertyChanged("polarisationChannels");
                }
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string antennaLookDirection
        {
            get
            {
                return this.antennaLookDirectionField;
            }
            set
            {
                if ((this.antennaLookDirectionField != null))
                {
                    if ((antennaLookDirectionField.Equals(value) != true))
                    {
                        this.antennaLookDirectionField = value;
                        this.OnPropertyChanged("antennaLookDirection");
                    }
                }
                else
                {
                    this.antennaLookDirectionField = value;
                    this.OnPropertyChanged("antennaLookDirection");
                }
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public AngleType minimumIncidenceAngle
        {
            get
            {
                return this.minimumIncidenceAngleField;
            }
            set
            {
                if ((this.minimumIncidenceAngleField != null))
                {
                    if ((minimumIncidenceAngleField.Equals(value) != true))
                    {
                        this.minimumIncidenceAngleField = value;
                        this.OnPropertyChanged("minimumIncidenceAngle");
                    }
                }
                else
                {
                    this.minimumIncidenceAngleField = value;
                    this.OnPropertyChanged("minimumIncidenceAngle");
                }
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public AngleType maximumIncidenceAngle
        {
            get
            {
                return this.maximumIncidenceAngleField;
            }
            set
            {
                if ((this.maximumIncidenceAngleField != null))
                {
                    if ((maximumIncidenceAngleField.Equals(value) != true))
                    {
                        this.maximumIncidenceAngleField = value;
                        this.OnPropertyChanged("maximumIncidenceAngle");
                    }
                }
                else
                {
                    this.maximumIncidenceAngleField = value;
                    this.OnPropertyChanged("maximumIncidenceAngle");
                }
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public AngleType incidenceAngleVariation
        {
            get
            {
                return this.incidenceAngleVariationField;
            }
            set
            {
                if ((this.incidenceAngleVariationField != null))
                {
                    if ((incidenceAngleVariationField.Equals(value) != true))
                    {
                        this.incidenceAngleVariationField = value;
                        this.OnPropertyChanged("incidenceAngleVariation");
                    }
                }
                else
                {
                    this.incidenceAngleVariationField = value;
                    this.OnPropertyChanged("incidenceAngleVariation");
                }
            }
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public MeasureType dopplerFrequency
        {
            get
            {
                return this.dopplerFrequencyField;
            }
            set
            {
                if ((this.dopplerFrequencyField != null))
                {
                    if ((dopplerFrequencyField.Equals(value) != true))
                    {
                        this.dopplerFrequencyField = value;
                        this.OnPropertyChanged("dopplerFrequency");
                    }
                }
                else
                {
                    this.dopplerFrequencyField = value;
                    this.OnPropertyChanged("dopplerFrequency");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "AcquisitionPropertyType", Namespace = "http://www.opengis.net/sar/2.0")]
    [System.Xml.Serialization.XmlRootAttribute("AcquisitionPropertyType", Namespace = "http://www.opengis.net/sar/2.0", IsNullable = true)]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AcquisitionPropertyType1", Namespace = "http://www.opengis.net/sar/2.0")]
    public partial class AcquisitionPropertyType1 : System.ComponentModel.INotifyPropertyChanged
    {

        [EditorBrowsable(EditorBrowsableState.Never)]
        private AcquisitionType1 acquisitionField;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ownsField;

        public AcquisitionPropertyType1()
        {
            this.acquisitionField = new AcquisitionType1();
            this.ownsField = false;
        }

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public AcquisitionType1 Acquisition
        {
            get
            {
                return this.acquisitionField;
            }
            set
            {
                if ((this.acquisitionField != null))
                {
                    if ((acquisitionField.Equals(value) != true))
                    {
                        this.acquisitionField = value;
                        this.OnPropertyChanged("Acquisition");
                    }
                }
                else
                {
                    this.acquisitionField = value;
                    this.OnPropertyChanged("Acquisition");
                }
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool owns
        {
            get
            {
                return this.ownsField;
            }
            set
            {
                if ((ownsField.Equals(value) != true))
                {
                    this.ownsField = value;
                    this.OnPropertyChanged("owns");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null))
            {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
