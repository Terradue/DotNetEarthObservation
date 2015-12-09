using System;
using Terradue.OpenSearch.Result;
using Terradue.ServiceModel.Ogc.Om;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public interface IEarthObservationOpenSearchResultItem : IOpenSearchResultItem {

        OM_ObservationType EarthObservation { get; }

    }
}

