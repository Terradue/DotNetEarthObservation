﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.4.0.38967
//    <NameSpace>Terradue.Metadata.EarthObservation.Ogc.Opt</NameSpace><Collection>Array</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><VirtualProp>False</VirtualProp><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net40</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><EnableEncoding>False</EnableEncoding><AutomaticProperties>False</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>True</ExcludeIncludedTypes><EnableInitializeFields>False</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
using Terradue.Metadata.EarthObservation.Ogc.Om;
using Terradue.GeoJson.Gml;
using Terradue.Metadata.EarthObservation.Ogc.Eop;


namespace Terradue.Metadata.EarthObservation.Ogc.Opt
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "EarthObservationType", Namespace = "http://www.opengis.net/opt/2.1")]
    [System.Xml.Serialization.XmlRootAttribute("EarthObservation", Namespace = "http://www.opengis.net/opt/2.1", IsNullable = false)]
    public partial class OptEarthObservationType : EarthObservationType
    {

        private object resultField;

        [System.Xml.Serialization.XmlElementAttribute("result", Namespace = "http://www.opengis.net/om/2.1", Type=typeof(OptEarthObservationResultPropertyType))]
        public override object result
        {
            get
            {
                return this.resultField;
            }
            set
            {
                this.resultField = value;
            }
        }
    }


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "EarthObservationResultType", Namespace = "http://www.opengis.net/opt/2.1")]
    [System.Xml.Serialization.XmlRootAttribute("EarthObservationResult", Namespace = "http://www.opengis.net/opt/2.1", IsNullable = false)]
    public partial class OptEarthObservationResultType : EarthObservationResultType
    {

        private MeasureType cloudCoverPercentageField;

        private MeasureType cloudCoverPercentageAssessmentConfidenceField;

        private string cloudCoverPercentageQuotationModeField;

        private MeasureType snowCoverPercentageField;

        private MeasureType snowCoverPercentageAssessmentConfidenceField;

        private string snowCoverPercentageQuotationModeField;

        [System.Xml.Serialization.XmlElementAttribute()]
        public MeasureType cloudCoverPercentage
        {
            get
            {
                return this.cloudCoverPercentageField;
            }
            set
            {
                this.cloudCoverPercentageField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public MeasureType cloudCoverPercentageAssessmentConfidence
        {
            get
            {
                return this.cloudCoverPercentageAssessmentConfidenceField;
            }
            set
            {
                this.cloudCoverPercentageAssessmentConfidenceField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public string cloudCoverPercentageQuotationMode
        {
            get
            {
                return this.cloudCoverPercentageQuotationModeField;
            }
            set
            {
                this.cloudCoverPercentageQuotationModeField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public MeasureType snowCoverPercentage
        {
            get
            {
                return this.snowCoverPercentageField;
            }
            set
            {
                this.snowCoverPercentageField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public MeasureType snowCoverPercentageAssessmentConfidence
        {
            get
            {
                return this.snowCoverPercentageAssessmentConfidenceField;
            }
            set
            {
                this.snowCoverPercentageAssessmentConfidenceField = value;
            }
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public string snowCoverPercentageQuotationMode
        {
            get
            {
                return this.snowCoverPercentageQuotationModeField;
            }
            set
            {
                this.snowCoverPercentageQuotationModeField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/opt/2.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.opengis.net/opt/2.1", IsNullable = true)]
    public partial class OptEarthObservationResultPropertyType
    {

        private OptEarthObservationResultType earthObservationResultField;

        private string nilReasonField;

        private string remoteSchemaField;

        private bool ownsField;

        public OptEarthObservationResultPropertyType()
        {
            this.ownsField = false;
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public OptEarthObservationResultType EarthObservationResult
        {
            get
            {
                return this.earthObservationResultField;
            }
            set
            {
                this.earthObservationResultField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nilReason
        {
            get
            {
                return this.nilReasonField;
            }
            set
            {
                this.nilReasonField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2", DataType = "anyURI")]
        public string remoteSchema
        {
            get
            {
                return this.remoteSchemaField;
            }
            set
            {
                this.remoteSchemaField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool owns
        {
            get
            {
                return this.ownsField;
            }
            set
            {
                this.ownsField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/opt/2.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.opengis.net/opt/2.1", IsNullable = true)]
    public partial class OptEarthObservationPropertyType
    {

        private OptEarthObservationType earthObservationField;

        private string nilReasonField;

        private string remoteSchemaField;

        private bool ownsField;

        public OptEarthObservationPropertyType()
        {
            this.ownsField = false;
        }

        [System.Xml.Serialization.XmlElementAttribute()]
        public OptEarthObservationType EarthObservation
        {
            get
            {
                return this.earthObservationField;
            }
            set
            {
                this.earthObservationField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nilReason
        {
            get
            {
                return this.nilReasonField;
            }
            set
            {
                this.nilReasonField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2", DataType = "anyURI")]
        public string remoteSchema
        {
            get
            {
                return this.remoteSchemaField;
            }
            set
            {
                this.remoteSchemaField = value;
            }
        }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool owns
        {
            get
            {
                return this.ownsField;
            }
            set
            {
                this.ownsField = value;
            }
        }
    }
}
