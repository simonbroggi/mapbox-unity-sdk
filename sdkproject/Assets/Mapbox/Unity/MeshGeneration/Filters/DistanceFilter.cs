namespace Mapbox.Unity.MeshGeneration.Filters
{
    using UnityEngine;
	using Mapbox.Utils;
    using Mapbox.Unity.MeshGeneration.Data;
	using Mapbox.Unity.Utilities;
	using Mapbox.VectorTile;
	using Mapbox.VectorTile.Geometry;

    [CreateAssetMenu(menuName = "Mapbox/Filters/Distance Filter")]
    public class DistanceFilter : FilterBase
    {
        public enum DistanceFilterOptions
        {
            LessThan,
            GreaterThan
        }

		public string m_latitudeLongitudeString;

        public override string Key { get { return ""; } }
        [SerializeField]
        private float _distance;
        [SerializeField]
        private DistanceFilterOptions _type;

		private void OnValidate()
		{
			_distance = Mathf.Max(0.01f, _distance);
		}

		public override void Initialize()
		{
			Debug.Log("I am a distance filter");
		}

        public override bool Try(VectorFeatureUnity feature)
        {	
			if(feature.WorldSpacePoints.Count > 0)
			{
				var latLon = Conversions.StringToLatLon(m_latitudeLongitudeString);
				var coor = Conversions.GeoToWorldPosition(latLon, new Vector2d(0, 0),100);
				Vector3 worldCoor = new Vector3((float)coor.x, 0f, (float)coor.y);
				var wsp = feature.WorldSpacePoints[0];
				if(wsp.Count > 0)
				{
					Vector3 position = wsp[0];
					
					GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					Vector3 pos = new Vector3(position.x, 10.0f, position.z);
        			sphere.transform.position = pos;

					float distance = (float)Vector3.Distance(worldCoor, position);
					Debug.Log(distance);
					if (_type == DistanceFilterOptions.LessThan && distance < _distance)
					{
						return true;
					}
            		if (_type == DistanceFilterOptions.GreaterThan && distance > _distance)
					{
						return true;
					}
				}	
			}
            return false;
        }
    }
}
