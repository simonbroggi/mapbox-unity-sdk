namespace Mapbox.Unity.Ar
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Mapbox.Unity.Location;
	using System;
	using Mapbox.MapMatching;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.Utilities;

	public enum ArTracking { Good, Bad };

	/// <summary>
	/// Centralized locator grants access for ARLocalization variables.
	/// </summary>
	public class CentralizedLocator : SingletonBehaviour<CentralizedLocator>, ISynchronizationContext
	{

		[SerializeField]
		AbstractMap _map;
		public AbstractMap CurrentMap
		{
			get
			{
				return _map;
			}
		}

		[SerializeField]
		NodeSyncBase[] _syncNodes;
		public NodeSyncBase[] SyncNodes
		{
			get
			{
				return _syncNodes;
			}
		}

		[SerializeField]
		Transform _arFirstPerson;
		/// <summary>
		/// Returns the ARCamera Transform. ARCamera Transform is immutable.
		/// </summary>
		/// <value>TReturns Transform.</value>
		public Transform ArFirstPerson
		{
			get
			{
				return _arFirstPerson;
			}
		}

		[SerializeField]
		Transform _ArRoot;
		/// <summary>
		/// Returns the ARRoot Transform.
		/// </summary>
		/// <value>Returns Transform.</value>
		public Transform ARRoot
		{
			get
			{
				return _ArRoot;
			}
		}

		[SerializeField]
		ComputeARLocalizationStrategy _initialLocalizationStrategy;

		[SerializeField]
		ComputeARLocalizationStrategy _generalLocalizationStrategy;

		[SerializeField]
		AbstractAlignmentStrategy _alignmentStrategy;

		public event Action<Alignment> OnAlignmentAvailable;

		//Currently this never get's called..
		public event Action OnAlignmentComplete;

		public event Action<ArTracking> OnTrackingStateChanged;

		void Start()
		{
			_alignmentStrategy.Register(this);

			// Initialize all sync-nodes.Make them ready to recieve node data.
			// Map needs to be generated before init. Otherwise bunch of errors.
			_map.OnInitialized += () => InitializeSyncNodes();
			_map.OnInitialized += Map_OnInitialized;
		}

		void Map_OnInitialized()
		{
			_map.OnInitialized -= Map_OnInitialized;
			// We don't want location updates until we have a map, otherwise our conversion will fail.
			ComputeFirstLocalization();
			Debug.Log("First map position: " + _map.CenterLatitudeLongitude);
		}

		/// <summary>
		/// Computes the first localization.
		/// </summary>
		protected void ComputeFirstLocalization()
		{
			_initialLocalizationStrategy.OnLocalizationComplete += OnFirstLocalizationComplete;
			_initialLocalizationStrategy.ComputeLocalization(this);
		}

		/// <summary>
		/// Delegate that gets triggered when first localization computation is complete
		/// </summary>
		/// <param name="alignment">Alignment from the first localization.</param>
		void OnFirstLocalizationComplete(Alignment alignment)
		{
			_initialLocalizationStrategy.OnLocalizationComplete -= OnFirstLocalizationComplete;
			//_alignmentStrategy.OnAlignmentApplicationComplete += OnAlignmentApplicationComplete;
			// Localization is complete. Now call AlignmentStrategy to align the map

			if (OnAlignmentAvailable != null)
			{
				OnAlignmentAvailable(alignment);
			}

			//We want Syncronize to be called when location is updated. This could extend to any other polling methods in the future.
			LocationProviderFactory.Instance.DefaultLocationProvider.OnLocationUpdated += SyncronizeNodesToFindAlignment;

			//Save new nodes for each type of sync node.
			SaveNodes();
		}

		/// <summary>
		/// Computes the general localization. Localization strategy to be used after first localization.
		/// </summary>
		void ComputeGeneralLocalization()
		{
			_generalLocalizationStrategy.OnLocalizationComplete += OnGeneralLocalizationComplete;
			_generalLocalizationStrategy.ComputeLocalization(this);
		}
		/// <summary>
		/// Delegate that gets triggered when the general localization is complete.
		/// </summary>
		/// <param name="alignment">Alignment.</param>
		void OnGeneralLocalizationComplete(Alignment alignment)
		{
			_generalLocalizationStrategy.OnLocalizationComplete -= OnGeneralLocalizationComplete;
			//_alignmentStrategy.OnAlignmentApplicationComplete += OnAlignmentApplicationComplete;
			// Localization is complete. Now call AlignmentStrategy to align the map

			if (OnAlignmentAvailable != null)
			{
				OnAlignmentAvailable(alignment);
			}
		}

		//void OnAlignmentApplicationComplete()
		//{
		//	if (OnAlignmentComplete != null)
		//	{
		//		OnAlignmentComplete();
		//	}
		//}

		/// <summary>
		/// Syncronizes the nodes to find alignment.
		/// </summary>
		/// <param name="location">Location.</param>
		protected void SyncronizeNodesToFindAlignment(Location location)
		{
			// Our location provider just got a new update.
			// We now ask all our nodes to update and save the most recent node.
			// Sync Nodes should also update a "Quality/Accuracy" metric.
			// Quality/Accuracy metric will be used to determine whether the node will be considered for the alignment computation or not.
			Debug.Log("SyncronizeNodesToFindAlignment");

			foreach (var node in _syncNodes)
			{
				node.SaveNode();
			}
			// Compute Alignment
			ComputeGeneralLocalization();
		}

		/// <summary>
		/// Initializes the sync nodes.
		/// </summary>
		protected void InitializeSyncNodes()
		{
			for (int i = 0; i < _syncNodes.Length; i++)
			{
				_syncNodes[i].InitializeNodeBase(_map);
			}
		}

		/// <summary>
		/// Saves the nodes.
		/// </summary>
		protected void SaveNodes()
		{
			foreach (var node in _syncNodes)
			{
				node.SaveNode();
			}
		}

	}
}
