namespace Mapbox.Unity.MeshGeneration.Factories
{
	using UnityEngine;
	using Mapbox.Directions;
	using System.Collections.Generic;
	using System.Linq;
	using Mapbox.Unity.Map;
	using Data;
	using Modifiers;
	using Mapbox.Utils;
	using Mapbox.Unity.Utilities;
	using System.Collections;
	using Mapbox.Examples;
	using System;

	public enum RouteProfileTypes
	{
		Driving,
		Walking,
		Cycling
	}

	public class DirectionsFactory : MonoBehaviour
	{
		[SerializeField]
		AbstractMap _map;

		public RouteProfileTypes RouteType;
		public ModifierStack RouteStyle;

		[SerializeField]
		private DragableDirectionWaypoint[] _waypoints; //taking pin objects through inspector 
		private List<DragableDirectionWaypoint> _cachedWaypoints; // caching waypoints to detect changes
		private Directions _directions;
		private GameObject _resultRouteGameObject;

		[SerializeField]
		private GameObject AnimationObject;
		[SerializeField]
		private float _speed;
		private Coroutine _animCoroutine;

		protected virtual void Awake()
		{
			if (_map == null)
			{
				_map = FindObjectOfType<AbstractMap>();
			}
			_directions = MapboxAccess.Instance.Directions;
			_map.OnInitialized += Query;
			_map.OnUpdated += Query;
		}

		public void Start()
		{
			if (_map == null)
			{
				Debug.Log("Can't find a Mapbox Map in the scene");
				return;
			}

			_cachedWaypoints = new List<DragableDirectionWaypoint>(_waypoints.Length);
			foreach (var item in _waypoints)
			{
				_cachedWaypoints.Add(item);
				item.PinDropped += (s, e) =>
				{
					Query();
				};
			}

			foreach (var modifier in RouteStyle.MeshModifiers)
			{
				modifier.Initialize();
			}
		}

		public void Update()
		{
			if (_waypoints.Length != _cachedWaypoints.Count)
			{
				_cachedWaypoints.Clear();
				foreach (var item in _waypoints)
				{
					_cachedWaypoints.Add(item);
				}
			}
		}

		public void OnValidate()
		{
			if (_map != null && _directions != null)
			{
				Query();
			}
		}

		protected virtual void OnDestroy()
		{
			_map.OnInitialized -= Query;
			_map.OnUpdated -= Query;
		}

		void Query()
		{
			if (_map == null || _directions == null)
				return;

			var count = _waypoints.Length;
			var wp = new Vector2d[count];
			for (int i = 0; i < count; i++)
			{
				wp[i] = _waypoints[i].transform.GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);
			}
			var _directionResource = new DirectionResource(wp, GetRouteType(RouteType));
			_directionResource.Steps = true;
			_directions.Query(_directionResource, HandleDirectionsResponse);
		}

		private RoutingProfile GetRouteType(RouteProfileTypes routeType)
		{
			switch (routeType)
			{
				case RouteProfileTypes.Driving:
					return RoutingProfile.Driving;
				case RouteProfileTypes.Walking:
					return RoutingProfile.Walking;
				case RouteProfileTypes.Cycling:
					return RoutingProfile.Cycling;
				default:
					return RoutingProfile.Driving;
			}
		}

		void HandleDirectionsResponse(DirectionsResponse response)
		{
			if (response == null || null == response.Routes || response.Routes.Count < 1)
			{
				return;
			}

			var meshData = new MeshData();
			var dat = new List<Vector3>();
			foreach (var point in response.Routes[0].Geometry)
			{
				dat.Add(Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz());
			}

			_cachedWaypoints[0].MoveTarget.transform.position = dat[0];
			_cachedWaypoints[_cachedWaypoints.Count - 1].MoveTarget.transform.position = dat[dat.Count - 1];

			var feat = new VectorFeatureUnity();
			feat.Points.Add(dat);

			if (_resultRouteGameObject != null)
			{
				Destroy(_resultRouteGameObject);
			}

			_resultRouteGameObject = RouteStyle.Execute(null, feat, meshData);

			if(_animCoroutine != null)
			{
				StopCoroutine(_animCoroutine);
			}
			_animCoroutine = StartCoroutine(AnimateRoute(dat));
		}

		private IEnumerator AnimateRoute(List<Vector3> dat)
		{
			AnimationObject.transform.position = dat[0];

			for (int i = 0; i < dat.Count; i++)
			{
				while (AnimationObject.transform.position != dat[i])
				{
					AnimationObject.transform.position = Vector3.MoveTowards(AnimationObject.transform.position, dat[i], _speed * Time.deltaTime);
					yield return null;
				}
			}

			//while(ind < dat.Count - 1)
			//{
			//	if(Vector3.Distance(AnimationBall.transform.position, dat[ind + 1]) > 0.2f)
			//	{
			//		var vec = Vector3.MoveTowards(AnimationBall.transform.position, dat[ind + 1], _speed);
			//		AnimationBall.transform.position = vec;
			//	}
			//	else
			//	{
			//		ind++;
			//	}
			//	yield return null;
			//}
		}
	}

}
