using System;
using System.Collections.Generic;
using System.Linq;

namespace Terradue.Metadata.EarthObservation {
    public class OrbitStateVectorMatrix {

        List<OrbitStateVector> orbits;

        double[] coef_x;
        double[] coef_y;
        double[] coef_z;

        public OrbitStateVectorMatrix(OrbitStateVector[] orbits) {
            this.orbits = orbits.ToList().OrderBy(o => o.Time);
        }

        public void ComputeCoefficient(){
            coef_x = Utils.SplineInterpol(orbits.Select<OrbitStateVector, DateTime>(o => o.Time).ToArray(), orbits.Select<OrbitStateVector, double>(o => o.X).ToArray());
            coef_y = Utils.SplineInterpol(orbits.Select<OrbitStateVector, DateTime>(o => o.Time).ToArray(), orbits.Select<OrbitStateVector, double>(o => o.Y).ToArray());
            coef_z = Utils.SplineInterpol(orbits.Select<OrbitStateVector, DateTime>(o => o.Time).ToArray(), orbits.Select<OrbitStateVector, double>(o => o.Z).ToArray());
        }

    }
}

