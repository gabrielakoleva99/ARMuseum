﻿using UnityEngine;
using System.Collections.Generic;
using CW.Common;
using PaintCore;

namespace PaintIn3D
{
	/// <summary>This is the base code for the <b>CwPaintableMesh</b> and <b>CwPaintableMeshAtlas</b> components.</summary>
	[RequireComponent(typeof(Renderer))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwMeshModel")]
	public abstract class CwMeshModel : CwModel
	{
		public enum UseMeshType
		{
			AsIs,
			AutoSeamFix
		}

		/// <summary>Transform the mesh with its position, rotation, and scale? Some skinned mesh setups require this to be disabled.</summary>
		public virtual bool IncludeScale { set { includeScale = value; } get { return includeScale; } } [SerializeField] protected bool includeScale = true;

		/// <summary>This allows you to choose how the <b>Mesh</b> attached to the current <b>Renderer</b> is used when painting.
		/// AsIs = Use what is currently set in the renderer.
		/// AutoSeamFix = Use (or automatically generate) a seam-fixed version of the mesh currently set in the renderer.</summary>
		public UseMeshType UseMesh { set { useMesh = value; } get { return useMesh; } } [SerializeField] protected UseMeshType useMesh;

		[System.NonSerialized]
		private SkinnedMeshRenderer cachedSkinned;

		[System.NonSerialized]
		private bool cachedFilterSet;

		[System.NonSerialized]
		private MeshFilter cachedFilter;

		[System.NonSerialized]
		private bool cachedSkinnedSet;

		[System.NonSerialized]
		private Material[] materials;

		[System.NonSerialized]
		private bool materialsSet;

		[System.NonSerialized]
		protected Mesh bakedMesh;

		[System.NonSerialized]
		protected bool bakedMeshSet;

		[System.NonSerialized]
		protected Mesh preparedMesh;

		[System.NonSerialized]
		protected Matrix4x4 preparedMatrix;

		[System.NonSerialized]
		private int[] preparedTriangles;

		[System.NonSerialized]
		private Vector3[] preparedPositions;

		[System.NonSerialized]
		private Vector2[] preparedCoord0;

		[System.NonSerialized]
		private Vector2[] preparedCoord1;

		[System.NonSerialized]
		protected static List<Vector3> tempVertices = new List<Vector3>();

		public Mesh PreparedMesh
		{
			get
			{
				return preparedMesh;
			}
		}

		public Material[] Materials
		{
			get
			{
				if (materialsSet == false)
				{
					materials    = CachedRenderer.sharedMaterials;
					materialsSet = true;
				}

				return materials;
			}
		}

		public int GetMaterialIndex(Material material)
		{
			if (material != null)
			{
				var materials = Materials;

				for (var i = materials.Length - 1; i >= 0; i--)
				{
					if (materials[i] == material)
					{
						if (CachedRenderer.isPartOfStaticBatch == true)
						{
							var meshRenderer = CachedRenderer as MeshRenderer;
							
							if (meshRenderer != null)
							{
								return meshRenderer.subMeshStartIndex + i;
							}
						}

						return i;
					}
				}
			}

			return -1;
		}

		public override void RemoveComponents()
		{
		}

		protected override void CacheRenderer()
		{
			base.CacheRenderer();

			if (TryCacheRenderer() == false)
			{
				Debug.LogError("This CwModel/CwPaintable (" + name + ") doesn't have a suitable Renderer, so it cannot be painted.", this);
			}
		}

		private bool TryCacheRenderer()
		{
			if (cachedRenderer is SkinnedMeshRenderer)
			{
				cachedSkinned    = (SkinnedMeshRenderer)cachedRenderer;
				cachedSkinnedSet = true;

				return true;
			}
			else if (cachedRenderer is MeshRenderer)
			{
				cachedFilter    = GetComponent<MeshFilter>();
				cachedFilterSet = true;

				return true;
			}

			return false;
		}

		/// <summary>Materials will give you a cached CachedRenderer.sharedMaterials array. If you have updated this array externally then call this to force the cache to update next them it's accessed.</summary>
		[ContextMenu("Dirty Materials")]
		public void DirtyMaterials()
		{
			materialsSet = false;
		}

		public void GetPreparedPoints(int triangleIndex, ref Vector3 pointA, ref Vector3 pointB, ref Vector3 pointC)
		{
			if (prepared == true && preparedMesh != null)
			{
				if (preparedPositions == null) preparedPositions = preparedMesh.vertices;
				if (preparedTriangles == null) preparedTriangles = preparedMesh.triangles;

				pointA = preparedPositions[preparedTriangles[triangleIndex * 3 + 0]];
				pointB = preparedPositions[preparedTriangles[triangleIndex * 3 + 1]];
				pointC = preparedPositions[preparedTriangles[triangleIndex * 3 + 2]];
			}
		}

		public void GetPreparedCoords0(int triangleIndex, ref Vector2 coordA, ref Vector2 coordB, ref Vector2 coordC)
		{
			if (prepared == true && preparedMesh != null)
			{
				if (preparedTriangles == null) preparedTriangles = preparedMesh.triangles;
				if (preparedCoord0    == null) preparedCoord0    = preparedMesh.uv;

				coordA = preparedCoord0[preparedTriangles[triangleIndex * 3 + 0]];
				coordB = preparedCoord0[preparedTriangles[triangleIndex * 3 + 1]];
				coordC = preparedCoord0[preparedTriangles[triangleIndex * 3 + 2]];
			}
		}

		public void GetPreparedCoords1(int triangleIndex, ref Vector2 coordA, ref Vector2 coordB, ref Vector2 coordC)
		{
			if (prepared == true && preparedMesh != null)
			{
				if (preparedTriangles == null) preparedTriangles = preparedMesh.triangles;
				if (preparedCoord1    == null) preparedCoord1    = preparedMesh.uv;

				coordA = preparedCoord1[preparedTriangles[triangleIndex * 3 + 0]];
				coordB = preparedCoord1[preparedTriangles[triangleIndex * 3 + 1]];
				coordC = preparedCoord1[preparedTriangles[triangleIndex * 3 + 2]];
			}
		}

		public override void GetPrepared(ref Mesh mesh, ref Matrix4x4 matrix, CwCoord coord)
		{
			if (prepared == false)
			{
				prepared = true;

				if (cachedRendererSet == false)
				{
					CacheRenderer();
				}

				TryGetPrepared(coord);
			}

			mesh   = preparedMesh;
			matrix = preparedMatrix;
		}

		private void TryGetPrepared(CwCoord coord)
		{
			if (cachedSkinnedSet == true)
			{
				if (bakedMeshSet == false)
				{
					bakedMesh    = new Mesh();
					bakedMeshSet = true;
				}

				if (useMesh == UseMeshType.AutoSeamFix)
				{
					var skinnedMesh = cachedSkinned.sharedMesh;

					if (skinnedMesh != null && skinnedMesh.name.EndsWith("(Fixed Seams)") == false && skinnedMesh.name.EndsWith("(Fixed)") == false)
					{
						cachedSkinned.sharedMesh = CwMeshFixer.GetCachedMesh(skinnedMesh, coord);
					}
				}

				var lossyScale    = cachedTransform.lossyScale;
				var scaling       = new Vector3(CwHelper.Reciprocal(lossyScale.x), CwHelper.Reciprocal(lossyScale.y), CwHelper.Reciprocal(lossyScale.z));
				var oldLocalScale = cachedTransform.localScale;

				cachedTransform.localScale = Vector3.one;

				cachedSkinned.BakeMesh(bakedMesh);

				cachedTransform.localScale = oldLocalScale;

				preparedMesh   = bakedMesh;
				preparedMatrix = cachedRenderer.localToWorldMatrix;

				if (includeScale == true)
				{
					preparedMatrix *= Matrix4x4.Scale(scaling);
				}
			}
			else if (cachedFilterSet == true)
			{
				preparedMesh   = cachedFilter.sharedMesh;
				preparedMatrix = cachedRenderer.localToWorldMatrix;

				if (useMesh == UseMeshType.AutoSeamFix)
				{
					preparedMesh = CwMeshFixer.GetCachedMesh(preparedMesh, coord);
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			CwHelper.Destroy(bakedMesh);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	using UnityEditor;
	using TARGET = CwMeshModel;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwMeshModel_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("includeScale", "Transform the mesh with its position, rotation, and scale? Some skinned mesh setups require this to be disabled.");
			Draw("useMesh", "This allows you to choose how the Mesh attached to the current Renderer is used when painting.\n\nAsIs = Use what is currently set in the renderer.\n\nAutoSeamFix = Use (or automatically generate) a seam-fixed version of the mesh currently set in the renderer.");
			Draw("hash", "The hash code for this model used for de/serialization of this instance.");

			Separator();

			if (Button("Analyze Mesh") == true)
			{
				CwMeshAnalysis.OpenWith(tgt.gameObject, tgt.PreparedMesh);
			}

			var mesh = PaintCore.CwCommon.GetMesh(tgt.gameObject, tgt.PreparedMesh);

			if (mesh != null && mesh.isReadable == false)
			{
				Error("You must set the Read/Write Enabled setting in this object's Mesh import settings.");
			}
		}
	}
}
#endif