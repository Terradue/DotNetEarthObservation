using System;
using Terradue.OpenSearch.Result;

namespace Terradue.Metadata.EarthObservation.OpenSearch {
    public interface IEarthObservationOpenSearchResultItem : IOpenSearchResultItem {

        Terradue.Metadata.EarthObservation.Ogc.Om.OM_ObservationType EarthObservation { get; }

    }
}

